using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	[Range(0.4f,2f)]
	public float temperature;
		//this will change the y area of each biome, relying on a normal distribution method
		//Mean is worldSizeY / 2
		//Standard Deviation will be figured out in reverse
		//Desert will take up 68.2% (34.1%)
		//Ice will take up 4.6% (2.3%)
		//Flood will take up 27.2% (13.6%)
		//
		//SO I'm going to do between 0.4 and 1.9, the temperature adjusts the grasslands and ice, then minuses both from 1 to figure out desert size
		//For now just use temperature as reverse maths isn't my strong point

	public GameObject[] tiles;

	public string seed;

	public int worldSizeX, worldSizeY;

	public int[,] grid;
	//0 = dune sea
	//1 = mesa
	//2 = flood plains
	//3 = ice
	//5 = river

	public Square[,] squareGrid;

	public Chunk[,] chunkGrid;

	private int sDev1, sDev2;

	private readonly MeshFilter meshFilter;

	public ResourceManager resManager;

	public int riverCount;

	public int[,] gridWeights;
	public Vector3[,] centerPoints;

	public bool mapGenerated;

	private void Awake() {
		resManager = GetComponent<ResourceManager>();
	}

	private void Start () {
		CreateGrid();
	}

	private void CreateGrid() {
		mapGenerated = false;

		if (seed == null) {
			seed = Time.time.ToString();
		}

		grid = new int[worldSizeX, worldSizeY];
		gridWeights = new int[worldSizeX, worldSizeY];

		Mathf.Clamp(temperature, 0.4f, 2f);

		//Ice Biome
		float sDev1P = 0.023f * temperature;
		//Grassland Biome
		float sDev2P = 0.136f * temperature;

		sDev1 = Mathf.RoundToInt(worldSizeY * sDev1P);
		sDev2 = Mathf.RoundToInt(worldSizeY * sDev2P);

		System.Random rando = new System.Random(seed.GetHashCode());

		for (int x = 0; x < worldSizeX; x++) {
			for (int y = 0; y < worldSizeY; y++) {
				int biome = 0;

				if (y <= sDev1) biome = 3;
				else if (y <= sDev2 && y > sDev1) biome = 2;
				else if (y >= worldSizeY - sDev1) biome = 3;
				else if (y >= worldSizeY - sDev2 && y < worldSizeY - sDev1) biome = 2;

				grid[x, y] = biome;
			}
		}

		//Here we go along every x unit via for loop
		for (int x = 0; x < worldSizeX; x++) {
			//We convert the unit into a radian, treating each unit as a percentage level
			float piX = (x / 100f) * Mathf.PI;
			float maxDev = Mathf.Sin(piX);

			float piX2 = ((x / 100f) + 1) * Mathf.PI;
			float maxDev2 = Mathf.Sin(piX2);

			//This is the level of deviation that's allowed in total
			int grassTiles = Mathf.RoundToInt((sDev2 - sDev1) * 0.75f);

			//We set the min and max values, with max being the sin deviation multiplied by the tile deviation
			int min = sDev2; int max = sDev2 + (Mathf.RoundToInt(grassTiles * maxDev));
			int min2 = (worldSizeY - sDev2) - (Mathf.RoundToInt(grassTiles * maxDev2)); int max2 = worldSizeY - sDev2;

			//This is just to make sure things work for when sin goes negative
			if (min > max) {
				int temp = min;
				min = max;
				max = temp;
			}

			if (min2 > max2) {
				int temp = min2;
				min2 = max2;
				max2 = temp;
			}

			//Change the edge tiles
			int randomlyChosenTile = rando.Next(min + 1, max + 1);
			if (grid[x, randomlyChosenTile] != 2) grid[x, randomlyChosenTile] = 2;
			else grid[x, randomlyChosenTile] = 0;

			int randomlyChosenTile2 = rando.Next(min2 + 1, max2 + 1);
			if (grid[x, randomlyChosenTile2] != 2) grid[x, randomlyChosenTile2] = 2;
			else grid[x, randomlyChosenTile2] = 0;

			//Here we're just making sure every tile to the deviation level is changed
			if (randomlyChosenTile > sDev2) {
				for (int y = sDev1 + 1; y < randomlyChosenTile; y++) {
					grid[x, y] = 2;
				}
			}
			else {
				for (int y = randomlyChosenTile; y <= sDev2; y++) {
					grid[x, y] = 0;
				}
			}

			if (randomlyChosenTile2 > worldSizeY - sDev2) {
				for (int y = worldSizeY - sDev2; y < randomlyChosenTile2; y++) {
					grid[x, y] = 0;
				}
			}
			else {
				for (int y = randomlyChosenTile2; y <= worldSizeY - sDev2; y++) {
					grid[x, y] = 2;
				}
			}
		}

		//And here we run a smoothing algorithm in order to make it seem more natural
		for (int x = 0; x < worldSizeX; x++) {
			for (int y = 0; y < worldSizeY; y++) {
				if (FindNeighboursOfType(x, y, 2) > 3) {
					grid[x, y] = 2;
				}
				else if (FindNeighboursOfType(x,y,0) > 3) grid[x,y] = 0;
			}
		}

		/*int topRiverCount = rando.Next(riverCount/3,(riverCount/2)+1);

		for (int bot = 0; bot < riverCount - topRiverCount; bot++) {
			ConstructRiver(rando.Next(0, worldSizeX), false);
		}
		for (int top = 0; top < topRiverCount; top++) {
			ConstructRiver(rando.Next(0, worldSizeX), true);
		}*/

		centerPoints = new Vector3[worldSizeX + 1, worldSizeY + 1];

		for (int x = 0; x < worldSizeX; x++) {
			for (int y = 0; y < worldSizeY; y++) {
				gridWeights[x, y] = FindNodeWeight(x,y, grid[x,y]);
			}
		}

		/*for (int x = 0; x < worldSizeX; x++) {
			for (int y = 0; y < worldSizeY; y++) {
				float topWeight = 1; if (y < worldSizeY - 1) topWeight = 0.125f * (gridWeights[x, y] + (8 - gridWeights[x, y + 1]));
				float rightWeight = 1; if (x < worldSizeX - 1) rightWeight = 0.125f * (gridWeights[x, y] + (8 - gridWeights[x + 1, y]));
				float bottomWeight = 1; if (y > 0 + 1) bottomWeight = 0.125f * (gridWeights[x, y] + (8 - gridWeights[x, y - 1]));
				float leftWeight = 1; if (x > 0 + 1) leftWeight = 0.125f * (gridWeights[x, y] + (8 - gridWeights[x - 1, y]));

				centerPoints[x, y] = new Vector3(FindWorldPos(x,y).x, 0, FindWorldPos(x,y).y + 0.5f);
				if (x < worldSizeX) centerPoints[x + 1, y] = new Vector3(FindWorldPos(x, y).x + 0.5f, 0, FindWorldPos(x, y).y);
				if (y > 0) centerPoints[x, y - 1] = new Vector3(FindWorldPos(x, y).x, 0, FindWorldPos(x, y).y - 0.5f);
				if (x > 0) centerPoints[x - 1, y] = new Vector3(FindWorldPos(x, y).x - 0.5f, 0, FindWorldPos(x, y).y);
			}
		}*/

		CreateMapMeshes();
		mapGenerated = true;
	}

	private void CreateMapMeshes () {
		squareGrid = new Square[worldSizeX - 1, worldSizeY - 1];
		chunkGrid = new Chunk[worldSizeX / 10, worldSizeY / 10];

		for (int x = 0; x < worldSizeX - 1; x++) {
			for (int y = 0; y < worldSizeY - 1; y++) {
				squareGrid[x,y] = new Square(x,y, grid[x,y+1], grid[x+1,y+1], grid[x+1,y], grid[x,y]);
			}
		}

		for (int x = 0; x < (worldSizeX - 1) / 10; x++) {
			for (int y = 0; y < (worldSizeY - 1) / 10; y++) {
				chunkGrid[x, y] = new Chunk(x,y,squareGrid,this);
			}
		}

		
	}

	private void ConstructRiver (int x, bool botOrTop) {
		System.Random rando = new System.Random();
		//false = bottom
		//true = top
		int y = 0;
		if (botOrTop) y = worldSizeY - sDev1;
		else y = sDev1;

		//Set the starting node to ice
		grid[x, y] = 3;

		//Create the start integers
		int startX = x;
		int startY = y;

		//Get the deviation level (this ain't working idk why, maybe gotta do the deviations in their seperate loops)
		int dX = rando.Next(-9,9);
		int dY = rando.Next(10, 25);

		//Do y dev properly
		if (botOrTop) y -= dY;
		else y += dY;

		if (dX < 0) x -= dX;
		else x += dX;

		//Correct new x value, keep it in place
		if (x >= worldSizeX) x = worldSizeX-1;
		if (x <= 0) x = 1;

		int diffX = x - startX; if (diffX < 0) diffX *= -1;
		int diffY = y - startY; if (diffY < 0) diffX *= -1;

		int nodesToChange = diffX + diffY;
		float yRatio = diffY / diffX;
		float xRatio = diffX / diffY;

		int currentX = startX;
		int currentY = startY;

		/*while (nodesToChange > 0) {


			nodesToChange--;
		}*/

		for (int i = 0; i < diffX; i++) {
			if (yRatio > 1) {
				for (int ii = 0; ii < yRatio; ii++) {
					grid[startX + i, currentY + ii] = 3;
					currentY += ii;
				}
			}
			else if (yRatio == 1) {
				grid[startX + i, startY + i] = 3;
			}
			else if (yRatio < 1) {
				//currentY += yRatio;
			}
		}

		grid[x, y] = 3;

		dX = rando.Next(-9, 9);
		dY = rando.Next(10, 25);

		if (botOrTop) y -= dY;
		else y += dY;

		if (dX < 0) x -= dX;
		else x += dX;

		if (x >= worldSizeX) x = worldSizeX - 1;
		if (x <= 0) x = 1;

		grid[x, y] = 3;
	}

	private int FindNodeWeight(int x, int y, int type) {
		int neighbours = 0;

		if (x - 1 > 0 && y + 1 < worldSizeY && grid[x - 1, y + 1] == type) neighbours++;
		if (y + 1 < worldSizeY && grid[x, y + 1] == type) neighbours++;
		if (x + 1 < worldSizeX && y + 1 < worldSizeY && grid[x + 1, y + 1] == type) neighbours++;
		if (x + 1 < worldSizeX && grid[x + 1, y] == type) neighbours++;
		if (x + 1 < worldSizeX && y - 1 > 0 && grid[x + 1, y - 1] == type) neighbours++;
		if (y - 1 > 0 && grid[x, y - 1] == type) neighbours++;
		if (x - 1 > 0 && y - 1 > 0 && grid[x - 1, y - 1] == type) neighbours++;
		if (x - 1 > 0 && grid[x - 1, y] == type) neighbours++;

		return neighbours;
	}

	private int FindNeighboursOfType(int x, int y, int type) {
		int neighbours = 0;

		for (int sX = x - 1; sX <= x + 1; sX++) {
			for (int sY = y - 1; sY <= y + 1; sY++) {
				if (sX >= 0 && sX < worldSizeX && sY >= 0 && sY < worldSizeY) {
					if (sX != x && sY != y && grid[sX, sY] == type) neighbours++;
				}
			}
		}

		return neighbours;
	}

	public Vector3 FindWorldPos(int x, int y) {
		Vector3 pos = new Vector3(transform.position.x + x, 0, transform.position.z + y);
		return pos;
	}

	public Vector2 FindGridPos(Vector3 position) {
		return new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
	}

	public struct Square {
		public int posX, posY;
		public int topLeft, topRight, bottomRight, bottomLeft;

		public Square (int _posX, int _posY, int _topLeft, int _topRight, int _bottomRight, int _bottomLeft) {
			posX = _posX;
			posY = _posY;
			topLeft = _topLeft;
			topRight = _topRight;
			bottomRight = _bottomRight;
			bottomLeft = _bottomLeft;
		}
	}

	public struct Chunk {
		public int posX, posY;

		public Square[,] localGrid;

		private List<Vector3> localVertices;
		private List<int> localTriangles;

		private Map map;

		private GameObject gObject;

		private static float weightScale;

		public bool inspect;

		public Chunk (int _posX, int _posY, Square[,] _grid, Map _map) {
			posX = _posX;
			posY = _posY;
			map = _map;

			localGrid = new Square[10, 10];
			localVertices = new List<Vector3>();
			localTriangles = new List<int>();

			int newX = posX * 10;
			int newY = posY * 10;

			for (int x = 0; x < 10; x++) {
				for (int y = 0; y < 10; y++) {
					localGrid[x, y] = _grid[newX + x, newY + y];
				}
			}

			gObject = Instantiate(map.resManager.chunk, map.FindWorldPos(posX * 10, posY * 10), map.transform.rotation);
			gObject.transform.parent = map.transform;

			weightScale = 0.125f;

			inspect = false;

			CreateMesh();
		}

		private void CreateMesh() {
			Mesh mesh = new Mesh() { subMeshCount = 3 };

			MarchingSquares(0);
			mesh.vertices = localVertices.ToArray();
			mesh.SetTriangles(localTriangles.ToArray(), 0);

			localTriangles = new List<int>();
			MarchingSquares(2);
			mesh.vertices = localVertices.ToArray();
			mesh.SetTriangles(localTriangles.ToArray(), 1);

			localTriangles = new List<int>();
			MarchingSquares(3);
			mesh.vertices = localVertices.ToArray();
			mesh.SetTriangles(localTriangles.ToArray(), 2);

			gObject.GetComponent<MeshFilter>().mesh = mesh;
		}

		private void MarchingSquares(int type) {
			for (int x = 0; x < 10; x++) {
				for (int y = 0; y < 10; y++) {
					Square s = localGrid[x, y];
					int config = 0;

					if (s.topLeft == type) config += 8;
					if (s.topRight == type) config += 4;
					if (s.bottomRight == type) config += 2;
					if (s.bottomLeft == type) config += 1;

					Vector3 topLeft = new Vector3(x - 0.5f, 0, y + 0.5f);
					Vector3 topRight = new Vector3(x + 0.5f, 0, y + 0.5f);
					Vector3 bottomRight = new Vector3(x + 0.5f, 0, y - 0.5f);
					Vector3 bottomLeft = new Vector3(x - 0.5f, 0, y - 0.5f);

					float topWeight = 0.5f; if (y <= map.worldSizeY) topWeight = 0.125f * ((map.gridWeights[s.posX, s.posY + 1] + (8 - map.gridWeights[s.posX + 1, s.posY + 1])) / 2);
					Vector3 centerTop = new Vector3((x - 0.5f) + (topWeight), 0, y + 0.5f);

					float rightWeight = 0.5f; if (x <= map.worldSizeX) rightWeight = 0.125f * ((map.gridWeights[s.posX + 1, s.posY] + (8 - map.gridWeights[s.posX + 1, s.posY + 1])) / 2);
					Vector3 centerRight = new Vector3(x + 0.5f, 0, (y - 0.5f) + (rightWeight));

					float bottomWeight = 0.5f; if (y >= 0) bottomWeight = 0.125f * ((map.gridWeights[s.posX, s.posY] + (8 - map.gridWeights[s.posX + 1, s.posY])) / 2);
					Vector3 centerBottom = new Vector3((x - 0.5f) + (bottomWeight), 0, y - 0.5f);
					
					float leftWeight = 0.5f; if (x >= 0) leftWeight = 0.125f * ((map.gridWeights[s.posX, s.posY] + (8 - map.gridWeights[s.posX, s.posY + 1])) / 2);
					Vector3 centerLeft = new Vector3(x - 0.5f, 0, (y - 0.5f) + (leftWeight));

					//if (s.posY > 7 && s.posY < 15 && !map.pp) Debug.Log("(" + s.posX + ", " + s.posY + ") " + map.gridWeights[s.posX, s.posY + 1] + " | " + map.gridWeights[s.posX + 1, s.posY + 1] + " | " + topWeight);
					//if (s.posY > 7 && s.posY < 15 && !map.pp) Debug.Log("(" + s.posX + ", " + s.posY + ") " + map.gridWeights[s.posX, s.posY] + " | " + map.gridWeights[s.posX + 1, s.posY] + " | " + bottomWeight);

					switch (config) {
						//Case 15 is being excluded as we want to avoid having too many triangles for optimisation //This is being ignored for now as we get things working

						//The top config is the corner config, the bottom config is the vertice config

						//Start with single corners
						case 1:     //Bottom left
							CreateTriangles(centerBottom, bottomLeft, centerLeft);
							break;
						case 2:     //Bottom Right
							CreateTriangles(centerRight, bottomRight, centerBottom);
							break;
						case 4:     //Top Right
							CreateTriangles(centerTop, topRight, centerRight);
							break;
						case 8:     //Top Left
							CreateTriangles(topLeft, centerTop, centerLeft);
							break;

						//Full Sides
						case 3:     //Full Bottom
							CreateTriangles(centerRight, bottomRight, bottomLeft, centerLeft);
							break;
						case 6:     //Full Right
							CreateTriangles(centerTop, topRight, bottomRight, centerBottom);
							break;
						case 9:     //Full left
							CreateTriangles(topLeft, centerTop, centerBottom, bottomLeft);
							break;
						case 12:    //Full Top
							CreateTriangles(topLeft, topRight, centerRight, centerLeft);
							break;

						//Diagonal
						case 5:     //Top Right and Bottom left
							CreateTriangles(centerTop, topRight, centerRight, centerBottom, bottomLeft, centerLeft);
							break;
						case 10:    //Top Left and Bottom Right
							CreateTriangles(topLeft, centerTop, centerRight, bottomRight, centerBottom, centerLeft);
							break;

						//Three Corners
						case 7:     //Top Right, Bottom Right and Bottom left
							CreateTriangles(centerTop, topRight, bottomRight, bottomLeft, centerLeft);
							break;
						case 11:    //Top left, Bottom right and Bottom left
							CreateTriangles(topLeft, centerTop, centerRight, bottomRight, bottomLeft);
							break;
						case 13:    //Top Right, Top Left and Bottom Left
							CreateTriangles(topLeft, topRight, centerRight, centerBottom, bottomLeft);
							break;
						case 14:    //Top Left, Top Right and Bottom Right
							CreateTriangles(topLeft, topRight, bottomRight, centerBottom, centerLeft);
							break;
						case 15:    //All four
							CreateTriangles(topLeft, topRight, bottomRight, bottomLeft);
							break;
					}
				}
			}
		}

		private void CreateTriangles(params Vector3[] points) {
			List<int> references = new List<int>();

			for (int i = 0; i < points.Length; i++) {
				references.Add(localVertices.Count);
				localVertices.Add(points[i]);
			}

			if (points.Length >= 3) CreateTriangle(references[0], references[1], references[2]);
			if (points.Length >= 4) CreateTriangle(references[0], references[2], references[3]);
			if (points.Length >= 5) CreateTriangle(references[0], references[3], references[4]);
			if (points.Length >= 6) CreateTriangle(references[0], references[4], references[5]);
		}

		private void CreateTriangle(int a, int b, int c) {
			localTriangles.Add(a);
			localTriangles.Add(b);
			localTriangles.Add(c);
		}
	}

	private void OnDrawGizmos() {
		foreach (Chunk c in chunkGrid) {
			//if (c.inspect) {
				for (int x = 0; x < 10; x++) {
					for (int y = 0; y < 10; y++) {
						Gizmos.color = Color.black;
						Gizmos.DrawCube(FindWorldPos((c.posX * 10) + x, (c.posY * 10) + y), Vector3.one / 4);
					}
				}
			//}
		}

		for (int x = 0; x < worldSizeX; x++) {
			for (int y = 0; y < worldSizeY; y++) {
				Gizmos.color = Color.red;
				Vector3 v = FindWorldPos(x, y);
				v.x -= 0.5f;
				v.z -= 0.5f;
				Gizmos.DrawCube(v, Vector3.one / 4);
			}
		}
	}
}