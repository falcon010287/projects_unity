using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour {

	public void PlayAgain() {
		
		SceneManager.LoadScene ("GameMenu");
	}

	// Режим игры 1
	public void PlayModeOne() {
		
		Game.gridWidth = 10;
		Game.gridHeight = 20;
		Game.TableSlider = false;
		Game.CountMinLineToClear = 1;

		Game.blocksChance [0] = 15;
		Game.blocksChance [1] = 15;
		Game.blocksChance [2] = 10;
		Game.blocksChance [3] = 15;
		Game.blocksChance [4] = 10;
		Game.blocksChance [5] = 20;
		Game.blocksChance [6] = 15;
		Game.blocksChance [7] = 0;
		Game.blocksChance [8] = 0;
		Game.blocksChance [9] = 0;

		SceneManager.LoadScene ("Level");
	}

	// Режим игры 1
	public void PlayModeTwo() {

		Game.gridWidth = 12;
		Game.gridHeight = 20;
		Game.TableSlider = true;
		Game.CountMinLineToClear = 2;

		Game.blocksChance [0] = 15;
		Game.blocksChance [1] = 15;
		Game.blocksChance [2] = 10;
		Game.blocksChance [3] = 15;
		Game.blocksChance [4] = 10;
		Game.blocksChance [5] = 5;
		Game.blocksChance [6] = 15;
		Game.blocksChance [7] = 5;
		Game.blocksChance [8] = 5;
		Game.blocksChance [9] = 5;

		SceneManager.LoadScene ("Level");
	}
}
