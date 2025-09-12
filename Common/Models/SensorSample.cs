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

        public SensorSample(double volume, double temperatureDHT, double temperatureBMP, double pressure, DateTime dateTime)
        {
            Volume = volume;
            TemperatureDHT = temperatureDHT;
            TemperatureBMP = temperatureBMP;
            Pressure = pressure;
            DateTime = dateTime;
        }

        [DataMember] public double Volume { get; set; }
        [DataMember] public double TemperatureDHT { get; set; }
        [DataMember] public double TemperatureBMP { get; set; }
        [DataMember] public double Pressure { get; set; }
        [DataMember] public DateTime DateTime { get; set; }

        public override string ToString()
        {
            return $"{Volume},{TemperatureDHT},{TemperatureBMP},{Pressure},{DateTime}";
        }
    }
}
