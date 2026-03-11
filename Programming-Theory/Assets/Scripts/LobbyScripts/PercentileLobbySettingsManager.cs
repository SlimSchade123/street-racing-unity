using UnityEngine;


public class PercentileLobbySettingsManager : LobbySettingsManager
{
    public bool isInteger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override string ToString()
    {
        if (!isInteger)
        {
            return $"{(currentValue * 100)}%";
        }
        else
        {
            return $"{currentValue}";
        }
    }
}
