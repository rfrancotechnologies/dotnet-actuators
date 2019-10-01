using System;

namespace Com.Rfranco.HttpActuator.Health
{
    public class ExceptionReport
    {
        public Exception OriginalException {get;}
        public DateTimeOffset WhenHappend {get;}

        public ExceptionReport(Exception originalException)
        {
            this.OriginalException = originalException;
            this.WhenHappend = DateTimeOffset.UtcNow;
        }

        public override string ToString() 
        {
            return $"{{\"date\" : \"{WhenHappend.ToString()}\",  \"type\": \"{OriginalException.GetType()}\", \"description\" : \"{OriginalException.Message}\"}}";
        }
    }

}