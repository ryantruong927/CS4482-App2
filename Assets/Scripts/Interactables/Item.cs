using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	public GameObject[] obstacles;

	protected virtual void OnTriggerEnter2D(Collider2D collision) {
		PlayerController player = collision.GetComponent<PlayerController>();

		if (player != null) {
			if (player.PickUp(transform.parent.GetChild(0))) {
				foreach (GameObject obstacle in obstacles) {
					if (obstacle != null)
						Destroy(obstacle);
				}

				Destroy(transform.parent.GetComponent<Animator>());
				Destroy(transform.parent.GetChild(0).gameObject);
				Destroy(GetComponent<CircleCollider2D>());
			}
		}
	}
}
