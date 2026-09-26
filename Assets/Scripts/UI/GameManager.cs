using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _startingTransition;
    [SerializeField] private GameObject _endingTransition;
    [SerializeField] private GameObject BG;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void LoadScene(string sceneName)
    {
        Instance.StartCoroutine(Instance.ChangeScene(sceneName));
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     StartCoroutine(ChangeScene("Game"));
        // }
    }

    private IEnumerator ChangeScene(string sceneName)
    {
        BG.SetActive(true);
        _startingTransition.SetActive(true);

        yield return new WaitForSeconds(1f);

        _startingTransition.SetActive(false);
        _endingTransition.SetActive(true);

        Animator animator = _endingTransition.GetComponent<Animator>();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }

        // animator.Play("EndTransition");

        yield return null;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("EndTransition"))
        {
            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        BG.SetActive(false);
        _endingTransition.SetActive(false);
    }
}