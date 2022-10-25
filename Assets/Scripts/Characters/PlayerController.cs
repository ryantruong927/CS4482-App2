using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller {
	private Vector2 input;
	private Vector2 lookDirection = new Vector2(1f, 0f);

	public float pickUpDistance = 2;
	public float pickUpTime = 1;
	private bool isPickingUpItem = false;
	private float pickUpTimer;

	private Melee melee;
	private Ranged ranged;

	protected override void Update() {
		base.Update();

		if (isPickingUpItem) {
			pickUpTimer -= Time.deltaTime;

			if (pickUpTimer <= 0) {
				isPickingUpItem = false;
				anim.SetBool("Picking Up Item", false);
				Destroy(transform.GetChild(0).gameObject);
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

			if (Input.GetAxis("Attack") > 0) {
				anim.SetBool("Attacking", true);
				isAttacking = true;
				attackTimer = attackTime;
			}
		}
	}

	private void FixedUpdate() {
		if (!isPickingUpItem && !isAttacking) {
			Vector2 position = rb.position;

			if (input.magnitude > 1)
				input.Normalize();

			position += speed * Time.deltaTime * input;

			rb.MovePosition(position);
		}
	}

	public override void ChangeHealth(int amount) {
		base.ChangeHealth(amount);

		if (amount < 0)
			anim.SetTrigger("Hit");
	}

	public bool PickUp(Transform item) {
		if (isAttacking)
			return false;

		item.position = new Vector3(transform.position.x, transform.position.y + pickUpDistance, transform.position.z);
		item.parent = gameObject.transform;

		pickUpTimer = pickUpTime;
		isPickingUpItem = true;
		anim.SetBool("Picking Up Item", true);
		return true;
	}
}
