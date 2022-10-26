using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour {
	public MeleeData meleeData;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (transform.parent.GetComponent<Controller>().isMeleeEquipped) {
			Controller character = collision.GetComponent<Controller>();

			if (character != null && character.transform != transform.parent) {
				character.Hit(meleeData.dmg, meleeData.knockback, transform.parent.position);
			}
		}
	}
}
