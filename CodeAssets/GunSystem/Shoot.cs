using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Shoot : MonoBehaviour
{
    #region Variables

    private WeaponSettings weaponSettingsScript = null;
    private WeaponSwitching weaponSwitchingScript = null;
    [SerializeField] private GameObject firePoint = null;
    private GameObject currentBullet;

    #endregion

    private void Start()
    {
        weaponSettingsScript = GetComponentInParent<WeaponSettings>();
        weaponSwitchingScript = GetComponentInParent<WeaponSwitching>();
    }


    private void Update()
    {
        shootBullet();
    }

    private void shootBullet()
    {
        int currentWeapon = weaponSwitchingScript.currentWeapon;

        switch (currentWeapon)
        {
            case 0:
                currentBullet = weaponSettingsScript.pistolBullet;
                break;
            case 1:
                currentBullet = weaponSettingsScript.assaultRifleBullet;
                break;
            case 2:
                currentBullet = weaponSettingsScript.shotgunShell;
                break;
            case 3:
                currentBullet = weaponSettingsScript.SMGBullet;
                break;
            case 4:
                currentBullet = weaponSettingsScript.sniperBullet;
                break;
        }

        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(currentBullet, firePoint.transform.position, firePoint.transform.rotation);
            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
            bulletRB.AddForce(transform.forward * weaponSettingsScript.bulletVelocity);
        }
    }

    private void ZoomWeapon()
    {

    }
}
