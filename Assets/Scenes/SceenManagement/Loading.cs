using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField]
    private Image progress;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        AsyncOperation gamelevel = SceneManager.LoadSceneAsync(Experiment.current.sceneToBeLoaded);
        while (gamelevel.progress < 1)
        {
            Debug.Log(gamelevel.progress);
            progress.fillAmount = gamelevel.progress;
            yield return new WaitForEndOfFrame();
        }
    }

}
