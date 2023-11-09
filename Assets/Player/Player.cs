using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public List<City> cities = new List<City>();
	public City capital;

	public int faction;

	public GameObject gameController;

	public Map world;

	public bool isTurn;

	public bool nextTurn;

	public int researchPool;

	public bool isPlayer;

	public bool allowNextTurn;

	public Research currentResearch;
	private List<Research> availableTech = new List<Research>();
	public List<Research> researched = new List<Research>();

	public GameObject selected;

	//Game Manager Scripts
	private ResourceManager resManager;
	//private PlayerManager playerManager;

	//Next Turn Counter
	private GameObject ui_nextTurnButton;
	private GameObject ui_waitingOnPlayer;
	private GameObject ui_turnCounter;
		public Text ui_turnCounter_text;

	//Choose Research and Production Buttons
	public GameObject ui_chooseResearch;
	public GameObject ui_chooseProduction;

	//Research Window
	private GameObject ui_researchWindow;
	public GameObject ui_currentResearch;
		//private Text ui_currentResearch_text;
	public GameObject[] ui_researchOptionButtons;

	//City Inspector
	public GameObject ui_cityInspector;
	private InspectionManager inspectionManager;

	private void Awake() {
		world = gameController.GetComponent<Map>();
		resManager = gameController.GetComponent<ResourceManager>();
		//playerManager = gameController.GetComponent<PlayerManager>();
		ui_nextTurnButton = GameObject.Find("NextTurn");
		ui_waitingOnPlayer = GameObject.Find("WaitingOnPlayer");
		ui_turnCounter = GameObject.Find("turn_Counter");
		ui_turnCounter_text = ui_turnCounter.GetComponent<Text>();
		ui_researchWindow = GameObject.Find("ResearchWindow");
		ui_currentResearch = GameObject.Find("CurrentResearch").gameObject;
		//ui_currentResearch_text = ui_currentResearch.GetComponentInChildren<Text>();
		ui_chooseProduction = GameObject.Find("ChooseProduction");
		ui_chooseResearch = GameObject.Find("ChooseResearch");

		ui_cityInspector = GameObject.Find("CityInspector");
		inspectionManager = ui_cityInspector.GetComponent<InspectionManager>();
	}

	private void Start() {
		if (isPlayer) {
			if (!isTurn) ui_waitingOnPlayer.SetActive(true);
			else ui_nextTurnButton.SetActive(true);
			allowNextTurn = false;
			ui_researchWindow.SetActive(false);

			ui_chooseProduction.SetActive(true);
			ui_chooseResearch.SetActive(true);
		}

		availableTech = resManager.startingResearch;
		ui_researchWindow.SetActive(false);
		ui_cityInspector.SetActive(false);
    }

	private void Update() {
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			GameObject hitObject;
			if (Physics.Raycast(ray, out hit)) hitObject = hit.collider.gameObject;
			else return;
			if (hitObject.name != "ground") selected = hitObject;
		}
	}

	public void NextTurn() {
		nextTurn = true;

		if (isPlayer) {
			ui_nextTurnButton.SetActive(false);
			ui_waitingOnPlayer.SetActive(true);
		}
	}

	public void StartTurn() {
		foreach (City c in cities) {
			c.productionPool += c.production;
			researchPool += c.research;
		}

		if (currentResearch != null) {
			currentResearch.progress += researchPool;
		}

		if (isPlayer) {
			ui_nextTurnButton.SetActive(true);
			ui_waitingOnPlayer.SetActive(false);

			if (currentResearch != null && currentResearch.progress >= currentResearch.baseCost) {
				availableTech.Remove(currentResearch);
				foreach (string s in currentResearch.nextTech) {
					availableTech.Add(resManager.research[s]);
				}
				currentResearch = null;
				allowNextTurn = false;
				ui_chooseResearch.SetActive(true);
			}
		}
	}

	public void OpenResearchOptions () {
		ui_researchWindow.SetActive(true);

		for (int i = 0; i < availableTech.Count; i++) {
			if (!ui_researchOptionButtons[i].activeSelf) ui_researchOptionButtons[i].SetActive(true); ;
			ResearchOption r = ui_researchOptionButtons[i].GetComponent<ResearchOption>();
			r.option = availableTech[i];
			r.UpdateButton();
		}

		if (availableTech.Count < ui_researchOptionButtons.Length) {
			for (int i = availableTech.Count; i < ui_researchOptionButtons.Length; i++) {
				ui_researchOptionButtons[i].SetActive(false);
			}
		}
	}

	public void OpenCityInspector (City c) {
		ui_cityInspector.SetActive(true);
		inspectionManager.UpdateInspector(c);
	}
}