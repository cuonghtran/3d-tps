using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector] public CharacterAiming characterAiming;
    [HideInInspector] public CinemachineImpulseSource cameraShake;
    [HideInInspector] public Animator rigController;

    public float duration;
    public Vector2[] recoilPatterns;
    public float recoilModifier = 1;

    float verticalRecoil;
    float horizontalRecoil;
    int index;
    float time;
    Camera mainCamera;

    private void Awake()
    {
        cameraShake = GetComponent<CinemachineImpulseSource>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if ( time > 0)
        {
            characterAiming.yAxis.Value -= (verticalRecoil/10) * Time.deltaTime / duration * recoilModifier;
            characterAiming.xAxis.Value -= (horizontalRecoil / 10) * Time.deltaTime / duration * recoilModifier;
            time -= Time.deltaTime;
        }
    }

    int GetNextIndex(int index)
    {
        int rng = Random.Range(0, recoilPatterns.Length);
        return rng;
    }

    public void GenerateRecoil(string weaponName)
    {
        time = duration;
        cameraShake.GenerateImpulse(mainCamera.transform.forward);
        horizontalRecoil = recoilPatterns[index].x;
        verticalRecoil = recoilPatterns[index].y;
        index = GetNextIndex(index);

        rigController.Play("weapon_recoil_" + weaponName, 1, 0f);
    }

    public void ResetIndex()
    {
        index = 0;
    }
}
