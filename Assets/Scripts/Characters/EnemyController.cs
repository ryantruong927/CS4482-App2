using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Controller {
	private Vector2 lastSeenPosition;
	private bool isChasing;
	public float chaseSpeed = 2.5f;
	public float chaseTime = 2;
	private float chaseTimer;

	public float changeTime = 2;
	private float changeTimer;

	public bool isGoingUpAndDown = false;

	public int damage = -1;

	protected override void Start() {
		base.Start();

		changeTimer = changeTime;

		if (isGoingUpAndDown)
			anim.SetFloat("Move X", 0f);
		else
			anim.SetFloat("Move Y", 0f);
	}

	protected override void Update() {
			base.Update();

			if (isChasing) {
				if (chaseTimer > 0)
					chaseTimer -= Time.deltaTime;
				else {
					// if done chasing, restart the change cycle

					isChasing = false;
					changeTimer = changeTime;

					if (isGoingUpAndDown)
						anim.SetFloat("Move X", 0f);
					else
						anim.SetFloat("Move Y", 0f);
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

	private void FixedUpdate() {
			Vector2 position = rb.position;

			if (isChasing) {
				// change the direction the enemy is facing based on its position relative to the player
				// in other words, make the enemy face the player correctly given their speed in toward a direction
				if (Mathf.Abs(lastSeenPosition.x - position.x) > Mathf.Abs(lastSeenPosition.y - position.y)) {
					anim.SetFloat("Move X", position.x < lastSeenPosition.x ? 1f : -1f);
					anim.SetFloat("Move Y", 0f);
				}
				else {
					anim.SetFloat("Move X", 0f);
					anim.SetFloat("Move Y", position.y < lastSeenPosition.y ? 1f : -1f);
				}

				// move enemy toward the player's position
				position = Vector2.MoveTowards(rb.position, lastSeenPosition, Mathf.Abs(chaseSpeed) * Time.deltaTime);
				rb.MovePosition(position);
			}
			else {
				if (isGoingUpAndDown) {
					anim.SetFloat("Move Y", speed);
					position.y += speed * Time.deltaTime;
				}
				else {
					anim.SetFloat("Move X", speed);
					position.x += speed * Time.deltaTime;
				}

				rb.MovePosition(position);
			}
	}

	private void OnTriggerStay2D(Collider2D collision) {
			PlayerController player = collision.gameObject.GetComponent<PlayerController>();

			// if player entered the FOV
			if (player != null) {
				// get positions of enemy and player
				Vector2 position = rb.position;
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
