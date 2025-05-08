using UnityEngine;

namespace Game.Runtime.Utils
{
    public static class LogUtil
    {
        public static void Log(string caller, string logString = null, Color textColor = default,
            LogType logType = LogType.Log, bool editorOnly = false)
        {
#if !UNITY_EDITOR
            if (editorOnly)
                return;
#endif

#if UNITY_EDITOR
            var color = ColorUtility.ToHtmlStringRGB(textColor == default ? Color.white : textColor);
            logString = $"<b><color=#{color}>[{caller}]</b>. {(string.IsNullOrEmpty(logString) ? string.Empty : logString)}</color>";
#else
            logString = $"[{caller}]. {(string.IsNullOrEmpty(logString) ? string.Empty : logString)}";
#endif

            Debug.unityLogger.Log(logType, logString);
        }

        public static void LogWarning(string caller, string logString = null, bool editorOnly = false)
        {
            Log(caller, logString, Color.yellow, LogType.Warning, editorOnly);
        }

        public static void LogError(string caller, string logString = null, bool editorOnly = false)
        {
            Log(caller, logString, textColor: Color.red, LogType.Error, editorOnly);
        }
    }
}