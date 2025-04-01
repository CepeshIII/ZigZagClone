using UnityEngine;

internal interface ICollidable
{
    public void OnCollisionEnter(Collision collision);

    public bool CheckCollision();
}