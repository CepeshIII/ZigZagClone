using System;
using System.Collections;
using UnityEngine;

public class GameObjectDestroyer : MonoBehaviour
{
    public void Innit<T>(Predicate<T> condition, T object1)
    {
        StartCoroutine(DestroyAfter(condition, object1));
    }

    public IEnumerator DestroyAfter<T>(Predicate<T> condition, T object1)
    {
        while (condition.Invoke(object1))
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
