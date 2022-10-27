using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour {
	public MeleeData meleeData;

	private void OnTriggerEnter2D(Collider2D collider) {
		if (collider.GetType() != typeof(CircleCollider2D)) {
			Controller character = collider.GetComponent<Controller>();

			if (character != null && character.transform != transform.parent)
				character.Hit(meleeData.dmg);
			//character.Hit(meleeData.dmg, meleeData.knockback, transform.parent.position);
		}
	}
}
