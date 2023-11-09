using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectionManager : MonoBehaviour {

	public City inspectedCity;

	public GameObject cityInfo;
	private Text info_cityName;
	private Text info_cityProduction;
	private Text info_cityFood;
	private Text info_cityWater;
	private Text info_cityResearch;
	private Text info_cityCulture;
	private Text info_currentProduction;
	private Text info_turnsLeft;

	private GameObject[] productionOptions;

	private ResourceManager resManager;

	private Player player;

	private void Start() {
		resManager = GameObject.Find("GameManager").GetComponent<ResourceManager>();

		info_cityName = cityInfo.transform.Find("city_info_CityName").GetComponent<Text>();
		info_cityProduction = cityInfo.transform.Find("city_info_Production").GetComponent<Text>();
		info_cityFood = cityInfo.transform.Find("city_info_Food").GetComponent<Text>();
		info_cityResearch = cityInfo.transform.Find("city_info_Research").GetComponent<Text>();
		info_cityCulture = cityInfo.transform.Find("city_info_Culture").GetComponent<Text>();

		player = GameObject.Find("Player").GetComponent<Player>();
	}

	public void UpdateInspector(City c) {
		if (productionOptions != null) {
			foreach (GameObject g in productionOptions) {
				Destroy(g);
			}
		}

		productionOptions = new GameObject[resManager.buildings.Length];

		inspectedCity = c;
		info_cityName.text = c.cityName;
		info_cityProduction.text = c.production.ToString();
		info_cityFood.text = c.food.ToString();
		info_cityResearch.text = c.research.ToString();
		info_cityCulture.text = c.culture.ToString();

		if (c.currentProduction != null) {
			info_currentProduction.text = c.currentProduction.objectName;
			info_turnsLeft.text = ((c.currentProduction.cost - c.productionPool) / c.production).ToString();
		}

		for (int i = 0; i < resManager.buildings.Length; i++) {
			if (resManager.buildings[i].GetComponent<WorldObject>().prerequisite == null || c.owner.researched.Contains(resManager.buildings[i].GetComponent<WorldObject>().prerequisite)) {
				productionOptions[i] = Instantiate(resManager.productionOption, transform);
				RectTransform rect = productionOptions[i].GetComponent<RectTransform>();
				rect.anchorMin = new Vector2(0, 0.81f - (0.04f * i));
				rect.anchorMax = new Vector2(1, 0.85f - (0.04f * i));
				ProductionOption p = productionOptions[i].GetComponent<ProductionOption>();
				p.UpdateOption(resManager.buildings[i], inspectedCity, GetComponent<InspectionManager>());
			}
		}
	}

	public void SetProduction(GameObject g) {
		inspectedCity.currentProduction = g.GetComponent<WorldObject>();
		inspectedCity.UpdateTrackerProd(inspectedCity.currentProduction);
		player.ui_cityInspector.SetActive(false);
	}
}