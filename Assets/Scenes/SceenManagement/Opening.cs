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

    public void openTOS()
    {
        SimpleGDPR.ShowDialog(new TermsOfServiceDialog().
                SetTermsOfServiceLink("https://policies.google.com/terms?hl=en-US").
                SetPrivacyPolicyLink("https://policies.google.com/privacy?hl=en-US"),
                onMenuClosed);
    }
    private void onMenuClosed()
    {
        Debug.LogWarning("Nothing, Terms of Service already accepted");
    }
}
