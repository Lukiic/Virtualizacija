using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface ISensorService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationException))]
        ServerResponse StartSession(SensorSample meta);

        [OperationContract]
        [FaultContract(typeof(ValidationException))]
        ServerResponse PushSample(SensorSample sensorSample);

        [OperationContract]
        ServerResponse EndSession();
    }
}
