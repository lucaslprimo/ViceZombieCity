using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject startButton;

    [SerializeField]
    private GameObject loadingContainer;

    [SerializeField]
    private Slider sliderLoading;

    public static LevelLoader Instance;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadScene(sceneIndex));
    }

    IEnumerator LoadScene(int sceneIndex)
    {
        FadeIn();
        yield return new WaitForSeconds(2f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            sliderLoading.value = operation.progress;
            yield return null;
        }

        loadingContainer.SetActive(false);
        startButton.SetActive(true);
    }

    public void FadeOut()
    {
        animator.SetTrigger("fade_out");
        if (GameManager.Instance)
        {
            GameManager.Instance.StartGame();
        }
    }

    private void FadeIn()
    {
        animator.SetTrigger("fade_in");
    }
}
