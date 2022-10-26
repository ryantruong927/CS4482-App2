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

		if (isGoingUpAndDown) {
			lookDirection = new Vector2(1, 0);
			anim.SetFloat("Look X", 0);
		}
		else {
			lookDirection = new Vector2(0, 1);
			anim.SetFloat("Look Y", 0);
		}
	}

	protected override void Update() {
		base.Update();

		if (!isKnockedBack) {
			if (isChasing) {
				if (chaseTimer > 0)
					chaseTimer -= Time.deltaTime;
				else {
					// if done chasing, restart the change cycle

					isChasing = false;
					changeTimer = changeTime;

					if (isGoingUpAndDown) {
						lookDirection = new Vector2(1, 0);
						anim.SetFloat("Look X", 0);
					}
					else {
						lookDirection = new Vector2(0, 1);
						anim.SetFloat("Look Y", 0);
					}
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

			anim.SetFloat("Speed", Mathf.Abs(speed));
		}
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if (!isKnockedBack && !isAttacking) {
			Vector2 position = rb.position;

			if (isChasing) {
				// change the direction the enemy is facing based on its position relative to the player
				// in other words, make the enemy face the player correctly given their speed in toward a direction
				if (Mathf.Abs(lastSeenPosition.x - position.x) > Mathf.Abs(lastSeenPosition.y - position.y)) {
					lookDirection = new Vector2(position.x < lastSeenPosition.x ? 1 : -1, 0);
				}
				else {
					lookDirection = new Vector2(0, position.y < lastSeenPosition.y ? 1 : -1);
				}

				anim.SetFloat("Look X", lookDirection.x);
				anim.SetFloat("Look Y", lookDirection.y);

				// move enemy toward the player's position
				position = Vector2.MoveTowards(rb.position, lastSeenPosition, Mathf.Abs(chaseSpeed) * Time.deltaTime);
				rb.MovePosition(position);

				if (isMeleeEquipped) {
					if ((lastSeenPosition - position).magnitude <= melee.range) {
						Attack();
					}
				}
				else {
					if ((lastSeenPosition - position).magnitude <= ranged.range) {
						Attack();
					}
				}
			}
			else {
				if (isGoingUpAndDown) {
					lookDirection.y = speed;
					lookDirection.Normalize();
					anim.SetFloat("Look Y", lookDirection.y);
					position.y += speed * Time.deltaTime;
				}
				else {
					lookDirection.x = speed;
					lookDirection.Normalize();
					anim.SetFloat("Look X", lookDirection.x);
					position.x += speed * Time.deltaTime;
				}

				rb.MovePosition(position);
			}
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
