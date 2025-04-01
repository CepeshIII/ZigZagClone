using System;
using UnityEngine;

internal interface IMovable
{
    public void Move(float speed);
    public void ChangeDirection(Vector3 direction);
}