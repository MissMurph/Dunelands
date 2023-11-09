using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionOption : MonoBehaviour {

	public GameObject option;

	private string optionName;
	private int cost;

	private Text nameText, costText;

	public InspectionManager manager;

	private void Awake() {
		nameText = transform.Find("Name").GetComponent<Text>();
		costText = transform.Find("Cost").GetComponent<Text>();

		GetComponent<Button>().onClick.AddListener(ClickListen);
	}

	public void UpdateOption(GameObject g, City c, InspectionManager i) {
		manager = i;
		option = g;
		optionName = g.GetComponent<WorldObject>().objectName;
		cost = g.GetComponent<WorldObject>().cost;

		nameText.text = optionName;
		costText.text = ((cost - c.productionPool) / c.production).ToString();
	}

	public void ClickListen () {
		manager.SetProduction(option);
	}
}