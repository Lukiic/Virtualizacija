using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public enum ResponseStatus
    {
        [EnumMember] ACK,
        [EnumMember] NACK
    }

    [DataContract]
    public enum SessionStatus
    {
        [EnumMember] IN_PROGRESS,
        [EnumMember] COMPLETED
    }

    [DataContract]
    public class ServerResponse
    {
        public ServerResponse(ResponseStatus responseStatus, SessionStatus sessionStatus)
        {
            ResponseStatus = responseStatus;
            SessionStatus = sessionStatus;
        }

        [DataMember] public ResponseStatus ResponseStatus { get; set; }
        [DataMember] public SessionStatus SessionStatus { get; set; }
    }
}
