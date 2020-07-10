using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSettings : MonoBehaviour
{
    [SerializeField] public int bulletVelocity;

    [Header("Weapon statistics")]
    [SerializeField] public int assaultRifleDamage;
    [SerializeField] public float assaultRifleFireRate;

    [Header("Bullet prefabs")]
    [SerializeField] public GameObject pistolBullet = null;
    [SerializeField] public GameObject assaultRifleBullet = null;
    [SerializeField] public GameObject shotgunShell = null;
    [SerializeField] public GameObject SMGBullet = null;
    [SerializeField] public GameObject sniperBullet = null;
}
