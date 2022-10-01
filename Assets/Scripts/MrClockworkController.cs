using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrClockworkController : Controller {
	public bool isBroken = true;

	private Vector2 lastSeenPosition;
	private bool isChasing;
	public float chaseSpeed = 2.5f;
	public float chaseTime = 2f;
	private float chaseTimer;

	public float changeTime = 2f;
	private float changeTimer;

	public bool isGoingUpAndDown = false;

	public int damage = -1;

	protected override void Start() {
		base.Start();

		changeTimer = changeTime;

		if (isGoingUpAndDown)
			animator.SetFloat("Move X", 0f);
		else
			animator.SetFloat("Move Y", 0f);
	}

	protected override void Update() {
		if (isBroken) {
			base.Update();

			if (isChasing) {
				if (chaseTimer > 0)
					chaseTimer -= Time.deltaTime;
				else {
					// if done chasing, restart the change cycle

					isChasing = false;
					changeTimer = changeTime;

					if (isGoingUpAndDown)
						animator.SetFloat("Move X", 0f);
					else
						animator.SetFloat("Move Y", 0f);
				}
			}
			else {
				if (changeTimer > 0)
					changeTimer -= Time.deltaTime;
				else {
					changeTimer = changeTime;
					speed *= -1;
				}
			}
		}
	}

	private void FixedUpdate() {
		if (isBroken) {
			Vector2 position = rigidbody2D.position;

			if (isChasing) {
				// change the direction the enemy is facing based on its position relative to the player
				// in other words, make the enemy face the player correctly given their speed in toward a direction
				if (Mathf.Abs(lastSeenPosition.x - position.x) > Mathf.Abs(lastSeenPosition.y - position.y)) {
					animator.SetFloat("Move X", position.x < lastSeenPosition.x ? 1f : -1f);
					animator.SetFloat("Move Y", 0f);
				}
				else {
					animator.SetFloat("Move X", 0f);
					animator.SetFloat("Move Y", position.y < lastSeenPosition.y ? 1f : -1f);
				}

				// move enemy toward the player's position
				position = Vector2.MoveTowards(rigidbody2D.position, lastSeenPosition, Mathf.Abs(chaseSpeed) * Time.deltaTime);
				rigidbody2D.MovePosition(position);
			}
			else {
				if (isGoingUpAndDown) {
					animator.SetFloat("Move Y", speed);
					position.y += speed * Time.deltaTime;
				}
				else {
					animator.SetFloat("Move X", speed);
					position.x += speed * Time.deltaTime;
				}

				rigidbody2D.MovePosition(position);
			}
		}
	}

	public void Fix() {
		isBroken = false;
		rigidbody2D.simulated = false;
		animator.SetTrigger("Fixed");
	}

	private void OnCollisionStay2D(Collision2D collision) {
		if (isBroken) {
			RubyController player = collision.gameObject.GetComponent<RubyController>();

			if (player != null)
				player.ChangeHealth(damage);
		}
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (isBroken) {
			RubyController player = collision.gameObject.GetComponent<RubyController>();

			// if player entered the FOV
			if (player != null) {
				// get positions of enemy and player
				Vector2 position = rigidbody2D.position;
				lastSeenPosition = collision.gameObject.GetComponent<Rigidbody2D>().position;

				// check if the player is in front of the enemy
				if (!isChasing) {
					if (speed > 0) {
						if (isGoingUpAndDown && (lastSeenPosition.y < position.y))
							return;
						else if (lastSeenPosition.x < position.x)
							return;
					}
					else if (isGoingUpAndDown && (lastSeenPosition.y > position.y))
						return;
					else if (lastSeenPosition.x > position.x)
						return;
				}

				isChasing = true;

				// keep restarting the timer as long as the player is seen
				chaseTimer = chaseTime;
			}
		}
	}
}
