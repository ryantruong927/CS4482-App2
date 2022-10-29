using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {
	public int dmg = 5;

	private void OnTriggerStay2D(Collider2D collision) {
		Controller character = collision.GetComponent<Controller>();

		if (character != null)
			character.Hit(dmg);
	}
}
