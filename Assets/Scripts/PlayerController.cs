using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IController
{
    private IMovable _movement;
    private PlayerInput _playerInput;

    private readonly Vector3[] _moveDirections =
    {
        Vector3.right,
        Vector3.forward,
    };

    private int _currentDirectionIndex = 0;

    public void OnEnable()
    {
        _movement = GetComponent<IMovable>();
        _playerInput = GetComponent<PlayerInput>();

        _playerInput.OnClick += ChangeDirection;

        ChangeDirection();
    }

    public void Update()
    {
        _movement?.Move(1f);
    }

    public void ChangeDirection()
    {
        _currentDirectionIndex = (_currentDirectionIndex + 1) % 2;

        var newDirection = _moveDirections[_currentDirectionIndex];
        _movement?.ChangeDirection(newDirection);
    }

    private void OnDestroy()
    {
        if(_playerInput != null)
            _playerInput.OnClick -= ChangeDirection;
    }
}
