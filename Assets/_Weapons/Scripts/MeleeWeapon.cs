using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeWeapon", menuName = "Weapons/Melee")]
public class MeleeWeapon : ScriptableObject
{
    public int damage;
    public float attackSpeed;
}
