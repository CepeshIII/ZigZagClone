using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour, IMovable
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed;
    [SerializeField] private float _angularSpeed;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Vector3 _forwardDirection;

    private RaycastHit _hit;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, Quaternion.LookRotation(_forwardDirection), _angularSpeed));
    }

    public void ChangeDirection(Vector3 direction)
    {
        _forwardDirection = direction;
    }

    public void Move(float speedMultiplier)
    {
        var newPosition = _rb.position + _speed * speedMultiplier * Time.deltaTime * _forwardDirection;
        _rb.Move(newPosition, _rb.rotation);

        _animator.SetFloat("HorizontalSpeed", speedMultiplier);

        _animator.SetBool("Falling", Mathf.Abs(_rb.linearVelocity.y) > 1f);
    }

    public bool IsGrounded()
    {
        var position = _collider.bounds.center;
        var direction = Vector3.down;

        var IsGrounded = Physics.Raycast(position, direction, out _hit, Mathf.Infinity, _groundLayerMask);
        return IsGrounded;
    }

}
