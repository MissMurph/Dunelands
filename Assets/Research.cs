using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Research {

	public string techName;
	public int baseCost;
	public int progress;

	public bool researched;

	public Sprite image;

	public int specialization;
	//0 = physics
	//1 = biolody
	//2 = engineering

	public List<WorldObject> unlocks = new List<WorldObject>();
	public List<string> nextTech = new List<string>();

	public Research (string n, int c, int s, List<string> u = null) {
		progress = 0;
		techName = n;
		baseCost = c;
		specialization = s;
		nextTech = u;
	}
}