using UnityEngine;

public enum SessionType
{
    None,
    Anonymous,
    Unity
}

public static class SessionData
{
    private const string KEY_SESSION_TYPE = "SESSION_TYPE";
    private const string KEY_PLAYER_NAME = "PLAYER_NAME";

    public static SessionType GetSessionType()
    {
        return (SessionType)PlayerPrefs.GetInt(KEY_SESSION_TYPE, (int)SessionType.None);
    }

    public static string GetPlayerName()
    {
        return PlayerPrefs.GetString(KEY_PLAYER_NAME, "");
    }

    public static void SetSession(SessionType type, string playerName)
    {
        PlayerPrefs.SetInt(KEY_SESSION_TYPE, (int)type);
        PlayerPrefs.SetString(KEY_PLAYER_NAME, playerName ?? "");
        PlayerPrefs.Save();
    }

    public static void ClearSession()
    {
        PlayerPrefs.DeleteKey(KEY_SESSION_TYPE);
        PlayerPrefs.DeleteKey(KEY_PLAYER_NAME);
        PlayerPrefs.Save();
    }
}
