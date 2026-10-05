using UnityEngine;
using UnityEngine.UI;

public class StationProgressUI : MonoBehaviour
{
    [SerializeField] private WorkStationCounter workStationCounter;
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject progressBarContainer;

    private void Start()
    {
        workStationCounter.OnProgressChanged += WorkStationCounter_OnProgressChanged;
        progressBarContainer.SetActive(false);
    }

    private void WorkStationCounter_OnProgressChanged(float progress)
    {
        progressBarContainer.SetActive(progress > 0f && progress < 1f);
        if (barImage != null)
        {
            barImage.fillAmount = progress;
        }
    }
}