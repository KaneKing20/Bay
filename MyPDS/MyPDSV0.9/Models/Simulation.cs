using PDS.WITSMLstudio.Desktop.Core.Connections;
using Energistics.DataAccess.WITSML141.ReferenceData;
using Energistics.Datatypes.ChannelData;
using Energistics.Protocol.ChannelDataFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyPDSV0._9.Models
{
    [DataContract]
    public class Simulation
    {
        public Simulation()
        {
            //Channels = new BindableCollection<ChannelMetadataRecord>();
            WitsmlConnection = new Connection();
            EtpConnection = new Connection();
            LogIndexType = LogIndexType.measureddepth;
            IsSimpleStreamer = true;
            PortNumber = 9000;
            Interval = 5000;
        }
        [DataMember]
        public string Name { set; get; }

        [DataMember]
        public string Version { set; get; }

        [DataMember]
        public int Interval { set; get; }

        [DataMember]
        public Connection WitsmlConnection { set; get; }

        [DataMember]
        public Connection EtpConnection { set; get; }

        [DataMember]
        public int PortNumber { set; get; }

        [DataMember]
        public string WellName { set; get; }

        [DataMember]
        public string WellUid { set; get; }

        [DataMember]
        public string WellboreName { set; get; }

        [DataMember]
        public string WellboreUid { set; get; }

        [DataMember]
        public string LogName { set; get; }

        [DataMember]
        public string LogUid { set; get; }

        [DataMember]
        public LogIndexType LogIndexType { set; get; }

        [DataMember]
        public string ChannelSetName { set; get; }

        [DataMember]
        public string ChannelSetUid { set; get; }

        [DataMember]
        public List<ChannelMetadataRecord> Channels { set; get; }

        [DataMember]
        public string WitsmlVersion { set; get; }

        [DataMember]
        public string EtpVersion { set; get; }

        [DataMember]
        public bool IsSimpleStreamer { set; get; }
    }
}
