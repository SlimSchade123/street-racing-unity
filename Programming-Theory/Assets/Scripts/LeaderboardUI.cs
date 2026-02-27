using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{

    [SerializeField] private TMP_Text positionText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text carText;
    [SerializeField] private TMP_Text timeText;

    public void Setup(int position, LeaderboardEntry entry)
    {
        positionText.text = position.ToString();
        nameText.text = entry.playerName;
        carText.text = entry.carType.ToString();
        timeText.text = entry.time.ToString(@"mm\:ss\.fff");
    }
}
