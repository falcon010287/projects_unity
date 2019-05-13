using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris : MonoBehaviour {

	float fall = 0; 					// время прошедшее между смещение фигуры вниз
	public float fallSpeed = 1; 		// задержка между смещениями фигур вниз
	public bool allowRotation = true; 	// разрешить вращение фигуры
	public bool limitRotation = false; 	// ограничить вращение до угла в 90 градусов

	private float continuousVerticalSpeed = 0.05f; 		// скорость перемещения блока вниз при зажатой клавише вниз 
	private float continuousHorizontalSpeed = 0.1f; 	// скорость перемещения блока по горизонтали при зажатой клавише перемещения
	private float buttonDownWaitMax = 0.2f; 			// задержка перед повторением перемещения при зажатой клавише

	private float verticalTimer = 0;
	private float horizontalTimer = 0;
	private float buttonDownWaitTimer = 0;

	private bool movedImmediateHorizontal = false; 		// используется для обеспечения задержки перед началом повторения перемещения 
	private bool movedImmediateVertical = false;		// используется для обеспечения задержки перед началом повторения перемещения

	public int individualScore = 100; // максимальное количество очков за скоростную установку блока

	private float individualScoreTime;

	// Update is called once per frame
	void Update () {

		CheckUserInput ();

		UpdateIndividualScore ();
	}

	// пересчёт очков за скоростную установку блока
	void UpdateIndividualScore() {

		if (individualScoreTime < 1) {

			individualScoreTime += Time.deltaTime;
		} else {

			individualScoreTime = 0;

			individualScore = Mathf.Max (individualScore - 10, 0);
		}
	}

	// управление установкой блока
	void CheckUserInput() {

		if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.DownArrow)) {

			movedImmediateHorizontal = false;
			movedImmediateVertical = false;

			horizontalTimer = 0;
			verticalTimer = 0;
			buttonDownWaitTimer = 0;
		}

		if (Input.GetKey (KeyCode.RightArrow)) {

			if (movedImmediateHorizontal) {
				if (buttonDownWaitTimer < buttonDownWaitMax) {

					buttonDownWaitTimer += Time.deltaTime;
					return;
				}

				if (horizontalTimer < continuousHorizontalSpeed) {

					horizontalTimer += Time.deltaTime;
					return;
				}
			}

			if (!movedImmediateHorizontal)
				movedImmediateHorizontal = true;

			horizontalTimer = 0;

			BlocksMoveRight ();

			if (CheckIsValidPosition ()) {

				FindObjectOfType<Game> ().UpdateGrid (this);

			} else {

				transform.position += new Vector3 (-1, 0, 0);
			}


		} else if (Input.GetKey (KeyCode.LeftArrow)) {

			if (movedImmediateHorizontal) {
				if (buttonDownWaitTimer < buttonDownWaitMax) {

					buttonDownWaitTimer += Time.deltaTime;
					return;
				}

				if (horizontalTimer < continuousHorizontalSpeed) {

					horizontalTimer += Time.deltaTime;
					return;
				}
			}

			if (!movedImmediateHorizontal)
				movedImmediateHorizontal = true;
			
			horizontalTimer = 0;

			BlocksMoveLeft ();

			if (CheckIsValidPosition ()) {

				FindObjectOfType<Game> ().UpdateGrid (this);

			} else {

				transform.position += new Vector3 (1, 0, 0);
			}


		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {

			if (allowRotation) {

				if (limitRotation) {

					if (transform.rotation.eulerAngles.z >= 90) {

						BlocksRotationRight ();

					} else {

						BlocksRotationLeft ();
					}

				} else {
					
					BlocksRotationLeft ();
				}

				if (CheckIsValidPosition ()) {

					FindObjectOfType<Game> ().UpdateGrid (this);

				} else {

					if (limitRotation) {
						if (transform.rotation.eulerAngles.z >= 90) {

							BlocksRotationRight ();
						} else {

							BlocksRotationLeft ();
						}
					} else {
						BlocksRotationRight ();
					}
				}
			}


		} else if (Input.GetKey (KeyCode.DownArrow) || Time.time - fall >= fallSpeed) {

			if (movedImmediateVertical) {
				if (buttonDownWaitTimer < buttonDownWaitMax) {

					buttonDownWaitTimer += Time.deltaTime;
					return;
				}

				if (verticalTimer < continuousVerticalSpeed) {

					verticalTimer += Time.deltaTime;
					return;
				}
			}

			if (!movedImmediateVertical)
				movedImmediateVertical = true;

			verticalTimer = 0;

			transform.position += new Vector3 (0, -1, 0);

			BlocksStabilization ();

			if (CheckIsValidPosition ()) {

				FindObjectOfType<Game> ().UpdateGrid (this);

			} else {

				transform.position += new Vector3 (0, 1, 0);

				FindObjectOfType<Game> ().DeleteRow ();

				if (FindObjectOfType<Game> ().CheckIsAboveGrid (this)) {

					FindObjectOfType<Game> ().GameOver ();
				}

				FindObjectOfType<Game> ().SpawnNextBlock ();

				Game.currentScore += individualScore;

				enabled = false;
			}

			fall = Time.time;
		}
	}

	// проверка на правильную позицию блока
	bool CheckIsValidPosition() {

		foreach (Transform block in transform) {

			Vector2 pos = block.position;

			if (FindObjectOfType<Game> ().CheckIsInsideGrid (pos) == false) {
				
				return false;
			}

			if (FindObjectOfType<Game> ().GetTransformAtGridPosition (pos) != null && FindObjectOfType<Game> ().GetTransformAtGridPosition (pos).parent != transform) {

				return false;
			}
		}

		return true;
	}

	// смещение блока влево
	void BlocksMoveLeft() {

		if (Game.TableSlider) {

			transform.position += new Vector3 (-1, 0, 0);

			BlocksStabilization ();
		} else {

			transform.position += new Vector3 (-1, 0, 0);
		}
	}

	// смещение блока вправо
	void BlocksMoveRight() {

		if (Game.TableSlider) {

			transform.position += new Vector3 (1, 0, 0);

			BlocksStabilization ();
		} else {

			transform.position += new Vector3 (1, 0, 0);
		}
	}

	// вращение блока против часовой стрелки
	void BlocksRotationLeft() {

		if (Game.TableSlider) {

			float dx = Game.gridWidth / 2 - transform.position.x;

			transform.position += new Vector3 (dx, 0, 0);

			BlocksStabilization ();

			transform.Rotate (0, 0, 90);

			transform.position += new Vector3 (-dx, 0, 0);

			BlocksStabilization ();
		} else {

			transform.Rotate (0, 0, 90);
		}
	}

	// вращение блока по часовой стрелки
	void BlocksRotationRight() {
		
		if (Game.TableSlider) {

			float dx = Game.gridWidth / 2 - transform.position.x;

			transform.position += new Vector3 (dx, 0, 0);

			BlocksStabilization ();

			transform.Rotate (0, 0, -90);

			transform.position += new Vector3 (-dx, 0, 0);

			BlocksStabilization ();
		} else {

			transform.Rotate (0, 0, -90);
		}
	}

	// корректировка координат блока при движении за границу поля (перенос на другую строну)
	void BlocksStabilization() {

		if (transform.position.x < 0)
			transform.position += new Vector3 (Game.gridWidth, 0, 0);

		if (transform.position.x > Game.gridWidth - 1)
			transform.position -= new Vector3 (Game.gridWidth, 0, 0);
		
		foreach (Transform block in transform) {

			if (block.transform.position.x < 0)
				block.transform.position += new Vector3 (Game.gridWidth, 0, 0);
			
			if (Mathf.Round(block.transform.position.x) >= Game.gridWidth)
				block.transform.position += new Vector3(-Game.gridWidth, 0, 0);
			
			if (block.transform.position.x > Game.gridWidth - 1) {
				block.transform.position += new Vector3(-Game.gridWidth, 0, 0);
			}
		}
	}
}
