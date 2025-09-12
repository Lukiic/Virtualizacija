using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class ValidationException
    {
        public ValidationException(string field, string message)
        {
            Field = field;
            Message = message;
        }

        [DataMember]
        public string Field { get; set; }
        [DataMember]
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{Field} Validation failed: {Message}";
        }
    }
}
