using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;        // ชื่อเรียกเสียง เช่น "Click", "Buy", "Pop"
    public AudioClip clip;     // ไฟล์เสียง
    [Range(0f, 1f)]
    public float volume = 1f;  // ความดัง
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource; // ลาก AudioSource สำหรับเพลงฉากมาใส่
    public AudioSource sfxSource; // ลาก AudioSource สำหรับเสียงเอฟเฟกต์มาใส่

    [Header("Audio Clips")]
    public Sound[] bgmSounds;
    public Sound[] sfxSounds;

    private Dictionary<string, Sound> sfxDictionary = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> bgmDictionary = new Dictionary<string, Sound>();

    private void Awake()
    {
        // ทำเป็น Singleton และให้อยู่ข้าม Scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // นำข้อมูลเข้า Dictionary เพื่อให้ค้นหาชื่อเสียงได้ไวและปลอดภัย
            foreach (Sound s in sfxSounds) sfxDictionary[s.name] = s;
            foreach (Sound s in bgmSounds) bgmDictionary[s.name] = s;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ฟังก์ชันสแตติกสำหรับเล่นเสียง SFX (เรียกใช้จากสคริปต์ไหนก็ได้)
    /// </summary>
    public static void PlaySFX(string soundName)
    {
        // ระบบกันพัง 1: ถ้าไม่มี AudioManager ในฉาก ให้ข้ามไปเลย ไม่ Error
        if (Instance == null || Instance.sfxSource == null) return;

        // ระบบกันพัง 2: ถ้ามีชื่อเสียงนี้ และมีการใส่ AudioClip ไว้ ถึงจะเล่น
        if (Instance.sfxDictionary.TryGetValue(soundName, out Sound s))
        {
            if (s.clip != null)
            {
                Instance.sfxSource.PlayOneShot(s.clip, s.volume);
            }
        }
        else
        {
            // ถ้าพิมพ์ชื่อผิด หรือลืมตั้งค่า จะแค่แจ้งเตือนสีเหลือง (ไม่แดง ไม่ค้าง)
            Debug.LogWarning($"[AudioManager] ไม่พบเสียง SFX ชื่อ: {soundName}");
        }
    }

    /// <summary>
    /// ฟังก์ชันเล่นเพลงพื้นหลัง (BGM)
    /// </summary>
    public static void PlayBGM(string soundName)
    {
        if (Instance == null || Instance.bgmSource == null) return;

        if (Instance.bgmDictionary.TryGetValue(soundName, out Sound s))
        {
            if (s.clip != null)
            {
                Instance.bgmSource.clip = s.clip;
                Instance.bgmSource.volume = s.volume;
                Instance.bgmSource.loop = true;
                Instance.bgmSource.Play();
            }
        }
    }
}