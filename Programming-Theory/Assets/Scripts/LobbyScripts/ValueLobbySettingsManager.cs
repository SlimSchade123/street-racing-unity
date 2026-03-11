using UnityEngine;

public class ValueLobbySettingsManager : LobbySettingsManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string[] valueNames;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override string ToString()
    {
        return $"{valueNames[(int)currentValue]}";
    }
}
