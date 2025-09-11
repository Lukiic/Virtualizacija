using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public enum ResponseStatus
    {
        ACK,
        NACK
    }

    [DataContract]
    public enum SessionStatus
    {
        IN_PROGRESS,
        COMPLETED
    }

    [DataContract]
    public class ServerResponse
    {
        [DataMember] public ResponseStatus ResponseStatus { get; set; }
        [DataMember] public SessionStatus SessionStatus { get; set; }
    }
}
