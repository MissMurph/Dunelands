using UnityEngine;
using System.Collections;

public class Building : WorldObject {

	public City city;

	public int upkeep;
	public int waterCost;

	public int foodAdd;
	public int waterAdd;
	public int productionAdd;
	public int researchAdd;
	public int goldAdd;
	public int cultureAdd;

	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();

		city.gold -= upkeep;

		city.food += foodAdd;
		city.production += productionAdd;
		city.research += researchAdd;
		city.gold += goldAdd;
		city.culture += cultureAdd;
	}

	protected override void Update() {
		base.Update();
	}

	protected override void Spawn() {
		base.Spawn();
	}
}