using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller {
	private Vector2 input;
	private Vector2 lookDirection = new Vector2(1f, 0f);

	protected override void Update() {
		base.Update();

		input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		if (!Mathf.Approximately(input.x, 0.0f) || !Mathf.Approximately(input.y, 0.0f)) {
			lookDirection.Set(input.x, input.y);
			lookDirection.Normalize();
		}

		animator.SetFloat("Look X", lookDirection.x);
		animator.SetFloat("Look Y", lookDirection.y);
		animator.SetFloat("Speed", input.magnitude);
	}

	private void FixedUpdate() {
		Vector2 position = rigidbody2D.position;
		position += speed * Time.deltaTime * input;

		rigidbody2D.MovePosition(position);
	}

	public override void ChangeHealth(int amount) {
		base.ChangeHealth(amount);

		if (amount < 0)
			animator.SetTrigger("Hit");
	}
}
