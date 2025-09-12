using Common;
using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.Threading;

namespace OfficeSensor
{
    public class ClientProgram
    {
        static void Main(string[] args)
        {
            ChannelFactory<ISensorService> factory = new ChannelFactory<ISensorService>("SensorService");
            ISensorService proxy = factory.CreateChannel();
            FileReader fileReader = new FileReader(ConfigurationManager.AppSettings["dataCsvFile"]);

            StartSessionWithServer(proxy, fileReader);

            for (int i = 0; i < 100; ++i)
            {
                Thread.Sleep(200);
                PushSampleToServer(proxy, fileReader);
            }

            EndSessionWithServer(proxy);

            LogInvalidLines(fileReader);

            fileReader.Dispose();
        }

        private static void StartSessionWithServer(ISensorService proxy, FileReader fileReader)
        {
            (bool isSuccessful, SensorSample sensorSample) = fileReader.GetSensorSample();

            if (!isSuccessful)
            {
                Console.WriteLine("No data for reading.");
                return;
            }

            try
            {
                ServerResponse serverResponse = proxy.StartSession(sensorSample);   // As meta data

                if (serverResponse.ResponseStatus == ResponseStatus.ACK && serverResponse.SessionStatus == SessionStatus.IN_PROGRESS)
                    Console.WriteLine("Session with server successfully started");
            }
            catch (FaultException<ValidationException> ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void PushSampleToServer(ISensorService proxy, FileReader fileReader)
        {
            (bool isSuccessful, SensorSample sensorSample) = fileReader.GetSensorSample();

            if (!isSuccessful)
            {
                Console.WriteLine("No data for reading.");
                return;
            }

            try
            {
                ServerResponse serverResponse = proxy.PushSample(sensorSample);

                if (serverResponse.ResponseStatus == ResponseStatus.NACK)
                    Console.WriteLine("Server did not acknowledge pushed sample.");
            }
            catch (FaultException<ValidationException> ex)
            {
                Console.WriteLine(ex.Detail);
            }
        }

        private static void EndSessionWithServer(ISensorService proxy)
        {
            ServerResponse serverResponse = proxy.EndSession();

            if (serverResponse.ResponseStatus == ResponseStatus.ACK && serverResponse.SessionStatus == SessionStatus.COMPLETED)
                Console.WriteLine("Session with server successfully ended");
        }

        private static void LogInvalidLines(FileReader fileReader)
        {
            string logFilePath = ConfigurationManager.AppSettings["logFile"];

            File.WriteAllLines(logFilePath, fileReader.GetAllInvalidLines());
            File.AppendAllText(logFilePath, fileReader.GetAllRemainingText());
        }
    }
}
