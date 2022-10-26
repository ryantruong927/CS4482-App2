using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "New Melee Weapon", menuName = "Weapon/Melee Weapon")]
public class MeleeData : WeaponData {
	public int knockback;
}
