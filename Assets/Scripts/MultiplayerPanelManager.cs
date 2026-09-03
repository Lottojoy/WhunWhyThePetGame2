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
        hostBtn.onClick.AddListener(() =>
        {
            currentPanel.SetActive(false);
            currentPanel = hostPanel;
            currentPanel.SetActive(true);
            codeLabel.text = "Code : X X X X X X";
        });
        hostCloseBtn.onClick.AddListener(() =>
        {
            currentPanel.SetActive(false);
            currentPanel = selectTypePanel;
            currentPanel.SetActive(true);
        });
        // ========= JoinPanel =========
        joinBtn.onClick.AddListener(() =>
        {
            currentPanel.SetActive(false);
            currentPanel = joinPanel;
            currentPanel.SetActive(true);
        });
        clientCloseBtn.onClick.AddListener(() =>
        {
            currentPanel.SetActive(false);
            codeInput.text = "";
            currentPanel = selectTypePanel;
            currentPanel.SetActive(true);
        });
        enterBtn.onClick.AddListener(() =>
        {
            Debug.Log("Entered Code : " + codeInput.text);
        });
    }
}
