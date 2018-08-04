using SQLite;
using System;

namespace WOL.Model
{
    public class DeviceInfo
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string IpAddress { get; set; }

        public string MacAddress { get; set; }

        public string BroadcastAddress { get; set; }

        public int Port { get; set; }

        public int SendingCount { get; set; }

        public string NcInfo { get; set; }

        public int Type { get; set; }
    }
}
