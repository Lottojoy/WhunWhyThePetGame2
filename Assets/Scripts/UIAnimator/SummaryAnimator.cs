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

    private void Start()
    {
        // =========================
        // STARS
        // =========================
        if (stars != null)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null && stars[i].gameObject.activeInHierarchy)
                    stars[i].PopIn(i * 0.15f);
            }
        }

        // =========================
        // EARN
        // =========================
        if (earnText != null && earnText.gameObject.activeInHierarchy)
            earnText.PopIn(0.75f);

        // =========================
        // GRAPH
        // =========================
        if (graph != null && graph.gameObject.activeInHierarchy)
        {
            graph.PopIn(0.9f);

            // รอให้กล่องกราฟ PopIn เสร็จก่อน (delay 0.9 + duration เด้ง 0.35 ใน CuteUIAnimator)
            // ค่อยเริ่มวาดจุดทีละอันข้างใน
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
                    playerInfos[i].PopIn(
                        1.1f + i * 0.15f
                    );
                }
            }
        }
    }

    private IEnumerator DrawGraphAfterPopIn()
    {
        yield return new WaitForSeconds(0.9f + 0.35f);
        summaryUIController.DrawGraphAnimated();
    }
}