using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {

	public List<Player> players = new List<Player>();
	public int playerCount;
	public Player currentTurn;
	private int turnPointer;

	public int turn;
	public int nextTurnN;

	public bool seqOrSim;
	//false is sequential, true is simultaneous

	public ResourceManager manager;
	public Map world;

	public bool gameStarted;
	
	private void Awake() {
		manager = GetComponent<ResourceManager>();
		world = GetComponent<Map>();
		turn = 1;
		gameStarted = false;
	}

	private void Update() {
		if (world.mapGenerated && !gameStarted) StartGame();

		if (seqOrSim) {
			if (nextTurnN == players.Count) NextTurn();
		}
		else {
			if (currentTurn.nextTurn) NextTurn();
		}
	}

	private void StartGame() {
		System.Random rando = new System.Random(world.seed.GetHashCode());
		foreach (Player p in players) {
			bool allowed = false;

			p.isTurn = false;
			

			int posX = 0;
			int posY = 0;

			while (!allowed) {
				posX = rando.Next(1,world.worldSizeX);
				posY = rando.Next(1, world.worldSizeY);

				if (world.grid[posX, posY] == 2) {
					allowed = true;
					GameObject g = Instantiate(manager.cities[0], world.FindWorldPos(posX, posY), transform.rotation) as GameObject;
					City c = g.GetComponent<City>();
					p.capital = c;
					c.owner = p;
					p.cities.Add(p.capital);
					c.InitializeCity();
				}
			}

			if (p.isPlayer) p.ui_turnCounter_text.text = turn.ToString();
		}

		currentTurn = players[turnPointer];
		currentTurn.isTurn = true;
		currentTurn.StartTurn();
		turnPointer += 1;

		gameStarted = true;
	}

	private void NextTurn() {
		if (seqOrSim) {
			foreach (Player p in players) {
				p.StartTurn();
			}
		}
		else {
			currentTurn.isTurn = false;
			currentTurn.nextTurn = false;

			if (turnPointer < players.Count) { currentTurn = players[turnPointer]; turnPointer += 1; }
			else {
				turnPointer = 0;
				currentTurn = players[turnPointer];
				turnPointer += 1;
				turn += 1;

				foreach (Player p in players) {
					if (p.isPlayer) p.ui_turnCounter_text.text = turn.ToString();
				}
			}

			currentTurn.StartTurn();
			currentTurn.isTurn = true;
		}
	}
}