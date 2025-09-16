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
            FileWriter fileWriter = new FileWriter(ConfigurationManager.AppSettings["logFile"]);

            #region EventPublishersForLogging
            service.OnTransferStarted += (s, e) =>
            {
                fileWriter.WriteText($"{DateTime.Now}: {e.EventMessage}");
                Console.WriteLine(e.EventMessage);
            };

            service.OnSampleReceived += (s, e) =>
            {
                fileWriter.WriteText($"{DateTime.Now}: {e.EventMessage}");
                Console.WriteLine(e.EventMessage);
            };

            service.OnWarningRaised += (s, e) =>
            {
                fileWriter.WriteText($"{DateTime.Now}: {e.EventMessage}");

                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.EventMessage);
                Console.ForegroundColor = defaultColor;
            };

            service.OnTransferCompleted += (s, e) =>
            {
                fileWriter.WriteText($"{DateTime.Now}: {e.EventMessage}");
                Console.WriteLine(e.EventMessage);
            };

            service.VolumeSpike += (s, e) =>
            {
                string warningMessage = $"Volume warning (comparing with last value): {e.EventMessage}";
                fileWriter.WriteText($"{DateTime.Now}: {warningMessage}");

                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(warningMessage);
                Console.ForegroundColor = defaultColor;
            };

            service.OutOfBandWarning += (s, e) =>
            {
                string warningMessage = $"Volume warning (comparing with mean value): {e.EventMessage}";
                fileWriter.WriteText($"{DateTime.Now}: {warningMessage}");

                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(warningMessage);
                Console.ForegroundColor = defaultColor;
            };

            service.TemperatureSpikeDHT += (s, e) =>
            {
                string warningMessage = $"TemperatureDHT warning (comparing with last value): {e.EventMessage}";
                fileWriter.WriteText($"{DateTime.Now}: {warningMessage}");

                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(warningMessage);
                Console.ForegroundColor = defaultColor;
            };

            service.TemperatureSpikeBMP += (s, e) =>
            {
                string warningMessage = $"TemperatureBMP warning (comparing with last value): {e.EventMessage}";
                fileWriter.WriteText($"{DateTime.Now}: {warningMessage}");

                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(warningMessage);
                Console.ForegroundColor = defaultColor;
            };
            #endregion

            ServiceHost host = new ServiceHost(service);
            host.Open();

            Console.WriteLine("Service is active.");
            Console.ReadKey();

            host.Close();
            fileWriter.Dispose();
            Console.WriteLine("Service is closed");
        }
    }
}
