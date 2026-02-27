using UnityEngine;

[System.Serializable]
public class LeaderboardEntry : MonoBehaviour
{
    public string playerName;
    public GameManager.Cars carType;
    public System.TimeSpan time;

    public LeaderboardEntry(string name, GameManager.Cars car, System.TimeSpan timeSpan)
    {
        playerName = name;
        carType = car;
        time = timeSpan;
    }
}
