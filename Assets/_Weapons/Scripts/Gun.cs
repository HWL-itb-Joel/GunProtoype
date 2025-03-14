using UnityEngine;

public enum fireRateType { Auto, Semi, OneShot, OverHeating}

[CreateAssetMenu(fileName = "NewGun", menuName = "Weapons/Gun")]
public class Gun : Weapon
{
    [Header("   Gun Type Settings")]
    public float fireRate;
    public fireRateType rateType;
    public int bulletsByShot = 1;
    public bool sprayBullets = false;
    public float sprayIndicator;
    public float bulletsFireRate;

    [Header("   General Settings")]
    public float maxDistance;
    public int damage;
    public int clipSize;
    public int reservedAmmoCapacity;

    [Header("   Alternative Gun Settings")]
    public bool alternativeShoot = false;
    public fireRateType alternativeType;
    public int alternativeBulletsByShoot = 1;
    public bool alternativeSpray = false;
    public float alternativeSprayIndicator;
    public int alternativeDamage;
    public float alternativeFireRate;

    [Header("   OverHeating Settings")]
    public float heatThreshold;
    public float currentHeat;
    public float overheatCooldown;
    public bool isOverheated = false;

    public float baseCoolingRate;
    public float maxCoolingRate;
    public float coolingAccelerationTime;
    public float coolingMultiplier = 0f;
    public bool isCooling = false;


}
