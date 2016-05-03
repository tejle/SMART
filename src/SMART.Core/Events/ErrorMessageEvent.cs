namespace SMART.Core.Events
{
    public class ErrorMessageEvent :SmartEvent<ErrorMessage>{}

    public class ErrorMessage
    {
        private string message;

        public ErrorMessage(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get { return message; }
        }
    }
}