using UnityEngine;

public class MainMenuAnimator : MonoBehaviour
{
    public CuteUIAnimator title1;
    public CuteUIAnimator title2;
    public CuteUIAnimator title3;

    public CuteUIAnimator startButton;
    public CuteUIAnimator optionsButton;
    public CuteUIAnimator donateButton;
    public CuteUIAnimator quitButton;

    private void Start()
    {
        // ตรวจสอบว่าลืมลาก Title 1 หรือไม่
        if (title1 != null)
        {
            title1.PopIn(0f);
            title1.StartFloating(2f, 1f); // ย้ายขึ้นมาอยู่กลุ่มเดียวกัน
        }
        else
        {
            Debug.LogError("กรุณาลาก Object มาใส่ในช่อง Title 1 บน Inspector ด้วยครับ!", this);
        }

        // ตรวจสอบ Title 2 และ 3
        if (title2 != null) title2.PopIn(0.08f);
        if (title3 != null) title3.PopIn(0.16f);

        // ตรวจสอบกลุ่ม Buttons
        if (startButton != null) startButton.PopIn(0.30f);
        if (optionsButton != null) optionsButton.PopIn(0.38f);
        if (donateButton != null) donateButton.PopIn(0.46f);
        if (quitButton != null) quitButton.PopIn(0.54f);
    }
}
