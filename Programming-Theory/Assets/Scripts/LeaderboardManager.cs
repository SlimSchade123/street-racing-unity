using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }
    [SerializeField] private int maxEntries = 10;
    public List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    public void AddEntry(string playerName, GameManager.Cars carType, System.TimeSpan time)
    {
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, carType, time);
        leaderboardEntries.Add(newEntry);
        
        leaderboardEntries = leaderboardEntries.OrderBy(entry => entry.time).Take(maxEntries).ToList();
    }


    public List <LeaderboardEntry> GetLeaderboard()
    {
        return leaderboardEntries;
    }

}
