
using System.Diagnostics;

public static class LogWrapper
{
    [Conditional("DEBUGLOG")]
    public static void Log(string message)
    {
        UnityEngine.Debug.Log(message);
    }
}