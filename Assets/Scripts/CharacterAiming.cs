using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class CharacterAiming : MonoBehaviour
{
    [Header("Aiming")]
    public float turnSpeed = 15;
    public float aimDuration = 0.2f;

    [Header("Camera")]
    public Transform cameraLookAt;
    public AxisState xAxis;
    public AxisState yAxis;

    public Camera mainCamera;

    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (!photonView.IsMine)
        //    return;

        // rotate character
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        // rotate camera
        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);
        cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);
    }
}
