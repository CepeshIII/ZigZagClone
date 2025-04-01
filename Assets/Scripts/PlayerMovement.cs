using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour, IMovable
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] CapsuleCollider collider;

    private RaycastHit hit;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider>();
    }

    public void ChangeDirection(Vector3 direction)
    {
        rb.MoveRotation(Quaternion.LookRotation(direction));
    }

    public void Move(float speedMultiplier)
    {
        var newPosition = rb.position + speed * speedMultiplier * Time.deltaTime * transform.forward;
        rb.Move(newPosition, rb.rotation);

        animator.SetFloat("HorizontalSpeed", speedMultiplier);

        animator.SetBool("Falling", Mathf.Abs(rb.linearVelocity.y) > 1f);
    }

    public bool IsGrounded()
    {
        var position = collider.bounds.center;
        var direction = Vector3.down;

        var IsGrounded = Physics.Raycast(position, direction, out hit, Mathf.Infinity, groundLayerMask);
        return IsGrounded;
    }

}
