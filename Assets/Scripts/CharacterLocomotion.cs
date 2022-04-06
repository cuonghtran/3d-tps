using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterLocomotion : MonoBehaviour
{
    [Header("References")]
    public Animator rigController;

    [Header("Information")]
    public float jumpHeight;
    public float gravity;
    public float airControl;
    public float jumpDamp;
    public float runMultiplier;
    public float pushPower;

    Animator animator;
    CharacterController characterController;
    PhotonView photonView;
    Vector2 input;
    Vector3 velocity;

    public bool IsJumping { get; private set; }
    public bool IsSprinting { get; private set; }

    int runMultiplierHash = Animator.StringToHash("runMultiplier");
    int inPutXHash = Animator.StringToHash("InputX");
    int inPutYHash = Animator.StringToHash("InputY");
    int isJumpingHash = Animator.StringToHash("isJumping");
    int isSprintingParam = Animator.StringToHash("isSprinting");

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!photonView.IsMine || !photonView.AmOwner)
        //    return;

        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        animator.SetFloat(runMultiplierHash, runMultiplier);
        animator.SetFloat(inPutXHash, input.x);
        animator.SetFloat(inPutYHash, input.y);

        UpdateIsSprinting();

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void FixedUpdate()
    {
        //if (!photonView.IsMine)
        //    return;

        if (IsJumping)
        {
            UpdateInAir();
        }
        else
        {
            UpdateOnGround();
        }
    }

    void UpdateIsSprinting()
    {
        IsSprinting = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool(isSprintingParam, IsSprinting);
        rigController.SetBool(isSprintingParam, IsSprinting);
    }

    private void UpdateInAir()
    {
        velocity.y -= gravity * Time.fixedDeltaTime;
        Vector3 displacement = velocity * Time.fixedDeltaTime;
        displacement += CalculateAirControl();
        displacement.x *= IsSprinting ? jumpDamp + 0.45f : jumpDamp;
        displacement.z *= IsSprinting ? jumpDamp + 0.45f : jumpDamp;
        characterController.Move(displacement);
        IsJumping = !characterController.isGrounded;
        animator.SetBool(isJumpingHash, IsJumping);
    }

    void UpdateOnGround()
    {
        //if (!characterController.isGrounded)
        //{
        //    IsJumping = true;
        //    animator.SetBool(isJumpingHash, true);
        //}
    }

    void Jump()
    {
        if (!IsJumping)
        {
            IsJumping = true;
            velocity.y = Mathf.Sqrt(2 * gravity * jumpHeight);
            animator.SetBool(isJumpingHash, true);
        }
    }

    Vector3 CalculateAirControl()
    {
        return ((transform.forward * input.y) + (transform.right * input.x)) * (airControl / 100);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        body.velocity = pushDir * pushPower;
    }
}
