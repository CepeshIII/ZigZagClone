using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public static class RequiredAudioMethods
{
    public static void PlayResourcesAtPoint(AudioResource audioResource, Vector3 position, 
        AudioMixerGroup audioMixerGroup = null, float volume = 1f)
    {
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.resource = audioResource;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play();

        DestroyAfterPlay(audioSource);
    }

    public static IEnumerator DestroyAfterPlay(AudioSource audioSource)
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        Object.Destroy(audioSource.gameObject);
    }
}
