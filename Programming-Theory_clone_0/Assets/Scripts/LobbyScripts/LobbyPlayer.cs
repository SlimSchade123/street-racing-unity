using PurrNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : NetworkIdentity
{
    public string playerName = "TestPlayer";
    public CarController selectedCar;
    public bool isReady = false;
    public Button readyButton;
    void Start()
    {
        transform.Find("PlayerName").GetComponent<TMP_Text>().text = playerName;
        readyButton = GetComponentInChildren<Button>();
        readyButton.onClick.AddListener(OnReadyUp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnReadyUp()
    {
        isReady = !isReady;
        readyButton.GetComponentInChildren<TMP_Text>().text = isReady ? "Ready" : "Not Ready";
    }
}
