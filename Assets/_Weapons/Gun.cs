using UnityEngine;

public enum fireRateType { Auto, Semi, OneShot}

[CreateAssetMenu(fileName = "NewGun", menuName = "Weapons/Gun")]
public class Gun : Weapon
{
    [Header("   Gun Type Settings")]
    public float fireRate;
    public fireRateType rateType;
    public int bulletsByShot = 1;
    public float bulletsFireRate;

    [Header("   General Settings")]
    public float maxDistance;
    public int damage;
    public int clipSize;
    public int reservedAmmoCapacity;
}
