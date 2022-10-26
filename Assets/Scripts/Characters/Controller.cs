using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	protected Vector2 lookDirection = new Vector2(0f, -1f);

	public int maxHealth = 10;
	[SerializeField]
	protected int currentHealth;
	public int Health { get { return currentHealth; } }

	public bool isMeleeEquipped = true;
	[SerializeField]
	protected MeleeData melee;
	[SerializeField]
	protected RangedData ranged;

	public float invincibleTime = 1;
	protected bool isInvincible = false;
	protected float invisibleTimer;
	protected bool isKnockedBack = false;
	protected int knockback;
	protected Vector2 knockbackPosition;

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

		if (melee != null)
			transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = melee.sprite;
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
				transform.GetChild(0).gameObject.SetActive(false);
			}
		}
	}

	protected virtual void FixedUpdate() {
		if (isKnockedBack) {
			if (rb.position == knockbackPosition) {
				isKnockedBack = false;
				return;
			}

			Vector2 position = Vector2.MoveTowards(rb.position, knockbackPosition, knockback * Time.deltaTime);
			rb.MovePosition(position);
			Debug.Log("Wtfd");
		}
	}

	public virtual void Hit(int dmg) {
		if (!isInvincible) {
			currentHealth -= dmg;

			if (currentHealth <= 0)
				Destroy(gameObject);
			else {
				invisibleTimer = invincibleTime;
				isInvincible = true;
			}
		}
	}


	public virtual void Hit(int dmg, int knockback, Vector2 knockbackPosition) {
		if (!isInvincible) {
			currentHealth -= dmg;
			isKnockedBack = true;
			this.knockback = knockback;
			this.knockbackPosition = knockbackPosition - rb.position;
			float magnitude = this.knockbackPosition.magnitude + knockback;
			this.knockbackPosition.Normalize();
			this.knockbackPosition *= magnitude;

			if (currentHealth <= 0)
				Destroy(gameObject);
			else {
				invisibleTimer = invincibleTime;
				isInvincible = true;
			}
		}
	}

	public virtual void Heal(int heal) {
		if (currentHealth != maxHealth)
			currentHealth = Mathf.Clamp(currentHealth + heal, 0, maxHealth);
	}

	public virtual void Attack() {
		anim.SetBool("Attacking", true);
		isAttacking = true;
		attackTimer = attackTime;
		BoxCollider2D hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>();

		if (isMeleeEquipped && melee != null) {
			hitbox.size = new Vector2(melee.range, melee.range);

			if (lookDirection.x == 0)
				hitbox.offset = new Vector2(0, lookDirection.y > 0 ? melee.range * 1.5f : melee.range * -0.5f);
			else if (lookDirection.y == 0)
				hitbox.offset = new Vector2(lookDirection.x * melee.range, melee.range * 0.5f);
			else {
				if (lookDirection.y == 0)
					hitbox.offset = new Vector2(hitbox.offset.x, 0);
				else
					hitbox.offset = new Vector2(hitbox.offset.x, -1 * melee.range / 2);
			}
		}
		else if (ranged != null) {
			hitbox.size = Vector2.zero;
			float lookDirectionX = lookDirection.x;

			if (lookDirectionX == 0)
				lookDirectionX = lookDirection.y < 0 ? lookDirectionX -= 0.125f : lookDirectionX -= 0.1875f;

			Quaternion rotation = Quaternion.FromToRotation(Vector2.right, lookDirection);

			GameObject projectile = Instantiate(ranged.projectilePrefab, transform.position + new Vector3(lookDirectionX, lookDirection.y + 0.25f, 0), rotation);
			projectile.GetComponent<Projectile>().Create(ranged, lookDirection);
		}
	}
}
