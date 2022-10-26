using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "New Ranged Weapon", menuName = "Weapon/Ranged Weapon")]
public class RangedData : WeaponData {
	public GameObject projectilePrefab;
	public Sprite projectileSprite;
	public float chargeTime;
	public int pierce;
}
