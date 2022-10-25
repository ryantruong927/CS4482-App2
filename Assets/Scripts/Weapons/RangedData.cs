using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "New Ranged Weapon", menuName = "Weapon/Ranged Weapon")]
public class RangedData : WeaponData {
	public Sprite projectile;
	public float chargeTime;
	public float pierce;
}
