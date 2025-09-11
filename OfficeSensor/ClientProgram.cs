using Common;
using System.ServiceModel;

namespace OfficeSensor
{
    public class ClientProgram
    {
        static void Main(string[] args)
        {
            ChannelFactory<ISensorService> factory = new ChannelFactory<ISensorService>("SensorService");

            ISensorService proxy = factory.CreateChannel();
        }
    }
}
