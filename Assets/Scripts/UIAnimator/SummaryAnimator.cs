using UnityEngine;
using System.Collections;

public class SummaryAnimator : MonoBehaviour
{
    public CuteUIAnimator[] stars;
    public CuteUIAnimator[] playerInfos;
    public CuteUIAnimator earnText;
    public CuteUIAnimator graph;

    [Header("ลาก SummaryUIController มาใส่ เพื่อสั่งวาดกราฟทีละจุดหลังกล่องเด้งเข้ามาเสร็จ")]
    public SummaryUIController summaryUIController;

    [Header("=== Audio Settings ===")]
    [Tooltip("พิมพ์ชื่อเพลง BGM ที่ตั้งไว้ใน AudioManager ลงช่องนี้")]
    public string menuBgmName = "MenuBackground";

    [Tooltip("ชื่อเสียงตอนกดปุ่ม Play")]
    public string playButtonSfxName = "Click";

   
    public string PopUpSoundSfxName = "PopUpSound";
    public string textShowSoundSfxName = "textShowSound";

    private void Start()
    {
        AudioManager.PlayBGM(menuBgmName);

        

        // =========================
        // EARN
        // =========================
        if (earnText != null && earnText.gameObject.activeInHierarchy)
        {
            earnText.PopIn(0.75f);
            StartCoroutine(PlaySoundDelayed(textShowSoundSfxName, 0.75f));
        }

        // =========================
        // GRAPH
        // =========================
        if (graph != null && graph.gameObject.activeInHierarchy)
        {
            graph.PopIn(0.9f);
            StartCoroutine(PlaySoundDelayed(PopUpSoundSfxName, 0.9f));

            if (summaryUIController != null)
                StartCoroutine(DrawGraphAfterPopIn());
        }

        // =========================
        // PLAYERS
        // =========================
        if (playerInfos != null)
        {
            for (int i = 0; i < playerInfos.Length; i++)
            {
                if (playerInfos[i] != null && playerInfos[i].gameObject.activeInHierarchy)
                {
                    float delay = 1.1f + (i * 0.15f);
                    playerInfos[i].PopIn(delay);
                    StartCoroutine(PlaySoundDelayed(PopUpSoundSfxName, delay));
                }
            }
        }
    }

    private IEnumerator DrawGraphAfterPopIn()
    {
        // ลบเสียงออกจากตรงนี้ เพราะเสียงกล่องโผล่ถูกย้ายไปเล่นคู่กับ graph.PopIn ด้านบนแล้ว
        yield return new WaitForSeconds(0.9f + 0.35f);
        summaryUIController.DrawGraphAnimated();
    }

    /// <summary>
    /// ฟังก์ชันช่วยหน่วงเวลาเล่นเสียงให้ตรงกับจังหวะที่ UI โผล่ขึ้นมาจริงๆ
    /// </summary>
    private IEnumerator PlaySoundDelayed(string soundName, float delayTime)
    {
        // รอเวลาตามค่า delay
        if (delayTime > 0f)
        {
            yield return new WaitForSeconds(delayTime);
        }

        // เมื่อครบเวลาค่อยสั่งเล่นเสียง
        AudioManager.PlaySFX(soundName);
    }
}