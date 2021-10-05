using System;
using System.Diagnostics;

namespace VectorMaker
{
    public static class Debug
    {
        public static void Log(string text)
        {
            Trace.TraceInformation(DateTime.Now +" " + text);
        }

        public static void LogError(string text)
        {
            Trace.TraceError(DateTime.Now.ToString() + "   " + text);
        }

        public static void LogWarning(string text)
        {
            Trace.TraceWarning(DateTime.Now.ToString() + "   " + text);
        }
    }
}
