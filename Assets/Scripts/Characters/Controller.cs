using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	protected Rigidbody2D rb;
	protected Animator anim;

	protected Transform hitbox;

	[SerializeField]
	protected Transform equippedWeapon;
	public MeleeData meleeWeapon;
	public RangedData rangedWeapon;

	protected Vector2 lookDirection = new Vector2(0f, -1f);

	public int maxHealth = 10;
	[SerializeField]
	protected int currentHealth;
	public int Health { get { return currentHealth; } }

	public float flickerTime = 0.2f;
	protected float flickerTimer;
	protected bool isFlickering = false;

	public float invincibleTime = 1;
	protected bool isInvincible = false;
	protected float invisibleTimer;
	protected bool isKnockedBack = false;
	protected int knockback;
	protected Vector3 knockbackPosition;

	public bool isMeleeEquipped = true;
	protected bool isAttacking = false;
	protected float attackTimer;

	public float speed = 3.5f;

	protected virtual void Start() {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		hitbox = transform.GetChild(0);
		equippedWeapon = transform.GetChild(1);
		currentHealth = maxHealth;

		if (meleeWeapon != null)
			transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = meleeWeapon.sprite;
	}

	protected virtual void Update() {
		if (isInvincible) {
			flickerTimer -= Time.deltaTime;
			invisibleTimer -= Time.deltaTime;

			if (flickerTimer <= 0) {
				GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
				flickerTimer = flickerTime * 0.5f;
			}
			if (invisibleTimer <= 0) {
				if (currentHealth == 1) {
					flickerTimer = flickerTime;
					isFlickering = true;
				}

				GetComponent<SpriteRenderer>().enabled = true;
				isInvincible = false;
			}
		}

		if (isAttacking) {
			attackTimer -= Time.deltaTime;

			if (attackTimer <= 0) {
				anim.SetBool("Attacking", false);
				isAttacking = false;
				hitbox.gameObject.SetActive(false);

				if (!isMeleeEquipped)
					Fire();
			}

			hitbox.position = equippedWeapon.position;
			hitbox.rotation = equippedWeapon.rotation;
		}
	}

	protected virtual void FixedUpdate() {
		if (isKnockedBack) {
			Vector2 position = Vector2.MoveTowards(rb.position, knockbackPosition, knockback * 2 * Time.deltaTime);
			rb.MovePosition(position);
			Debug.Log(Vector3.Distance(transform.position, knockbackPosition));
			if (Vector3.Distance(rb.position, knockbackPosition) <= 0)
				isKnockedBack = false;
		}

		rb.velocity = Vector3.zero;
	}

	public virtual void Hit(int dmg) {
		if (!isInvincible) {
			anim.SetBool("Attacking", false);
			currentHealth -= dmg;

			if (currentHealth <= 0) {
				GetComponent<SpriteRenderer>().enabled = true;
				anim.SetBool("Dead", true);
				Destroy(this);
			}
			else {
				GetComponent<SpriteRenderer>().enabled = false;
				flickerTimer = flickerTime * 0.5f;

				invisibleTimer = invincibleTime;
				isInvincible = true;
			}
		}
	}


	//public virtual void Hit(int dmg, int knockback, Vector3 knockbackPosition) {
	//	if (!isInvincible) {
	//		anim.SetBool("Attacking", false);
	//		currentHealth -= dmg;

	//		if (currentHealth <= 0)
	//			Destroy(gameObject);
	//		else {
	//			flickerTimer = flickerTime;
	//			isFlickering = true;

	//			invisibleTimer = invincibleTime;
	//			isInvincible = true;

	//			Debug.Log(transform.position);
	//			Debug.Log(knockbackPosition);
	//			isKnockedBack = true;
	//			this.knockback = knockback;
	//			this.knockbackPosition = transform.position - knockbackPosition;
	//			float magnitude = this.knockbackPosition.magnitude + knockback;
	//			this.knockbackPosition.Normalize();
	//			this.knockbackPosition *= magnitude;
	//			Debug.Log(knockbackPosition);
	//		}
	//	}
	//}

	public virtual void Heal(int heal) {
		if (currentHealth != maxHealth) {
			currentHealth = Mathf.Clamp(currentHealth + heal, 0, maxHealth);

			if (currentHealth > 1) {
				GetComponent<SpriteRenderer>().enabled = true;
				isFlickering = false;
			}
		}
	}

	public virtual void Attack() {
		BoxCollider2D hitboxCollider = hitbox.GetComponent<BoxCollider2D>();

		if (isMeleeEquipped && meleeWeapon != null) {
			anim.SetBool("Attacking", true);
			isAttacking = true;
			attackTimer = CompareTag("Enemy") ? meleeWeapon.speed * 2 : meleeWeapon.speed;

			hitboxCollider.size = new Vector2(meleeWeapon.range, meleeWeapon.range);
			hitboxCollider.offset = new Vector2(0, meleeWeapon.range * -0.5f);
		}
		else if (rangedWeapon != null) {
			anim.SetBool("Attacking", true);
			isAttacking = true;
			attackTimer = CompareTag("Enemy") ? rangedWeapon.chargeTime * 2 : rangedWeapon.chargeTime;
		}
	}

	protected virtual void Fire() {
		hitbox.GetComponent<BoxCollider2D>().size = Vector2.zero;
		float lookDirectionX = lookDirection.x;

		if (lookDirectionX == 0)
			lookDirectionX = lookDirection.y < 0 ? lookDirectionX -= 0.125f : lookDirectionX -= 0.1875f;

		Quaternion rotation = Quaternion.FromToRotation(Vector2.right, lookDirection);

		GameObject projectile = Instantiate(rangedWeapon.projectilePrefab, transform.position + new Vector3(lookDirectionX, lookDirection.y + 0.25f, 0), rotation);
		projectile.GetComponent<Projectile>().Create(tag, rangedWeapon, lookDirection);
	}
}
