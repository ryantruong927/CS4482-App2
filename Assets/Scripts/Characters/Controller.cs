using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	public int maxHealth = 10;
	[SerializeField]
	protected int currentHealth;
	public int Health { get { return currentHealth; } }

	public float invincibleTime = 1;
	protected bool isInvincible = false;
	protected float invisibleTimer;

	public float attackTime = 0.3f;
	protected bool isAttacking = false;
	protected float attackTimer;

	public float speed = 3.5f;

	protected Rigidbody2D rb;
	protected Animator anim;

	protected virtual void Start() {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		currentHealth = maxHealth;
	}

	protected virtual void Update() {
		if (isInvincible) {
			invisibleTimer -= Time.deltaTime;

			if (invisibleTimer <= 0)
				isInvincible = false;
		}

		if (isAttacking) {
			attackTimer -= Time.deltaTime;

			if (attackTimer <= 0) {
				anim.SetBool("Attacking", false);
				isAttacking = false;
			}
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
