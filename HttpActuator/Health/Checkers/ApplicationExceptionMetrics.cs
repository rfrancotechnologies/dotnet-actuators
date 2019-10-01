using System;
using System.Collections.Generic;

namespace Com.Rfranco.HttpActuator.Health
{
    public static class ApplicationExceptionMetrics
    {
        public static List<ExceptionReport> ExceptionsReported { get; } = new List<ExceptionReport>();

        public static long NumberOfExceptionsThrown
        {
            get { return ExceptionsReported.Count; }
        }

        public static void Add(Exception e)
        {
            ExceptionsReported.Add(new ExceptionReport(e));
            ExceptionsReported.RemoveAll(ex => ex.WhenHappend < DateTimeOffset.UtcNow.AddDays(-1));
        }
    }
}