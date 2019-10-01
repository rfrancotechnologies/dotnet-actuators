namespace Com.Rfranco.HttpActuator.Health
{
    public class ApplicationHealthCheckerOption
    {
        public ApplicationHealthCheckerOption()
        {
            // By default 10 errors per minute or less is considered healthy. Upper values, unhealthy
            MaxNumberOfExceptionSupported = 10;
            IntervaleOfActitivySeconds = 60;
        }
        
        public int MaxNumberOfExceptionSupported { get; set; }
        public int IntervaleOfActitivySeconds { get; set; }
    }
}