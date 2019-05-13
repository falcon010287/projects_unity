using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour {

	public Transform block;

	// генерация игрового поля
	public void TableGenerating() {
		
		for (int y = 0; y < Game.gridHeight; ++y) {
			for (int x = 0; x < Game.gridWidth; ++x) {

				Transform tile = Instantiate (block, new Vector3 (x, y, 0), Quaternion.identity);
				tile.SetParent(GameObject.FindGameObjectWithTag("Grid").transform);
			}
		}
	}
}
