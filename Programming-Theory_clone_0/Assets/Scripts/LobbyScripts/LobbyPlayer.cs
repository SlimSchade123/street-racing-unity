using PurrNet;
using TMPro;
using UnityEngine;

public class LobbyPlayer : NetworkIdentity
{
    public string playerName = "TestPlayer";
    public CarController selectedCar;
    void Start()
    {
        transform.Find("PlayerName").GetComponent<TMP_Text>().text = playerName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
