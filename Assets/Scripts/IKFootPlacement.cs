using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootPlacement : MonoBehaviour
{
    Animator animator;
    [Range (0, 1f)] public float distanceToGround;
    public LayerMask layerMask;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, animator.GetFloat("IKLeftFootWeight"));
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, animator.GetFloat("IKLeftFootWeight"));
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, animator.GetFloat("IKRightFootWeight"));
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, animator.GetFloat("IKRightFootWeight"));
            //animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            //animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            //animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            //animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

            // left foot
            RaycastHit hit;
            Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, distanceToGround + 1f, layerMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += distanceToGround;
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation); // Quaternion.LookRotation(transform.forward, hit.normal)
                }
            }

            // right foot
            ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, distanceToGround + 1f, layerMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += distanceToGround;
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation); // Quaternion.LookRotation(transform.forward, hit.normal)
                }
            }
        }
    }
}
