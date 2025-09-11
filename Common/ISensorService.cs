using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface ISensorService
    {
        [OperationContract]
        ServerResponse StartSession(SensorSample meta);

        [OperationContract]
        ServerResponse PushSample(SensorSample sensorSample);

        [OperationContract]
        ServerResponse EndSession();
    }
}
