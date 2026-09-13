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
    [SerializeField] private GameObject leavePanel;

    [Header("=== Button ===")]
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button joinBtn;
    [SerializeField] private Button hostCloseBtn;
    [SerializeField] private Button enterBtn;
    [SerializeField] private Button clientCloseBtn;
    [SerializeField] private Button leaveBtn;

    [Header("=== Code Display ===")]
    [SerializeField] private TMP_Text hostCodeLabel;
    [SerializeField] private TMP_Text clientCodeLabel;
    [SerializeField] private TMP_InputField codeInput;

    private GameObject currentPanel;
    private string currentJoinCode;

    void Awake()
    {
        currentPanel = selectTypePanel;
    }

    void Start()
    {
        // ========= HostPanel =========
        hostBtn.onClick.AddListener(onHostBtnClick);
        hostCloseBtn.onClick.AddListener(onCloseBtnHostClick);

        // Host code label — click to copy
        if (hostCodeLabel != null)
        {
            var hostCodeBtn = hostCodeLabel.GetComponent<Button>();
            if (hostCodeBtn != null)
                hostCodeBtn.onClick.AddListener(OnHostCodeClicked);
        }

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

        // ========= LeavePanel =========
        if (leaveBtn != null)
            leaveBtn.onClick.AddListener(OnLeaveBtnClicked);
    }

    private void OnHostCodeClicked()
    {
        if (!string.IsNullOrEmpty(currentJoinCode))
        {
            GUIUtility.systemCopyBuffer = currentJoinCode;
            Debug.Log($"[Lobby] Room code copied to clipboard: {currentJoinCode}");
        }
    }

    private async void onEnterBtn()
    {
        if (codeInput.text.Length != 6) return;

        // Store the code before joining
        currentJoinCode = codeInput.text.Replace(" ", "");
        await ConnectRelay.Instance.JoinRelay(currentJoinCode);
        codeInput.text = "";

        // Switch to leave panel showing room code
        ShowLeavePanel();
    }

    private async void onHostBtnClick()
    {
        string joinCode = await ConnectRelay.Instance.CreateRelay();
        currentJoinCode = joinCode;

        currentPanel.SetActive(false);

        // Show code on host panel
        string separated = string.Join(" ", joinCode.ToCharArray());
        if (hostCodeLabel != null)
            hostCodeLabel.text = "code : " + separated;

        currentPanel = hostPanel;
        currentPanel.SetActive(true);
    }

    private void ShowLeavePanel()
    {
        currentPanel.SetActive(false);

        // Show code on leave panel for client
        if (clientCodeLabel != null)
        {
            string separated = string.Join(" ", currentJoinCode.ToCharArray());
            clientCodeLabel.text = "code : " + separated;
        }

        currentPanel = leavePanel;
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

    private void OnLeaveBtnClicked()
    {
        currentPanel.SetActive(false);
        ConnectRelay.Instance.CloseRelay();
        currentJoinCode = "";
        currentPanel = selectTypePanel;
        currentPanel.SetActive(true);
    }
}
