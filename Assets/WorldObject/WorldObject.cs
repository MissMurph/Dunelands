using UnityEngine;
using System.Collections;

public class WorldObject : MonoBehaviour {

	public string objectName;
	public Sprite objectIcon;
	public int cost, hitPoints, maxHitPoints;

	public Player owner;

	public Research prerequisite;

	protected bool currentlySelected;

	protected virtual void Awake () {
		hitPoints = maxHitPoints;
	}

	protected virtual void Start () {

	}

	protected virtual void Update () {

	}

	protected virtual void Spawn () {

	}
}