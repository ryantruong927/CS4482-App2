using UnityEngine;

public class PlayerController : Controller {
	private HUD HUD;

	private Vector2 input;

	private float pickUpDistance = 1.5f;
	public float pickUpTime = 1;
	private bool isPickingUpItem = false;
	private float pickUpTimer;

	private bool isSwapButtonDown = false;
	private bool isAttackButtonDown = false;

	protected override void Start() {
		base.Start();

		HUD = GetComponent<HUD>();
	}

	protected override void Update() {
		if (Input.GetKeyDown(KeyCode.Escape))
			HUD.PauseGame();

		if (!HUD.isGamePaused) {

			base.Update();

			if (!isInvincible && isFlickering) {
				flickerTimer -= Time.deltaTime;

				if (flickerTimer <= 0) {
					GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;

					if (currentHealth > 1)
						isFlickering = false;
					else
						flickerTimer = flickerTime;
				}
			}

			if (isPickingUpItem) {
				input = Vector2.zero;
				pickUpTimer -= Time.deltaTime;

				if (pickUpTimer <= 0) {
					anim.SetBool("Picking Up Item", false);
					lookDirection = Vector2.down;
					anim.SetFloat("Look X", lookDirection.x);
					anim.SetFloat("Look Y", lookDirection.y);
					isPickingUpItem = false;
					Destroy(transform.GetChild(2).gameObject);
				}
			}

			if (!isKnockedBack) {
				if (!isAttacking) {
					input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

					if (!Mathf.Approximately(input.x, 0.0f) || !Mathf.Approximately(input.y, 0.0f)) {
						lookDirection.Set(input.x, input.y);
						lookDirection.Normalize();
					}

					anim.SetFloat("Look X", lookDirection.x);
					anim.SetFloat("Look Y", lookDirection.y);
					anim.SetFloat("Speed", input.magnitude);

					if (Input.GetAxis("Swap") > 0) {
						if (!isSwapButtonDown) {
							isSwapButtonDown = true;
							Swap();
						}
					}
					else
						isSwapButtonDown = false;
					if (Input.GetAxis("Attack") > 0) {
						if (!isAttackButtonDown) {
							isAttackButtonDown = true;
							if ((isMeleeEquipped && meleeWeapon != null) || (!isMeleeEquipped && rangedWeapon != null))
								Attack();
						}
					}
					else
						isAttackButtonDown = false;
				}
			}
		}
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if (!isKnockedBack && !isPickingUpItem && !isAttacking) {
			if (input.magnitude > 1)
				input.Normalize();

			Vector2 position = rb.position;
			position += speed * Time.deltaTime * input;

			rb.MovePosition(position);
		}
	}

	public override void Hit(int dmg) {
		base.Hit(dmg);

		HUD.UpdateHearts(-dmg);
	}

	public override void Heal(int heal) {
		base.Heal(heal);

		HUD.UpdateHearts(heal);
	}

	public void Swap() {
		isMeleeEquipped = !isMeleeEquipped;

		if (isMeleeEquipped && meleeWeapon != null)
			equippedWeapon.GetComponent<SpriteRenderer>().sprite = meleeWeapon.sprite;
		else if (rangedWeapon != null)
			equippedWeapon.GetComponent<SpriteRenderer>().sprite = rangedWeapon.sprite;

		HUD.Swap(isMeleeEquipped);
	}

	public bool PickUp(Transform item) {
		if (isAttacking)
			return false;

		pickUpTimer = pickUpTime;
		isPickingUpItem = true;
		anim.SetBool("Picking Up Item", true);

		item.position = new Vector3(transform.position.x, transform.position.y + pickUpDistance, transform.position.z);
		item.parent = gameObject.transform;

		if (item.GetComponent<Melee>() != null) {
			meleeWeapon = item.GetComponent<Melee>().meleeData;
			HUD.PickUpWeapon(item.GetComponent<SpriteRenderer>().sprite, true);
			hitbox.GetComponent<Melee>().meleeData = meleeWeapon;

			if (isMeleeEquipped)
				equippedWeapon.GetComponent<SpriteRenderer>().sprite = meleeWeapon.sprite;
		}
		else if (item.GetComponent<Ranged>() != null) {
			rangedWeapon = item.GetComponent<Ranged>().rangedData;
			HUD.PickUpWeapon(item.GetComponent<SpriteRenderer>().sprite, false);
			hitbox.GetComponent<Ranged>().rangedData = rangedWeapon;

			if (!isMeleeEquipped)
				equippedWeapon.GetComponent<SpriteRenderer>().sprite = rangedWeapon.sprite;
		}

		return true;
	}
}
