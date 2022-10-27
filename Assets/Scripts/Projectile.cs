using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	public bool hasFired = false;
	private string owner;
	private RangedData rangedData;
	private int remainingPierce;
	private Vector2 startPosition;

	private Rigidbody2D rb;

	public void Create(string owner, RangedData rangedData, Vector2 lookDirection) {
		this.owner = owner;
		this.rangedData = rangedData;

		rb = GetComponent<Rigidbody2D>();
		GetComponent<SpriteRenderer>().sprite = rangedData.projectileSprite;

		remainingPierce = rangedData.pierce;
		startPosition = rb.position;
		rb.velocity = (lookDirection * rangedData.speed);
		hasFired = true;
	}

	private void Update() {
		if (hasFired) {
			if ((rb.position - startPosition).magnitude >= rangedData.range)
				Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.layer == 8) {
			rb.velocity = Vector2.zero;
			Destroy(GetComponent<CapsuleCollider2D>());
		}
		else if (!collider.CompareTag(owner) && (collider.GetType() != typeof(CircleCollider2D))) {
			Controller character = collider.GetComponent<Controller>();

			if (character != null) {
				character.Hit(rangedData.dmg);
				remainingPierce--;

				if (remainingPierce < 0)
					Destroy(gameObject);
			}
		}
	}
}
