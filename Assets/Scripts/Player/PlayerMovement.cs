using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementEventsContainer 
{
    public IMovableEvent OnMove;
    public IMovableEvent OnFall;
}


public class PlayerMovement : MonoBehaviour, IMovable
{
    private IMovableEvent OnMove;
    private MovementEventsContainer _movementEventsContainer;

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private bool _isFalling = true;
    [SerializeField] private Animator _animator;
    [SerializeField] private CapsuleCollider _collider;

    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private Vector3 _forwardDirection;

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _runningSpeed = 10f;

    [SerializeField] private float _angularSpeed = 5f;

    [SerializeField] private bool _isRunning = false;


    private RaycastHit _hit;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        _forwardDirection = transform.forward;
        _animator.SetBool("Falling", _isFalling);

        if(PlayerPrefs.GetInt("IsPlayerShouldRun") == 1)
        {
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }
    }

    private void FixedUpdate()
    {
        if (!_isFalling)
        {
            _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, Quaternion.LookRotation(_forwardDirection), _angularSpeed));
        }
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, Quaternion.LookRotation(_forwardDirection), _angularSpeed / 5f));


    }

    public void ChangeDirection(Vector3 direction)
    {
        if (!_isFalling)
            _forwardDirection = direction;
    }

    public void Move(float speedMultiplier)
    {
        if (!_isFalling)
        {
            float speed;
            if (_isRunning)
            {
                speed = _runningSpeed * speedMultiplier * Time.deltaTime;
                _animator.SetFloat("HorizontalSpeed", speedMultiplier * 2);
            }
            else
            {
                speed = _speed * speedMultiplier * Time.deltaTime;
                _animator.SetFloat("HorizontalSpeed", speedMultiplier);
            }

            var newPosition = _rb.position + speed * _forwardDirection;
            _rb.Move(newPosition, _rb.rotation);
            OnMove?.Invoke();
        }
        CheckIfFalling();
    }

    public void CheckIfFalling()
    {
        var isFalling = Mathf.Abs(_rb.linearVelocity.y) > 1f;
        var startToFall = isFalling && !_isFalling;
        var stopFalling = !isFalling && _isFalling;

        if (startToFall)
        {
            _isFalling = true;
            _forwardDirection = Vector3.forward;
            _movementEventsContainer.OnFall?.Invoke();
        }
        else if (stopFalling)
        {
            _isFalling = false;
        }

        _animator.SetBool("Falling", _isFalling);
    }

    public bool IsGrounded()
    {
        var position = _collider.bounds.center + Vector3.up;
        var direction = Vector3.down;

        var IsGrounded = Physics.Raycast(position, direction, out _hit, Mathf.Infinity);

        return IsGrounded;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    _isFalling = !IsGrounded();
    //    _animator.SetBool("Falling", _isFalling);
    //}

    public void ConnectToMoveEvent(IMovableEvent moveEvent)
    {
        OnMove += moveEvent;
    }

    public void DisconnectFromMoveEvent(IMovableEvent moveEvent)
    {
        if(OnMove != null)
            OnMove -= moveEvent;
    }

    public MovementEventsContainer GetMovementEventsContainer()
    {
        if (_movementEventsContainer == null)
            _movementEventsContainer = new MovementEventsContainer();
        return _movementEventsContainer;
    }
}
