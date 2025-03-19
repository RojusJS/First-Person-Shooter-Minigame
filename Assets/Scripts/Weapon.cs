using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 0.2f;

    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    public float spreadIntensity;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 500;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    internal Animator animator;

    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    public enum WeaponModel
    {
        Pistol1911, 
        M16
    }

    public WeaponModel thisWeaponModel;


    //Naudojamos bitines operacijos - 1 t
    [Flags]
    public enum ShootingMode
    {
        None = 0,
        Single = 1 << 0, 
        Burst = 1 << 1, 
        Auto = 1 << 2  
    }

    public ShootingMode currentShootingMode = ShootingMode.Auto;

    public void ToggleShootingMode(ShootingMode mode)
    {
        currentShootingMode ^= mode;
        Debug.Log($"Current Shooting Mode: {currentShootingMode}");
    }

    //Naudojate 'switch' su 'when' raktazodziu - 0.5 t
    public void SetShootingMode(ShootingMode mode)
    {
        switch (mode)
        {
            case ShootingMode.Single when bulletsLeft > 0:
                Debug.Log("Single shot mode active.");
                break;
            case ShootingMode.Burst when bulletsLeft > 5:
                Debug.Log("Burst mode active.");
                break;
            case ShootingMode.Auto when bulletsLeft > 0:
                Debug.Log("Auto mode active.");
                break;
            default:
                Debug.Log("Cannot switch mode: not enough bullets.");
                break;
        }
    }


    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;
    }

    void Update()
    {
        if (isActiveWeapon)
        {
            GetComponent<Outline>().enabled = false;

            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyMagazineSound1911.Play();
            }


            if (Input.GetKeyDown(KeyCode.J)) ToggleShootingMode(ShootingMode.Single);
            if (Input.GetKeyDown(KeyCode.K)) ToggleShootingMode(ShootingMode.Burst);
            if (Input.GetKeyDown(KeyCode.L)) ToggleShootingMode(ShootingMode.Auto);

            isShooting = currentShootingMode switch
            {
                ShootingMode.Auto => Input.GetKey(KeyCode.Mouse0),
                ShootingMode.Single or ShootingMode.Burst => Input.GetKeyDown(KeyCode.Mouse0),
                _ => false
            };

            // Naudojamas operatorius 'is' - 0.5 t.
            if (currentShootingMode is ShootingMode.Auto)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                animator.SetTrigger("INSPECT");
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false)
            {
                Reload();
            }

            if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0)
            {
                Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            if (AmmoManager.Instance.ammoDisplay != null)
            {
                //
                AmmoManager.Instance?.ammoDisplay?.SetText($"{bulletsLeft / bulletsPerBurst}/{magazineSize / bulletsPerBurst}");
            } 
        }
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

        //SoundManager.Instance.shootingSound1911.Play();

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }

    }

    private void Reload()
    {
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);

        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
