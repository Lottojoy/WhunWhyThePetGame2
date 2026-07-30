using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GeneralSettingManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;

    void Start()
    {
        audioMixer.SetFloat("music", Mathf.Log10(musicSlider.value) * 20);
        musicSlider.onValueChanged.AddListener((volume) =>
        {
            audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        });
    }
}
