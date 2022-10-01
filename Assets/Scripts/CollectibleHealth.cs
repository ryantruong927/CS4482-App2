using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleHealth : MonoBehaviour {
	public int health = 1;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.GetType() == typeof(BoxCollider2D)) {
			Controller controller = collision.GetComponent<Controller>();

			if (controller != null) {
				if (controller.Health < controller.maxHealth) {
					controller.ChangeHealth(health);
					Destroy(gameObject);
				}
			}
		}
	}
}
