using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IController
{
    private IMovable _movement;

    private readonly Vector3[] _moveDirections =
    {
        Vector3.right,
        Vector3.forward,
    };

    private int _currentDirectionIndex = 0;

    public void OnEnable()
    {
        _movement = GetComponent<IMovable>();
    }

    public void Update()
    {
        _movement?.Move(Input.GetAxis("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeDirection();
        }
    }

    public void ChangeDirection()
    {
        _currentDirectionIndex = (_currentDirectionIndex + 1) % 2;

        var newDirection = _moveDirections[_currentDirectionIndex];
        _movement?.ChangeDirection(newDirection);
    }
}
