using UnityEngine;
using UnityEngine.SceneManagement;

public class Opening : MonoBehaviour
{

    public void exit()
    {
        Application.Quit();
    }

    public void openExperiment()
    {
        Experiment.current.sceneToBeLoaded = 2;
        SceneManager.LoadScene(2);
    }

    public void openMondrian()
    {
        Experiment.current.sceneToBeLoaded = 1;
        SceneManager.LoadScene(1);
    }
}
