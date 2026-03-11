using PurrNet;
using PurrNet.Modules;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuController : MonoBehaviour {
	public Animator startAnimator;
	public Animator arrowAnimator;
	public Animator fadeOutAnimator;

	public GameObject selection1;
	public GameObject selection2;
	public GameObject selection3;
	public GameObject selection4;

	public GameObject border;

	private readonly Dictionary<GameManager.Cars, GameObject> _selectionsDic = new Dictionary<GameManager.Cars, GameObject>();

	private GameObject _highlightedSelection;
	private GameObject _chosenSelection;
	public TMP_Text chosenText;

	public float rotationSpeed = 60f;
    private CursorSyncManager CursorSync =>
    _cursorSyncManager ??= FindAnyObjectByType<CursorSyncManager>();
    private CursorSyncManager _cursorSyncManager;

    public Button startButton;

    private void Start() {
		GameManager.IsGameStarted = false;
		UIManager.SetCursorVisibility(true);

		_selectionsDic.Add(GameManager.Cars.Convertible, selection1);
		_selectionsDic.Add(GameManager.Cars.Pickup, selection2);
		_selectionsDic.Add(GameManager.Cars.Jumpy, selection3);
		_selectionsDic.Add(GameManager.Cars.SharkTruck, selection4);

		ChooseSelection(GameManager.Cars.Convertible);
        _cursorSyncManager = FindAnyObjectByType<CursorSyncManager>();

        if (startButton != null)
            startButton.interactable = false;
    }

	private void Update() {
		if (Input.anyKeyDown) {
			startAnimator.SetTrigger("OnClick");
			arrowAnimator.SetBool("HasClicked", true);
		}

		if (_highlightedSelection != null) {
			Transform highlightedCar = _highlightedSelection.GetComponent<MenuSelection>().car;
			highlightedCar.localRotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
		}
		else if (_chosenSelection != null) {
			MenuSelection menuSelection = _chosenSelection.GetComponent<MenuSelection>();
			menuSelection.car.localRotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
		}

		if (_chosenSelection != null) {
			MenuSelection menuSelection = _chosenSelection.GetComponent<MenuSelection>();
			border.GetComponent<Image>().color = menuSelection.difficultyColor;
			border.transform.SetParent(_chosenSelection.transform);
			border.transform.SetAsFirstSibling();
			border.transform.position = menuSelection.borderReferencePosition.position;
		}
	}

    public void ChooseSelection(GameManager.Cars cRef)
    {
        _chosenSelection = _selectionsDic[cRef];
        MenuSelection menuSelection = _selectionsDic[cRef].GetComponent<MenuSelection>();
        chosenText.text = menuSelection.car.name;
        chosenText.color = menuSelection.difficultyColor;
        GameManager.Instance.choosenCarType = cRef;

        CursorSync?.LocalPlayerSelectedCar(cRef);
    }

    public void HighlightSelection(GameManager.Cars cRef)
    {
        _highlightedSelection = _selectionsDic[cRef];
        CursorSync?.OnCarHoverEnter(cRef); // Show blocked tint if taken
    }

    public void RemoveHighlightedSelection(GameManager.Cars cRef)
    {
        _highlightedSelection = null;
        CursorSync?.OnCarHoverExit(cRef); // Restore taken overlay
    }

    // Called by CursorSyncManager when all players have picked
    public void SetStartButtonReady(bool ready)
    {
        if (startButton != null)
            startButton.interactable = ready;
    }

    // Helper for CursorSyncManager to access selection objects
    public GameObject GetSelectionObject(GameManager.Cars car)
    {
        return _selectionsDic.TryGetValue(car, out GameObject obj) ? obj : null;
    }


    public void Play() { // ABSTRACTION
		if (GameManager.Instance == null)
			return;

        // Only the server should trigger the scene load
        if (!NetworkManager.main.isServer) return;

        GameManager.IsGameOver = false;

		StartCoroutine(ChangeScene());
	}

	private IEnumerator ChangeScene() { // ABSTRACTION
		fadeOutAnimator.SetTrigger("Fade");

        var settings = new PurrSceneSettings
        {
            isPublic = true,
            mode = LoadSceneMode.Additive  // Doesn't require DontDestroyOnLoad
        };

        yield return new WaitForSeconds(.75f);
        NetworkManager.main.sceneModule.LoadSceneAsync(1,settings);
    }
}
