using UnityEngine;

public static class PlayerSession
{
    public static string ClientId
    {
        get => PlayerPrefs.GetString("clientId", "");
        set => PlayerPrefs.SetString("clientId", value);
    }
}