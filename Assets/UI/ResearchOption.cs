using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchOption : MonoBehaviour {

	public Research option;

	public Text text;

	public Player player;

	private GameObject researchWindow;

	private void Awake() {
		text = GetComponentInChildren<Text>();
		player = GameObject.Find("Player").GetComponent<Player>();
		researchWindow = GameObject.Find("ResearchWindow");
	}

	public void UpdateButton() {
		text.text = option.techName;
	}

	public void ButtonPress() {
		player.currentResearch = option;
		player.ui_currentResearch.GetComponentInChildren<Text>().text = option.techName;
		player.ui_chooseResearch.SetActive(false);
		player.currentResearch.progress += player.researchPool;
		researchWindow.SetActive(false);
	}
}