using Energistics.Common;
using Energistics.DataAccess.WITSML141.ReferenceData;
using Energistics.Datatypes;
using Energistics.Datatypes.ChannelData;
using Energistics.Protocol.ChannelStreaming;
using Energistics.Protocol.Core;
using MyPDSV0._9.Models;
using PDS.WITSMLstudio;
using PDS.WITSMLstudio.Desktop.Core.Connections;
using PDS.WITSMLstudio.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace MyPDSV0._9.Porxies
{
    public class EtpChannelStreamingProxy : EtpProxy
    {
        //private static readonly log4net
        private readonly Random _random;

        /// <summary>
        /// 模拟数据中的每一道的道头描述;流传输的消息；
        /// </summary>
        public IList<ChannelMetadataRecord> Channels { get; }
        public IList<ChannelStreamingInfo> ChannelStreamingInfo { get; }
        
       /// <summary>
       /// 构造函数
       /// </summary>
       /// <param name="dataSchemaVersion"></param>
       /// <param name="log"></param>
        public EtpChannelStreamingProxy(string dataSchemaVersion,Action<string> log) : base(dataSchemaVersion, log)
        {
            _random = new Random(246);
            Channels = new List<ChannelMetadataRecord>();
            ChannelStreamingInfo = new List<ChannelStreamingInfo>();
        }

        /// <summary>
        /// 重写基类的虚函数，用于开始流传输
        /// </summary>
        /// <param name="model">存储了各道信息的Json文件的序列化对象</param>
        /// <param name="token">是否取消作业的凭证</param>
        /// <param name="interval">模拟数据生成时间间隔</param>
        /// <returns></returns>
        public override async Task Start(Simulation model, CancellationToken token, int interval = 5000)
        {
            Model = model;//基类中定义 public Models.Simulation Model { get; protected set; }
            using (Client = Model.EtpConnection.CreateEtpClient(Model.Name, Model.Version))
            {
                ///<summary>
                ///指定客户端的职能：生产数据流
                /// </summary>
                Client.Register<IChannelStreamingProducer, ChannelStreamingProducerHandler>();
                ///<remarks>
                /// 配置客户端生成数据时操作
                /// </remarks>
                Client.Handler<IChannelStreamingProducer>().OnStart += OnStart;//线程启动事件
                Client.Handler<IChannelStreamingProducer>().OnChannelDescribe += OnChannelDescribe;//获取道头描述事件
                Client.Handler<IChannelStreamingProducer>().OnChannelStreamingStart += OnChannelStreamingStart;//传输流开始事件
                Client.Handler<IChannelStreamingProducer>().OnChannelStreamingStop += OnChannelStreamingStop;//传输流结束事件
                ///<remarks>
                ///For simple streaming:
                //•  always sends all of its channels.
                //• The producer MUST NOT send any data until the Start message is received.The Start message
                //indicates that the consumer is ready to receive data and establishes any rate-control or throttling parameters.
                //• The producer sends at least one ChannelMetadata message, indicating the channels it will stream.
                //• After this, the producer can begin streaming ChannelData.
                //When a producer identifies itself as a SimpleStreamer, the producer and the consumer MUST NOT use
                //any messages other than Start, ChannelMetadata and ChannelData.
                /// </remarks>
                Client.Handler<IChannelStreamingProducer>().IsSimpleStreamer = Model.IsSimpleStreamer;
                Client.Handler<IChannelStreamingProducer>().DefaultDescribeUri = EtpUri.RootUri;
                Client.SocketClosed += OnClientSocketClosed;
                //绑定输出路径
                Client.Output = Log;
                //开启客户端
                Client.Open();

                //不停的生成数据并上传到服务器中
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    try
                    {
                        //在新的线程里开始任务
                        await Task.Delay(interval,token);
                    }
                    catch (TaskCanceledException)
                    {

                        break;
                    }
                }
                TaskRunner.Stop();
                Client.Handler<ICoreClient>().CloseSession("Streaming stopped");
            }
        }

    
       /// <summary>
       /// 核心函数，传输流事件的入口函数
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void OnStart(object sender, ProtocolEventArgs<Start> e)
        {
            ///<summary>
            ///该语句是PDS原代码中没有的，所以导致了PDS中的数据生成速度是1s。
            ///补充该句才能以文件中的interval来生成数据。
            /// </summary>
            e.Message.MaxMessageRate = Model.Interval;
            //构造TaskRunner
            TaskRunner = new PDS.WITSMLstudio.Framework.TaskRunner(e.Message.MaxMessageRate) {
             OnError = LogStreamingError,
             OnExecute = StreamChannelData
            };
            if (Client.Handler<IChannelStreamingProducer>().IsSimpleStreamer)
            {
                //channelMetadata就是json文件各个道的描述信息
                List<ChannelMetadataRecord> channelMetadata = GetChannelMetadata(e.Header);

                //将channelMetadata设置到流的处理器中
                Client.Handler<IChannelStreamingProducer>().
                    ChannelMetadata(e.Header, channelMetadata);
                //channelmetadata存到channelstreaminginfo里面，channelstreaminginfo内容更少
                foreach (var channel in channelMetadata.Select(ToChannelStreamingInfo))
                {
                    ChannelStreamingInfo.Add(channel);
                }
                //start一开，StreamChannelData就来，
                TaskRunner.Start();
            }

        }

        /// <summary>
        /// 关键函数，传输数据到服务器
        /// </summary>
        private void StreamChannelData()
        {
            if (!Client.IsOpen) return;

            ///<remarks>
            ///构造数据项，该数据项包含了数据值、
            /// </remarks>
            var dataItems = ChannelStreamingInfo
                .Select(ToChannelDataItem)
                .ToList();

            ///<remarks>
            ///核心关键句
            /// </remarks>
            Client.Handler<IChannelStreamingProducer>()
                .ChannelData(null, dataItems);
        }

        /// <summary>
        /// 生成数据项，包含索引值（深度、时间）以及具体数值。
        /// </summary>
        /// <param name="streamingInfo"></param>
        /// <returns></returns>
        private DataItem ToChannelDataItem(ChannelStreamingInfo streamingInfo)
        {
            var channel = Channels.FirstOrDefault(x => x.ChannelId == streamingInfo.ChannelId);
            if (channel == null) return null;

            var indexDateTimeOffset = DateTimeOffset.UtcNow;

            ///<summary>
            ///两个关键函数：toChannelIndexValue(为什么总是返回零）toChannelDataValue返回模拟值；
            /// </summary>
            return new DataItem()
            {
                ChannelId = channel.ChannelId,
                Indexes = channel.Indexes
                .Select(x => ToChannelIndexValue(streamingInfo, x, indexDateTimeOffset))
                .ToList(),
                ValueAttributes = new DataAttribute[0],
                Value = new DataValue()
                {
                    Item = ToChannelDataValue(channel, indexDateTimeOffset)
                }
            };
        }

        /// <summary>
        /// 关键函数，生成随机数据对应的深度或者时间值
        /// </summary>
        /// <param name="streamingInfo"></param>
        /// <param name="index"></param>
        /// <param name="indexDateTimeOffset"></param>
        /// <returns></returns>
        private long ToChannelIndexValue(ChannelStreamingInfo streamingInfo, IndexMetadataRecord index, DateTimeOffset indexDateTimeOffset)
        {
            if (index.IndexType == ChannelIndexTypes.Time)
                return indexDateTimeOffset.ToUnixTimeMicroseconds();

            var value = 0d;

            if (streamingInfo.StartIndex.Item is double)
            {
                ///<summary>
                ///关键语句：生成深度值
                /// </summary>
                value = (double)streamingInfo.StartIndex.Item
                      + Math.Pow(10, index.Scale) * 0.1;
            }

            streamingInfo.StartIndex.Item = value;

            return (long)value;
        }

        /// <summary>
        /// 关键函数，生成随机数据的值
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="indexDateTimeOffset"></param>
        /// <returns></returns>
        private object ToChannelDataValue(ChannelMetadataRecord channel, DateTimeOffset indexDateTimeOffset)
        {
            object dataValue = null;
            var indexType = channel.Indexes.Select(i => i.IndexType).FirstOrDefault();

            LogDataType logDataType;
            var logDataTypeExists = Enum.TryParse<LogDataType>(channel.DataType, out logDataType);

            switch (logDataType)
            {
                case LogDataType.@byte:
                    {
                        dataValue = "Y";
                        break;
                    }
                case LogDataType.datetime:
                    {
                        var dto = indexType == ChannelIndexTypes.Time
                            ? indexDateTimeOffset
                            : indexDateTimeOffset.AddSeconds(_random.Next(1, 5));

                        dataValue = dto.ToString("o");
                        break;
                    }
                case LogDataType.@double:
                case LogDataType.@float:
                    {
                        double tmpvalue = _random.NextDouble() * 100;
                        dataValue = tmpvalue.ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                case LogDataType.@int:
                case LogDataType.@long:
                case LogDataType.@short:
                    {
                        dataValue = _random.Next(11);
                        break;
                    }
                case LogDataType.@string:
                    {
                        dataValue = "wang xiaotian";
                        break;
                    }
                default:
                    {
                        dataValue = "null";
                    }
                    break;
            }

            return dataValue;
        }


        private ChannelStreamingInfo ToChannelStreamingInfo(ChannelMetadataRecord record)
        {
            return new ChannelStreamingInfo()
            {
                ChannelId = record.ChannelId,
                ReceiveChangeNotification = false,
                StartIndex = new StreamingStartIndex()
                {
                    // "null" indicates a request for the latest value
                    Item = null
                }
            };
        }
        
        //ChannelMetadataRecord就是每一道的描述
        private List<ChannelMetadataRecord> GetChannelMetadata(MessageHeader header)
        {
            var indexMetadata = ToIndexMetadataRecord(Model.Channels.First());

            // Skip index channel
            var channelMetadata = Model.Channels
                .Skip(1)
                .Select(x => ToChannelMetadataRecord(x, indexMetadata))
                .ToList();

            return channelMetadata;
        }

        protected virtual EtpUri GetChannelUri(string mnemonic)
        {
            if (OptionsIn.DataVersion.Version131.Equals(DataSchemaVersion))
            {
                return EtpUris.Witsml131
                    .Append(ObjectTypes.Well, Model.WellUid)
                    .Append(ObjectTypes.Wellbore, Model.WellboreUid)
                    .Append(ObjectTypes.Log, Model.LogUid)
                    .Append(ObjectTypes.LogCurveInfo, mnemonic);
            }

            if (OptionsIn.DataVersion.Version141.Equals(DataSchemaVersion))
            {
                return EtpUris.Witsml141
                    .Append(ObjectTypes.Well, Model.WellUid)
                    .Append(ObjectTypes.Wellbore, Model.WellboreUid)
                    .Append(ObjectTypes.Log, Model.LogUid)
                    .Append(ObjectTypes.LogCurveInfo, mnemonic);
            }

            if (OptionsIn.DataVersion.Version200.Equals(DataSchemaVersion))
            {
                return EtpUris.Witsml200
                    .Append(ObjectTypes.Well, Model.WellUid)
                    .Append(ObjectTypes.Wellbore, Model.WellboreUid)
                    .Append(ObjectTypes.Log, Model.LogUid)
                    .Append(ObjectTypes.ChannelSet, Model.ChannelSetUid)
                    .Append(ObjectTypes.Channel, mnemonic);
            }

            return default(EtpUri);
        }

        /// <summary>
        /// 本函数好像没有被任何函数调用
        /// </summary>
        /// <param name="record"></param>
        /// <param name="indexMetadata"></param>
        /// <returns></returns>
        private ChannelMetadataRecord ToChannelMetadataRecord(ChannelMetadataRecord record, IndexMetadataRecord indexMetadata)
        {
            var uri = GetChannelUri(record.ChannelName);

            var channel = new ChannelMetadataRecord()
            {
                ChannelUri = uri,
                ContentType = uri.ContentType,
                ChannelId = record.ChannelId,
                ChannelName = record.ChannelName,
                Uom = record.Uom,
                MeasureClass = record.MeasureClass,
                DataType = record.DataType,
                Description = record.Description,
                Uuid = record.Uuid,
                Status = record.Status,
                Source = record.Source,
                Indexes = new[]
                {
                    indexMetadata
                },
                CustomData = new Dictionary<string, DataValue>()
            };

            Channels.Add(channel);
            return channel;
        }
    
        /// <summary>
        /// 本函数好像没有被任何函数调用
        /// </summary>
        /// <param name="record"></param>
        /// <param name="indexMetadata"></param>
        /// <returns></returns>
        private IndexMetadataRecord ToIndexMetadataRecord(ChannelMetadataRecord record, int scale = 3)
        {
            return new IndexMetadataRecord()
            {
                Uri = GetChannelUri(record.ChannelName),
                Mnemonic = record.ChannelName,
                Description = record.Description,
                Uom = record.Uom,
                Scale = scale,
                IndexType = Model.LogIndexType == LogIndexType.datetime || Model.LogIndexType == LogIndexType.elapsedtime
                    ? ChannelIndexTypes.Time
                    : ChannelIndexTypes.Depth,
                Direction = IndexDirections.Increasing,
                CustomData = new Dictionary<string, DataValue>(0),
            };
        }
               
        /// <summary>
        /// 临时工函数
        /// </summary>
        /// <param name="ex"></param>
        private void LogStreamingError(Exception ex)
        {
            if (ex is TaskCanceledException)
            {
                Log(ex.Message);
            }
            else
            {
                Log("An error occurred: " + ex);
            }
        }
        
        private void OnClientSocketClosed(object sender, EventArgs e)
        {
            TaskRunner.Stop();
        }

        private void OnChannelStreamingStop(object sender, ProtocolEventArgs<ChannelStreamingStop> e)
        {
            TaskRunner.Stop();
        }

        private void OnChannelStreamingStart(object sender, ProtocolEventArgs<ChannelStreamingStart> e)
        {
            e.Message.Channels.ForEach(ChannelStreamingInfo.Add);
            TaskRunner.Start();
        }

        private void OnChannelDescribe(object sender, ProtocolEventArgs<ChannelDescribe, IList<ChannelMetadataRecord>> e)
        {
            GetChannelMetadata(e.Header).ForEach(e.Context.Add);
        }

    }
}
