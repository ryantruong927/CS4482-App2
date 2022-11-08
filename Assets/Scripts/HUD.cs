using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	public Image[] hearts;
	public Sprite fullHeart, halfHeart, emptyHeart;

	public TextMeshProUGUI time;

	public Image meleeWeaponBox, meleeWeapon;
	public Image rangedWeaponBox, rangedWeapon;

	public GameObject leaderboardBox, tutorialBox, pauseBox, submitBox;
	public TextMeshProUGUI nameField;

	private int activeScene;
	private bool hasWon = false;
	public bool isPlaying = true;
	public bool isGamePaused = false;
	private int health = 10;
	private float score;
	private string scoreFormatted;
	string dataPath;

	private void Awake() {
		dataPath = Application.persistentDataPath + "/leaderboard.txt";

		activeScene = SceneManager.GetActiveScene().buildIndex;

		if (activeScene == 0 || tutorialBox != null) {
			Time.timeScale = 0;
			isPlaying = false;
		}
		else {
			rangedWeaponBox.color = Color.gray;
			rangedWeapon.color = Color.gray;
		}
	}

	private void Update() {
		if (activeScene != 0 && isPlaying && !isGamePaused) {
			score = Time.timeSinceLevelLoad;
			scoreFormatted = TimeSpan.FromSeconds(score).ToString(@"mm\:ss");
			this.time.text = "Time:  " + scoreFormatted;
		}
	}

	public void StartGame() {
		SceneManager.LoadScene(1);
	}

	public void PauseGame() {
		if (activeScene != 0 && isPlaying) {
			if (!isGamePaused) {
				Time.timeScale = 0;
				pauseBox.SetActive(true);
			}
			else {
				Time.timeScale = 1;
				pauseBox.SetActive(false);
			}
			isGamePaused = !isGamePaused;
		}
	}

	public void PlayGame() {
		Time.timeScale = 1;
		isPlaying = true;
		Destroy(tutorialBox);
	}

	public void RestartGame() {
		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void ExitLevel() {
		SceneManager.LoadScene(0);
	}

	public void ExitGame() {
		Application.Quit();
		Debug.Log("Exiting game...");
	}

	public void ShowLeaderboard() {
		TextMeshProUGUI level1Board = leaderboardBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI level2Board = leaderboardBox.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
		level1Board.text = "";
		level2Board.text = "";

		FileStream fs;
		BinaryFormatter bf = new();

		try {
			fs = new(dataPath, FileMode.Open, FileAccess.Read, FileShare.None);

			LeaderboardData leaderboardData = (LeaderboardData)bf.Deserialize(fs);

			fs.Close();

			List<ScoreData> scoreData = leaderboardData.entries;
			scoreData.Sort((x, y) => x.score.CompareTo(y.score));

			List<string> level1Scores = new List<String>();
			List<string> level2Scores = new List<String>();

			foreach (ScoreData score in scoreData) {
				string scoreFormatted = TimeSpan.FromSeconds(score.score).ToString(@"mm\:ss");
				if (score.level == 1)
					level1Scores.Add(score.name + " - " + scoreFormatted + "\n");
				else
					level2Scores.Add(score.name + " - " + scoreFormatted + "\n");
			}

			for (int i = 0; i < 10; i++) {
				if (i < level1Scores.Count)
					level1Board.text += (i + 1) + ". " + level1Scores[i];
				else
					level1Board.text += (i + 1) + ". " + "\n";

				if (i < level2Scores.Count)
					level2Board.text += (i + 1) + ". " + level2Scores[i];
				else
					level2Board.text += (i + 1) + ". " + "\n";
			}
		}
		catch (FileNotFoundException) {
			for (int i = 0; i < 10; i++) {
				level1Board.text += (i + 1) + ". " + "\n";
				level2Board.text += (i + 1) + ". " + "\n";
			}
		}
	}

	public void UpdateHearts(int amount) {
		int currentHealth = Mathf.Clamp(health + amount, 0, 10);

		if (currentHealth == 0) {
			for (int i = 0; i < hearts.Length; i++)
				hearts[i].sprite = emptyHeart;

			GameOver();
		}
		else if (amount < 0) {
			for (int i = Mathf.CeilToInt((health - 1) / 2); i >= Mathf.CeilToInt((currentHealth - 1) / 2); i--) {
				if (i != Mathf.CeilToInt((currentHealth - 1) / 2))
					hearts[i].sprite = emptyHeart;
				else if (currentHealth % 2 != 0)
					hearts[i].sprite = halfHeart;
			}
		}
		else {
			for (int i = Mathf.CeilToInt((health - 1) / 2); i <= Mathf.CeilToInt((currentHealth - 1) / 2); i++) {
				if (i != Mathf.CeilToInt((currentHealth - 1) / 2))
					hearts[i].sprite = emptyHeart;
				else if (currentHealth % 2 != 0)
					hearts[i].sprite = halfHeart;
			}
		}

		health = currentHealth;
	}

	public void Swap(bool isMeleeEquipped) {
		if (isMeleeEquipped) {
			meleeWeaponBox.color = Color.white;
			meleeWeapon.color = Color.white;
			rangedWeaponBox.color = Color.gray;
			rangedWeapon.color = Color.gray;
		}
		else {
			meleeWeaponBox.color = Color.gray;
			meleeWeapon.color = Color.gray;
			rangedWeaponBox.color = Color.white;
			rangedWeapon.color = Color.white;
		}
	}

	public void PickUpWeapon(Sprite weapon, bool isMelee) {
		if (isMelee) {
			meleeWeapon.sprite = weapon;
			meleeWeapon.rectTransform.sizeDelta = weapon.rect.size;
			meleeWeapon.gameObject.SetActive(true);
		}
		else {
			rangedWeapon.sprite = weapon;
			rangedWeapon.rectTransform.sizeDelta = weapon.rect.size;
			rangedWeapon.gameObject.SetActive(true);
		}
	}

	public void Win() {
		Time.timeScale = 0;
		hasWon = true;
		isPlaying = false;
		submitBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level " + SceneManager.GetActiveScene().buildIndex + " Completed!";
		submitBox.SetActive(true);
	}

	public void GameOver() {
		hasWon = false;
		isPlaying = false;
		pauseBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Over!";
		pauseBox.SetActive(true);
	}

	public void SubmitScore() {
		string name = nameField.text;

		if (name.Length - 1 == 3) {
			int buildIndex = SceneManager.GetActiveScene().buildIndex;
			FileStream fs;
			BinaryFormatter bf = new();

			LeaderboardData leaderboardData;
			ScoreData scoreData = new ScoreData();
			scoreData.level = buildIndex;
			scoreData.name = name.ToUpper();
			scoreData.score = score;

			if (!File.Exists(dataPath)) {
				fs = new(dataPath, FileMode.Create, FileAccess.Write, FileShare.None);
				leaderboardData = new LeaderboardData();
			}
			else {
				fs = new(dataPath, FileMode.Open, FileAccess.Read, FileShare.None);

				try {
					leaderboardData = (LeaderboardData)bf.Deserialize(fs);
				}
				catch (SerializationException) {
					leaderboardData = new LeaderboardData();
				}

				fs.Close();
				fs = new(dataPath, FileMode.Open, FileAccess.Write, FileShare.None);
			}

			leaderboardData.entries.Add(scoreData);
			bf.Serialize(fs, leaderboardData);
			fs.Close();

			submitBox.SetActive(false);
			if (hasWon) {
				if (buildIndex + 2 <= SceneManager.sceneCountInBuildSettings)
					SceneManager.LoadScene(buildIndex + 1);
				else
					SceneManager.LoadScene(0);
			}
			else
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		Time.timeScale = 1;
	}
}
