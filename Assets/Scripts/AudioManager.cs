using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-------------- Audio Source --------------")]
    [SerializeField] private AudioSource musicSource;

    [Header("-------------- Audio Clip --------------")]
    public AudioClip background;

    void Start()
    {
      musicSource.clip = background;
      musicSource.Play();  
    }
}
