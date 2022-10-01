using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	public int maxHealth = 10;
	[SerializeField]
	protected int currentHealth;
	public int Health { get { return currentHealth; } }

	public float invincibleTime = 1f;
	protected bool isInvincible = false;
	protected float invisibleTimer;

	public float speed = 3f;

	protected Rigidbody2D rigidbody2D;
	protected Animator animator;

	protected virtual void Start() {
		rigidbody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		currentHealth = maxHealth;
	}

	protected virtual void Update() {
		if (isInvincible) {
			invisibleTimer -= Time.deltaTime;

			if (invisibleTimer <= 0)
				isInvincible = false;
		}
	}

	public virtual void ChangeHealth(int amount) {
		if (!isInvincible || amount >= 0) {
			currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
			Debug.Log(gameObject + ":" + currentHealth + "/" + maxHealth);

			if (currentHealth <= 0)
				Destroy(gameObject);
			else if (amount < 0) {
				invisibleTimer = invincibleTime;
				isInvincible = true;
			}
		}
	}
}
