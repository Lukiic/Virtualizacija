using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface ISensorService
    {
        [OperationContract]
        ServerResponse StartSession(params string[] meta);

        [OperationContract]
        [FaultContract(typeof(ValidationException))]
        ServerResponse PushSample(SensorSample sensorSample);

        [OperationContract]
        ServerResponse EndSession();
    }
}
