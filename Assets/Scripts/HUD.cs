using System;
using System.Collections;
using System.Collections.Generic;
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

	public GameObject tutorialBox, pauseBox, gameOverBox;
	public TextMeshProUGUI nameField;

	private int activeScene;
	public bool isPlaying = true;
	public bool isGamePaused = false;
	private int health = 10;
	private int score;

	private void Awake() {
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
			float time = Time.time;
			score = Mathf.FloorToInt(time);
			this.time.text = "Time:  " + TimeSpan.FromSeconds(time).ToString(@"mm\:ss");
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
		SceneManager.LoadScene(0);
	}

	public void ExitGame() {
		Application.Quit();
		Debug.Log("Exiting game...");
	}

	public void ShowLeaderboard() {

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

	public void GameOver() {
		isPlaying = false;
		gameOverBox.SetActive(true);
	}

	public void SubmitScore() {
		string name = nameField.text;

		if (name.Length - 1 == 3) {
			gameOverBox.SetActive(false);
		}

		SceneManager.LoadScene(0);
	}
}
