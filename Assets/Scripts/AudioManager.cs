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
    /// ฟังก์ชันเล่นเสียง SFX
    /// </summary>
    public static void PlaySFX(string soundName)
    {
        if (Instance == null || Instance.sfxSource == null) return;

        // 1. เช็คว่ามี "ชื่อเสียง" นี้อยู่ในระบบหรือไม่
        if (Instance.sfxDictionary.TryGetValue(soundName, out Sound s))
        {
            // 2. เช็คว่าชื่อนี้ มีการลาก "ไฟล์เสียง (Clip)" มาใส่ไว้หรือยัง
            if (s.clip != null)
            {
                Instance.sfxSource.PlayOneShot(s.clip, s.volume);

                // ---- [เพิ่ม Debug แจ้งเตือนว่ากำลังเล่นเสียง SFX] ----
                Debug.Log($"[AudioManager] 🔊 กำลังเล่นเสียง SFX: {soundName}");
            }
            else
            {
                // เตือนกรณีที่ 2: มีชื่อ แต่ลืมใส่ไฟล์เสียง
                Debug.LogWarning($"[AudioManager] พบชื่อ SFX '{soundName}' แต่คุณลืมลากไฟล์เสียง AudioClip มาใส่!");
            }
        }
        else
        {
            // เตือนกรณีที่ 1: พิมพ์ชื่อผิด หรือยังไม่ได้สร้างชื่อนี้
            Debug.LogWarning($"[AudioManager] ไม่พบเสียง SFX ชื่อ: '{soundName}' กรุณาเช็คการสะกดคำใน Inspector");
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

                // ---- [เพิ่ม Debug แจ้งเตือนว่ากำลังเล่นเพลง BGM] ----
                Debug.Log($"[AudioManager] 🎵 กำลังเล่นเพลง BGM: {soundName}");
            }
            else
            {
                Debug.LogWarning($"[AudioManager] พบชื่อ BGM '{soundName}' แต่คุณลืมลากไฟล์เพลง AudioClip มาใส่!");
            }
        }
        else
        {
            Debug.LogWarning($"[AudioManager] ไม่พบเพลง BGM ชื่อ: '{soundName}' กรุณาเช็คการสะกดคำใน Inspector");
        }
    }
}