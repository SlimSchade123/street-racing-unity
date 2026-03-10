using UnityEngine;

public struct LobbySettings
{
    
    public int maxPlayers;
    public int playerCount;
    public int speedMultiplier;

    public LobbySettings(int maxPlayers = 4, int playerCount = 0, int speedMultiplier = 1)
    {
        this.maxPlayers = maxPlayers;
        this.playerCount = playerCount;
        this.speedMultiplier = speedMultiplier;
    }
}
