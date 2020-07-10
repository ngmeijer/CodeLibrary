using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private WeaponSettings weaponSettings = null;
    private Rigidbody rb = null;
    private int bulletDamage;

    private void Awake()
    {
        weaponSettings = FindObjectOfType<WeaponSettings>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        bulletDamage = weaponSettings.assaultRifleDamage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
