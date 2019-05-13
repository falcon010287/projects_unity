using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	public static bool TableSlider; // Блоки переходят с одного конца поля на другой при выходе за границы
	public static int CountMinLineToClear; // минимальное количество строк удаляемых с поля
	public static int gridWidth; // Ширина игрового поля
	public static int gridHeight; // Высота игрового поля
	public GameObject[] blocks = new GameObject[10]; // префабы фигур
	public static int[] blocksChance = new int[10]; // параметры вероятности выпадания фигур

	public static Transform[,] grid = new Transform[12, 20]; // Матрица расположения блоков игрового поля

	public int scoreOneLine = 40; 		// очки за удаление одной строки
	public int scoreTwoLine = 100;		// очки за удаление двух строк
	public int scoreThreeLine = 300;	// очки за удаление трёх строк
	public int scoreFourLine = 1200;	// очки за удаление четырёх строк

	public Text hud_score;

	private int numberOfRowsThisTurn = 0;

	public static int currentScore = 0;

	void Start () {

		hud_score.text = "0";
		currentScore = 0;

		FindObjectOfType<Table> ().TableGenerating ();

		SpawnNextBlock ();
	}

	void Update() {

		UpdateScore ();

		UpdateUI ();
	}

	// обновление счётчика очков на экране
	public void UpdateUI() {

		hud_score.text = currentScore.ToString ();
	}

	// подсчёт очков за скорость установки фигуры
	public void UpdateScore() {

		if (numberOfRowsThisTurn > 0) {

			if (numberOfRowsThisTurn == 1) {

				ClearedOneLine ();

			} else if (numberOfRowsThisTurn == 2) {

				ClearedTwoLine ();

			} else if (numberOfRowsThisTurn == 3) {

				ClearedThreeLine ();

			} else if (numberOfRowsThisTurn == 4) {

				ClearedFourLine ();

			}

			numberOfRowsThisTurn = 0;
		}
	}

	// присвоение очков за удаление одной строки
	public void ClearedOneLine() {

		currentScore += scoreOneLine;
	}

	// присвоение очков за удаление двух строк
	public void ClearedTwoLine() {

		currentScore += scoreTwoLine;
	}

	// присвоение очков за удаление трёх строк
	public void ClearedThreeLine() {

		currentScore += scoreThreeLine;
	}

	// присвоение очков за удаление четырёх строк
	public void ClearedFourLine() {

		currentScore += scoreFourLine;
	}

	// проверка фигуры на нахождение за пределами поля
	public bool CheckIsAboveGrid(Tetris tetris) {

		for (int x = 0; x < gridWidth; ++x) {

			foreach (Transform tet in tetris.transform) {

				Vector2 pos = Round (tet.position);

				if (pos.y > gridHeight - 1) {

					return true;
				}
			}
		}

		return false;
	}

	// Проверка строки на полное заполнение
	public bool IsFullRowAt(int y) {

		for (int x = 0; x < gridWidth; ++x) {

			if (grid [x, y] == null) {

				return false;
			}
		}

		numberOfRowsThisTurn++;

		return true;
	}

	// Удаление строки блоков
	public void DeleteTetAt(int y) {

		for (int x = 0; x < gridWidth; ++x) {

			Destroy (grid [x, y].gameObject);

			grid [x, y] = null;
		}
	}

	// Переместить строку блоков на один уровень вниз
	public void MoveRowDown(int y) {

		for (int x = 0; x < gridWidth; ++x) {

			if (grid [x, y] != null) {

				grid [x, y - 1] = grid [x, y];

				grid [x, y] = null;

				grid [x, y - 1].position += new Vector3 (0, -1, 0);
			}
		}
	}

	// Сместить все строки вниз начиная со строки Y
	public void MoveAllRowsDown(int y) {

		for (int i = y; i < gridHeight; ++i) {

			MoveRowDown (i);
		}
	}

	// Удаление всех целиком заполненых блоками строк со смещением оставшихся блоков вниз
	public void DeleteRow() {

		int countFullRow = 0;

		for (int y = 0; y < gridHeight; ++y) {

			if (IsFullRowAt (y)) {

				++countFullRow;

				if (countFullRow >= CountMinLineToClear && !IsFullRowAt (y + 1)) {

					for (int yy = 0; yy < countFullRow; ++yy) {

						DeleteTetAt (y);

						MoveAllRowsDown (y + 1);

						--y;
					}

					countFullRow = 0;
				}
			} else {

				countFullRow = 0;
			}
		}
	}

	// обновление расположение блоков на поле
	public void UpdateGrid(Tetris tetris) {

		for (int y = 0; y < gridHeight; ++y) {

			for (int x = 0; x < gridWidth; ++x) {

				if (grid [x, y] != null) {

					if (grid [x, y].parent == tetris.transform) {

						grid [x, y] = null;
					}
				}
			}
		}

		foreach (Transform tet in tetris.transform) {

			Vector2 pos = Round (tet.position);

			if (pos.y < gridHeight) {

				grid [(int)pos.x, (int)pos.y] = tet;
			}
		}
	}

	// получение ссылки на клетку поля
	public Transform GetTransformAtGridPosition (Vector2 pos) {

		if (pos.y > gridHeight - 1) {

			return null;

		} else {

			return grid [(int)pos.x, (int)pos.y];
		}
	}

	// генерация следующего блока
	public void SpawnNextBlock() {

		GameObject block = (GameObject) Instantiate (GetRandomBlock (), new Vector2 (Mathf.Round(gridWidth / 2), 20.0f), Quaternion.identity);
		block.transform.SetParent(GameObject.FindGameObjectWithTag("Blocks").transform);
	}

	// проверка нахождения координаты в игровом поле
	public bool CheckIsInsideGrid (Vector3 pos) {
		
		return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
	}

	public Vector2 Round (Vector2 pos) {

		return new Vector2 (Mathf.Round (pos.x), Mathf.Round (pos.y));
	}

	// случайный выбор следующего блока с условием вероятности
	GameObject GetRandomBlock() {
		
		int chance = Random.Range (1, 100);
		int numBlock = -1;
		int iBlock = 0;
		int sumChance = 0;

		do {

			sumChance += blocksChance[iBlock];

			if (chance <= sumChance && chance > sumChance - blocksChance[iBlock])
				numBlock = iBlock;
			
			++iBlock;
		} while (numBlock == -1 && iBlock <= 9);

		if (numBlock == -1)
			numBlock = 0;

		return blocks[numBlock];
	}

	// загрузка сцены окончания игры
	public void GameOver() {

		SceneManager.LoadScene ("GameOver");
	}
}
