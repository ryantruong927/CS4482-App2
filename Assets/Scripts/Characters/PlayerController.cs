using UnityEngine;

public class PlayerController : Controller {
	private Vector2 input;

	public float pickUpDistance = 2;
	public float pickUpTime = 1;
	private bool isPickingUpItem = false;
	private float pickUpTimer;

	protected override void Update() {
		base.Update();

		if (isPickingUpItem) {
			pickUpTimer -= Time.deltaTime;

			if (pickUpTimer <= 0) {
				isPickingUpItem = false;
				anim.SetBool("Picking Up Item", false);
				Destroy(transform.GetChild(2).gameObject);
			}
		}
		else if (!isAttacking) {
			input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

			if (!Mathf.Approximately(input.x, 0.0f) || !Mathf.Approximately(input.y, 0.0f)) {
				lookDirection.Set(input.x, input.y);
				lookDirection.Normalize();
			}

			anim.SetFloat("Look X", lookDirection.x);
			anim.SetFloat("Look Y", lookDirection.y);
			anim.SetFloat("Speed", input.magnitude);

			if (Input.GetAxis("Swap") > 0) {
				Swap();
			}
			else if (Input.GetAxis("Attack") > 0) {
				if ((isMeleeEquipped && melee != null) || (!isMeleeEquipped && ranged != null))
					Attack();
			}
		}
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if (!isPickingUpItem && !isKnockedBack && !isAttacking) {
			Vector2 position = rb.position;

			if (input.magnitude > 1)
				input.Normalize();

			position += speed * Time.deltaTime * input;

			rb.MovePosition(position);
		}
	}

	public void Swap() {
		isMeleeEquipped = !isMeleeEquipped;

		if (isMeleeEquipped && melee != null)
			transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = melee.sprite;
		else if (ranged != null)
			transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = ranged.sprite;
	}

	public bool PickUp(Transform item) {
		if (isAttacking)
			return false;

		item.position = new Vector3(transform.position.x, transform.position.y + pickUpDistance, transform.position.z);
		item.parent = gameObject.transform;

		if (item.GetComponent<Melee>() != null) {
			melee = item.GetComponent<Melee>().meleeData;
			transform.GetChild(0).GetComponent<Melee>().meleeData = melee;

			if (isMeleeEquipped)
				transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = melee.sprite;
		}
		else if (item.GetComponent<Ranged>() != null) {
			ranged = item.GetComponent<Ranged>().rangedData;
			transform.GetChild(0).GetComponent<Ranged>().rangedData = ranged;

			if (!isMeleeEquipped)
				transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = ranged.sprite;
		}

		pickUpTimer = pickUpTime;
		isPickingUpItem = true;
		anim.SetBool("Picking Up Item", true);

		return true;
	}
}
