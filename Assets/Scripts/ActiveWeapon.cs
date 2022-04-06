using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations;
using Photon.Pun;

public class ActiveWeapon : MonoBehaviour
{
    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1
    }
    public Transform crosshairTarget;
    public Transform[] weaponSlots;
    public Transform weaponLeftGrip;
    public Transform weaponRightGrip;
    public Animator rigController;
    public CharacterAiming characterAiming;

    RaycastWeapon currentWeapon;
    RaycastWeapon[] equippedWeapons = new RaycastWeapon[2];
    int activeWeaponIndex = -1;
    bool isHolstered;
    bool isAiming;
    CharacterLocomotion characterLocomotion;
    Animator characterAnimator;
    PhotonView photonView;

    int holsterWeaponHash = Animator.StringToHash("holster_weapon");
    int weaponIndexHash = Animator.StringToHash("weapon_index");
    int isAimingHash = Animator.StringToHash("isAiming");

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        characterAnimator = GetComponent<Animator>();
        characterLocomotion = GetComponent<CharacterLocomotion>();
        characterAiming = GetComponent<CharacterAiming>();
        currentWeapon = GetComponentInChildren<RaycastWeapon>();
        if (currentWeapon)
            Equip(currentWeapon);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (currentWeapon)
        {
            if (!isHolstered && !currentWeapon.isReloading && !characterLocomotion.IsSprinting)
            {
                currentWeapon.recoil.recoilModifier = isAiming ? 0.3f : 1;

                if (Input.GetMouseButtonDown(0))
                {
                    currentWeapon.StartFiring();
                }

                if (currentWeapon.IsFiring)
                {
                    currentWeapon.UpdateFiring(Time.deltaTime);
                    UIManager.Instance.RefreshAmmoUI(currentWeapon.ammoCount);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    currentWeapon.StopFiring();
                }
            }

            // holster weapon
            if (Input.GetKeyDown(KeyCode.X))
            {
                ToggleActiveWeapon();
            }

            // crosshair UI
            UIManager.Instance.ToggleCrosshair(!characterLocomotion.IsSprinting);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetActiveWeapon(WeaponSlot.Primary);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetActiveWeapon(WeaponSlot.Secondary);

        if (currentWeapon && (Input.GetKeyDown(KeyCode.R) || currentWeapon.ammoCount <= 0))
            if (currentWeapon.ammoCount < currentWeapon.clipSize)
                rigController.SetTrigger("reload_weapon");

        // aim
        isAiming = Input.GetMouseButton(1);
        characterAnimator.SetBool(isAimingHash, isAiming && !characterLocomotion.IsSprinting);
    }

    RaycastWeapon GetWeapon(int index)
    {
        if (index < 0 || index >= equippedWeapons.Length)
            return null;
        return equippedWeapons[index];
    }

    public RaycastWeapon GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }

    public void Equip(RaycastWeapon newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        var weapon = GetWeapon(weaponSlotIndex);
        if (weapon)
            return; // do nothing if pickup an already owned weapon

        weapon = newWeapon;
        weapon.raycastDestination = crosshairTarget;
        weapon.recoil.characterAiming = characterAiming;
        weapon.recoil.rigController = rigController;
        weapon.transform.SetParent(weaponSlots[weaponSlotIndex], false);
        equippedWeapons[weaponSlotIndex] = weapon;

        SetActiveWeapon(newWeapon.weaponSlot);
    }

    void ToggleActiveWeapon()
    {
        bool isHolstered = rigController.GetBool(holsterWeaponHash);
        if (isHolstered)
            StartCoroutine(ActivateWeapon(activeWeaponIndex));
        else StartCoroutine(HolsterWeapon(activeWeaponIndex));
    }

    void SetActiveWeapon(WeaponSlot weaponSlot)
    {
        int holsterIndex = activeWeaponIndex;
        int activateIndex = (int)weaponSlot;
        if (holsterIndex != activateIndex) // only switch different weapons
            StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }

    IEnumerator HolsterWeapon(int index)
    {
        isHolstered = true;
        var weapon = GetWeapon(index);
        if (weapon)
        {
            weapon.isHolstered = true;
            rigController.SetBool(holsterWeaponHash, true);
            do
            {
                yield return new WaitForEndOfFrame();
            } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        }
    }

    IEnumerator ActivateWeapon(int index)
    {
        var weapon = GetWeapon(index);
        if (weapon)
        {
            currentWeapon = weapon;
            rigController.SetBool(holsterWeaponHash, false);
            rigController.Play("equip_" + weapon.weaponName);

            do
            {
                yield return new WaitForEndOfFrame();
            } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
            isHolstered = false;
            weapon.isHolstered = false;
            UIManager.Instance.RefreshAmmoUI(weapon.ammoCount);
        }
    }

    IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        rigController.SetInteger(weaponIndexHash, activateIndex);
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex;
    }
}
