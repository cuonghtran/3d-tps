using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{
    public Animator rigController;
    public WeaponAnimationEvents animationEvents;
    public Transform leftHand;

    ActiveWeapon activeWeapon;
    GameObject magazineHand;
    RaycastWeapon currentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        animationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
        activeWeapon = GetComponent<ActiveWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        //currentWeapon = activeWeapon.GetActiveWeapon();
        //if (currentWeapon)
        //{
        //    if (Input.GetKeyDown(KeyCode.R) || currentWeapon.ammoCount <=0)
        //        if (currentWeapon.ammoCount < currentWeapon.clipSize)
        //            rigController.SetTrigger("reload_weapon");
        //}
    }

    void OnAnimationEvent(string eventName)
    {
        currentWeapon = activeWeapon.GetActiveWeapon();

        switch (eventName)
        {
            case "start_reloading":
                currentWeapon.isReloading = true;
                break;
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillhMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
            case "end_reloading":
                currentWeapon.isReloading = false;
                break;
        }
    }

    void DetachMagazine()
    {
        currentWeapon.isReloading = true;
        magazineHand = Instantiate(currentWeapon.magazine, leftHand, true);
        currentWeapon.magazine.SetActive(false);
    }

    void DropMagazine()
    {
        GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        magazineHand.SetActive(false);
        Destroy(droppedMagazine, 1.75f);
    }

    void RefillhMagazine()
    {
        magazineHand.SetActive(true);
    }

    void AttachMagazine()
    {
        currentWeapon.magazine.SetActive(true);
        Destroy(magazineHand);
        currentWeapon.ReloadAmmo();
        UIManager.Instance.RefreshAmmoUI(currentWeapon.ammoCount);
        rigController.ResetTrigger("reload_weapon");
    }
}
