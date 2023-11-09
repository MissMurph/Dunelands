using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

	public GameObject[] cities;
	public GameObject[] buildings;
	public GameObject[] units;

	public Dictionary<string, Research> research = new Dictionary<string, Research>();

	public List<Research> startingResearch = new List<Research>();

	public GameObject cityTracker;
	public GameObject productionOption;

	public GameObject chunk;

	private void Start() {
		AddResearch();
	}

	public Vector2 FindGridPos(Vector3 position) {
		return new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
	}

	private void AddResearch() {
		research.Add("Hydroponics", new Research("Hydroponics", 65, 1, new List<string> { "Advanced Hydroponics" })); startingResearch.Add(research["Hydroponics"]);
		research.Add("Advanced Hydroponics", new Research("Advanced Hydroponics", 100, 1));
		research.Add("Assembly Lines", new Research("Assembly Lines", 70, 3, new List<string> { "Learning Assembly Lines" })); startingResearch.Add(research["Assembly Lines"]);
		research.Add("Learning Assembly Lines", new Research("Learning Assembly Lines", 115, 3));
		research.Add("Photon Scattering", new Research("Photon Scattering", 70, 0, new List<string> { "Photon Focusing" })); startingResearch.Add(research["Photon Scattering"]);
		research.Add("Photon Focusing", new Research("Photon Focusing", 110, 0));
	}
}