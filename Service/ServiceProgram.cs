using Service.Helpers;
using System;
using System.Configuration;
using System.ServiceModel;

namespace Service
{
    public class ServiceProgram
    {
        static void Main(string[] args)
        {
            SensorService service = new SensorService();
            EventPublishers.FileWriter = new FileWriter(ConfigurationManager.AppSettings["logFile"]);

            #region EventPublishersForLogging
            service.OnTransferStarted += EventPublishers.LogInformation;

            service.OnSampleReceived += EventPublishers.LogInformation;

            service.OnWarningRaised += EventPublishers.LogException;

            service.OnTransferCompleted += EventPublishers.LogInformation;

            service.VolumeSpike += EventPublishers.LogWarning;

            service.OutOfBandWarning += EventPublishers.LogWarning;

            service.TemperatureSpikeDHT += EventPublishers.LogWarning;

            service.TemperatureSpikeBMP += EventPublishers.LogWarning;
            #endregion

            ServiceHost host = new ServiceHost(service);
            host.Open();

            Console.WriteLine("Service is active.");
            Console.ReadKey();

            host.Close();
            EventPublishers.FileWriter.Dispose();
            Console.WriteLine("Service is closed");
        }
    }
}
