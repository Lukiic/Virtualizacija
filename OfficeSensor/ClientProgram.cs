using Common;
using System;
using System.Configuration;
using System.ServiceModel;

namespace OfficeSensor
{
    public class ClientProgram
    {
        static void Main(string[] args)
        {
            ChannelFactory<ISensorService> factory = new ChannelFactory<ISensorService>("SensorService");
            ISensorService proxy = factory.CreateChannel();
            FileReader fileReader = new FileReader(ConfigurationManager.AppSettings["csvFile"]);

            StartSessionWithServer(proxy, fileReader);

            for (int i = 0; i < 100; ++i)
                PushSampleToServer(proxy, fileReader);

            proxy.EndSession();
        }

        private static void StartSessionWithServer(ISensorService proxy, FileReader fileReader)
        {
            (bool isSuccessful, SensorSample sensorSample) = fileReader.GetSensorSample();

            if (!isSuccessful)
            {
                Console.WriteLine("No data for reading.");
                return;
            }

            ServerResponse serverResponse = proxy.StartSession(sensorSample);   // As meta data

            if (serverResponse.ResponseStatus == ResponseStatus.ACK && serverResponse.SessionStatus == SessionStatus.IN_PROGRESS)
                Console.WriteLine("Session with server successfully started");
        }

        private static void PushSampleToServer(ISensorService proxy, FileReader fileReader)
        {
            (bool isSuccessful, SensorSample sensorSample) = fileReader.GetSensorSample();

            if (!isSuccessful)
            {
                Console.WriteLine("No data for reading.");
                return;
            }

            ServerResponse serverResponse = proxy.PushSample(sensorSample);

            if (serverResponse.ResponseStatus == ResponseStatus.NACK)
                Console.WriteLine("Server did not acknowledge pushed sample.");
        }
    }
}
