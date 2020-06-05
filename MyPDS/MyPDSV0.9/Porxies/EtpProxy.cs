using Energistics;
using PDS.WITSMLstudio.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyPDSV0._9.Porxies
{
    public abstract class EtpProxy
    {
        public EtpProxy(/*IRuntimeService runtime,*/ string dataSchemaVersion, Action<string> log)
        {
            TaskRunner = new TaskRunner();
            //Runtime = runtime;
            DataSchemaVersion = dataSchemaVersion;
            Log = log;
        }

        //public IRuntimeService Runtime { get; private set; }

        public string DataSchemaVersion { get; private set; }

        public Action<string> Log { get; private set; }

        public Models.Simulation Model { get; protected set; }

        public EtpClient Client { get; protected set; }

        public TaskRunner TaskRunner { get; protected set; }

        public abstract Task Start(Models.Simulation model, CancellationToken token, int interval = 5000);
    }
}
