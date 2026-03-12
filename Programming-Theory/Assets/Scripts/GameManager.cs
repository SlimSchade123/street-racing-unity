using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public enum Cars { Convertible, Pickup, Jumpy, SharkTruck }

	[System.Serializable]
	public class HighscoreData {
		public System.TimeSpan convertibleScore;
		public System.TimeSpan pickupScore;
		public System.TimeSpan jumpyScore;
		public System.TimeSpan sharkScore;
	}

	public static GameManager Instance { get; private set; } // ENCAPSULATION
	public static bool IsGameOver { get; set; } // ENCAPSULATION
	public static bool IsGameStarted { get; set; } // ENCAPSULATION

	public Cars choosenCarType;
	public GameObject player;
	public HighscoreData highscoreData = new HighscoreData();

	public CarController[] allCars = null;
	public ConvertibleController convertible = null;
	public PickupController pickup = null;
	public JumpyController jumpy = null;
	public SharkController shark = null;

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
    //New Leaderboard system
    public void SaveLeaderboardEntry(System.TimeSpan score)
    {
        string playerName = "Player";
        LeaderboardManager.Instance.AddEntry(playerName, choosenCarType, score);
    }

<<<<<<< Updated upstream
    private void Start()
    {
		allCars = FindObjectsByType<CarController>(FindObjectsSortMode.None);
		convertible = FindFirstObjectByType<ConvertibleController>();
		pickup = FindFirstObjectByType<PickupController>();
		jumpy = FindFirstObjectByType<JumpyController>();
		shark = FindFirstObjectByType<SharkController>();
    }

=======
>>>>>>> Stashed changes

    public bool IsSelectedVehicle(Cars cRef) { // ABSTRACTION
		if (cRef == choosenCarType) return true;
		else return false;
	}

	private bool IsScoreLowerThan(System.TimeSpan score1, System.TimeSpan score2) { // ABSTRACTION
		if (score1 < score2)
			return true;
		else
			return false;
	}

	public void SaveHighscore(System.TimeSpan score, Cars cRef) { // ABSTRACTION
		HighscoreData data = highscoreData;

		switch (cRef) {
			case Cars.Convertible:
				if (highscoreData.convertibleScore == new System.TimeSpan() ||
					IsScoreLowerThan(score, highscoreData.convertibleScore)
				)
					data.convertibleScore = score;
				break;
			case Cars.Pickup:
				if (highscoreData.pickupScore == new System.TimeSpan() ||
					IsScoreLowerThan(score, highscoreData.pickupScore)
				)
					data.pickupScore = score;
				break;
			case Cars.Jumpy:
				if (highscoreData.jumpyScore == new System.TimeSpan() ||
					IsScoreLowerThan(score, highscoreData.jumpyScore)
				)
					data.jumpyScore = score;
				break;
			case Cars.SharkTruck:
				if (highscoreData.sharkScore == new System.TimeSpan() ||
					IsScoreLowerThan(score, highscoreData.sharkScore)
				)
					data.sharkScore = score;
				break;
		}

		highscoreData = data;
	}

    //New Leaderboard system
	public void SaveLeaderboardEntry(System.TimeSpan score)
	{
		string playerName = "Player";
		LeaderboardManager.Instance.AddEntry(playerName, choosenCarType, score);
    }
	public int CalculatePlacement()
	{
		if(allCars == null || allCars.Length == 0) allCars = FindObjectsByType<CarController>(FindObjectsSortMode.None);

        foreach (CarController car in allCars)
		{
			car.CalculatePlacement(allCars);
		}

		int placement = 0;

		switch (choosenCarType)
		{
			case Cars.Convertible:
				if(convertible == null) convertible = FindFirstObjectByType<ConvertibleController>();

                placement = convertible._currentPlacement;
				break;
			case Cars.Pickup:
                if(pickup == null) pickup = FindFirstObjectByType<PickupController>();

				placement = pickup._currentPlacement;
                break;
			case Cars.Jumpy:
				if(jumpy == null)
                jumpy = FindFirstObjectByType<JumpyController>();

				placement = jumpy._currentPlacement;
				break;
			case Cars.SharkTruck:
				if(shark == null) shark = FindFirstObjectByType<SharkController>();
				placement = shark._currentPlacement;
				break;
			default:
				break;
		}

        return placement;
	}
}
