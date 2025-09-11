using System;
using System.ServiceModel;

namespace Service
{
    public class ServiceProgram
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(SensorService));
            host.Open();

            Console.WriteLine("Service is open, press any key to close it.");
            Console.ReadKey();

            host.Close();
            Console.WriteLine("Service is closed");
        }
    }
}
