using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class City : MonoBehaviour {

	public string cityName;

	public int population;
	public int happiness;
		public int unrest;
	public int production;
	public int research;
	public int food;
		public int foodStock;
	public int gold;
	public int culture;

	public int productionPool;

	public WorldObject currentProduction;

	public Player owner;

	private Map world;
	private ResourceManager resManager;

	private GameObject tracker;
	private RectTransform trackerTrans;
	private Text uiName, uiPop, uiProd;
	
	public void InitializeCity () {
		population = 1;
		resManager = owner.gameController.GetComponent<ResourceManager>();
		world = owner.gameController.GetComponent<Map>();
		switch (world.grid[Mathf.RoundToInt(resManager.FindGridPos(transform.position).x),Mathf.RoundToInt(resManager.FindGridPos(transform.position).y)]) {
			case 0:
				production = 3;
				food = 1;
				research = 2;
				gold = 2;
				culture = 3;
				break;
			case 2:
				production = 1;
				food = 3;
				research = 3;
				gold = 2;
				culture = 2;
				break;
			case 3:
				production = 2;
				food = 1;
				research = 4;
				gold = 2;
				culture = 1;
				break;
		}

		cityName = "banana";
		
		tracker = Instantiate(resManager.cityTracker);
		tracker.transform.SetParent(GameObject.Find("CityTrackers").transform);
		trackerTrans = tracker.GetComponent<RectTransform>();
			uiName = tracker.transform.Find("CityName").GetComponent<Text>();
			uiPop = tracker.transform.Find("CityPop").GetComponent<Text>();
			uiProd = tracker.transform.Find("CityProd").GetComponent<Text>();

		uiName.text = cityName;
		uiPop.text = population.ToString();
		if (currentProduction != null) uiProd.text = currentProduction.name;
		else uiProd.text = "No Production";

		Button button = tracker.GetComponent<Button>();
		button.onClick.AddListener(TrackerClick);

		owner.allowNextTurn = false;
	}

	private void Update() {
		Vector3 worldPos = transform.position;
		worldPos.z += 2;
		Vector2 pos = Camera.main.WorldToScreenPoint(worldPos);
		pos.y += trackerTrans.rect.height / 2;
		trackerTrans.position = pos;
		//trackerTrans.localScale = Vector3.one / (Camera.main.transform.position.y / 100);
		//if (trackerTrans.localScale.x > 1) trackerTrans.localScale = Vector3.one;
	}

	public void UpdateTrackerPop() {

	}

	public void UpdateTrackerProd(WorldObject w) {
		uiProd.text = w.objectName;
	}

	private void TrackerClick() {
		owner.OpenCityInspector(this.GetComponent<City>());
	}
}