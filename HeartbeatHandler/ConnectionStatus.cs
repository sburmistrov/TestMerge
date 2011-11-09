using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace HeartbeatHandler
{
    [DataContract]
    public class ConnectionStatus
    {
        public ConnectionStatus(string Name, string status)
        {
            this.name = Name;
            this.status = status;
        }

        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string status { get; set; }
    }

    [DataContract]
    public class OverAllStatus
    {
        public OverAllStatus()
        {
            overallStatus = "success";
            databases = new List<ConnectionStatus>();
            mipServices = new List<ConnectionStatus>();
            linkServices = new List<ConnectionStatus>();
        }

        [DataMember]
        public string overallStatus { get; set; }

        [DataMember]
        public List<ConnectionStatus> databases, mipServices, linkServices;
    }
}