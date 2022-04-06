using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    [Header("FX")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;

    [Header("Information")]
    public Transform raycastOrigin;
    public Transform raycastDestination;
    public string weaponName;
    public ActiveWeapon.WeaponSlot weaponSlot;
    public WeaponRecoil recoil;

    [Header("Behaviors")]
    public GameObject magazine;
    public int ammoCount;
    public int clipSize;
    [HideInInspector] public bool isHolstered;
    [HideInInspector] public bool isReloading;

    bool isFiring = false;
    public bool IsFiring { get { return isFiring; } }
    public float fireRate = 20;
    float accumulatedTime = 0;

    Ray ray;
    RaycastHit hitInfo;

    private void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
        ammoCount = clipSize;
    }

    public void InitAmmo()
    {
        ammoCount = clipSize;
    }

    public void StartFiring()
    {
        isFiring = true;
        accumulatedTime = 0;
        recoil.ResetIndex();
    }

    public void UpdateFiring(float deltaTime)
    {
        accumulatedTime += deltaTime;
        float fireInterval = 1f / fireRate;
        while (accumulatedTime >= 0f)
        {
            FireBullets();
            accumulatedTime -= fireInterval;
        }
    }

    void FireBullets()
    {
        // Ammo
        if (ammoCount <= 0)
            return;
        ammoCount--;

        // Fire
        muzzleFlash.Emit(1);

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;

        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        if (Physics.Raycast(ray, out hitInfo))
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            // Collision impulse
            var rb = hitInfo.collider.GetComponent<Rigidbody>();
            if (rb)
                rb.AddForceAtPosition(ray.direction * 20, hitInfo.point, ForceMode.Impulse);

            tracer.transform.position = hitInfo.point;
        }

        recoil.GenerateRecoil(weaponName);
    }

    public void StopFiring()
    {
        isFiring = false;
    }

    public void ReloadAmmo()
    {
        ammoCount = clipSize;
    }
}
