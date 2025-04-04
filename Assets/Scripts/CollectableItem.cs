using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource;

    private void OnEnable()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            m_AudioSource.Play();
        }
    }

}
