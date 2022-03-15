using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private float moveForce;
    [SerializeField] private Animator animator;

    void Update()
    {
        rigidbody.velocity = new Vector3(joystick.Horizontal * moveForce, rigidbody.velocity.y, joystick.Vertical * moveForce);

        if (rigidbody.velocity.magnitude > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z)),
                Time.deltaTime * 100f
            );

            animator.SetBool("run", true);
        }
        else
        {
            animator.SetBool("run", false);
        }
    }
}
