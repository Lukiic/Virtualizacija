using System;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class SensorSample
    {
        public SensorSample()
        {
        }

        public SensorSample(double volume, double dHT, double bMP, double pressure, DateTime dateTime)
        {
            Volume = volume;
            T_DHT = dHT;
            T_BMP = bMP;
            Pressure = pressure;
            DateTime = dateTime;
        }

        [DataMember] public double Volume { get; set; }
        [DataMember] public double T_DHT { get; set; }
        [DataMember] public double T_BMP { get; set; }
        [DataMember] public double Pressure { get; set; }
        [DataMember] public DateTime DateTime { get; set; }
    }
}
