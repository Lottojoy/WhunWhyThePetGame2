using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerPanelManager : MonoBehaviour
{
    [Header("=== Panel ===")]
    [SerializeField] private GameObject selectTypePanel;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject joinPanel;


    [Header("=== Button ===")]
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button joinBtn;
    [SerializeField] private Button hostCloseBtn;
    [SerializeField] private Button enterBtn;
    [SerializeField] private Button clientCloseBtn;


    [SerializeField] private TMP_Text codeLabel;
    [SerializeField] private TMP_InputField codeInput;

    private GameObject currentPanel;

    void Awake()
    {
        currentPanel = selectTypePanel;
    }

    void Start()
    {
        // ========= HostPanel =========
        hostBtn.onClick.AddListener(onHostBtnClick);
        hostCloseBtn.onClick.AddListener(onCloseBtnHostClick);
        // ========= JoinPanel =========
        joinBtn.onClick.AddListener(onJoinBtnClick);
        clientCloseBtn.onClick.AddListener(() =>
        {
            currentPanel.SetActive(false);
            codeInput.text = "";
            currentPanel = selectTypePanel;
            currentPanel.SetActive(true);
        });
        enterBtn.onClick.AddListener(onEnterBtn);
    }

    private async void onEnterBtn()
    {
        if (codeInput.text.Length != 6) return;
        await ConnectRelay.Instance.JoinRelay(codeInput.text);
        codeInput.text = "";
    }

    private async void onHostBtnClick()
    {
        string joinCode = await ConnectRelay.Instance.CreateRelay();
        currentPanel.SetActive(false);
        currentPanel = hostPanel;
        string separated = string.Join(" ", joinCode.ToCharArray());
        codeLabel.text = "code : " + separated;
        currentPanel.SetActive(true);
    }

    private async void onCloseBtnHostClick()
    {
        currentPanel.SetActive(false);
        currentPanel = selectTypePanel;
        ConnectRelay.Instance.CloseRelay();
        currentPanel.SetActive(true);
    }

    private async void onJoinBtnClick()
    {
        currentPanel.SetActive(false);
        currentPanel = joinPanel;
        currentPanel.SetActive(true);
    }
}
