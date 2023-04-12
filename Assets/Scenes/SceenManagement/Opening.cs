using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Opening : MonoBehaviour
{
    public Experiment experiment = Experiment.current; //= Experiment.current;
    public GameObject cmdTestingInput;
    public void Start()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        if (args.Length > 1 && args.Length <= 6 && !experiment.hasUploaded)
        {
            experiment.args = args;
            SceneManager.LoadScene(2);
        }
    }

    public void separateArgs()
    {
        Debug.Log(cmdTestingInput.GetComponent<Text>().text);
        experiment.args = (cmdTestingInput.GetComponent<Text>().text).Split(' ');
        SceneManager.LoadScene(2);
    }

    //original attempt at command line function, ended up using a different method
    public void cmdprompt(string[] args)
    {
        foreach (var i in args)
            Debug.Log(i);
        bool uploadGood = true;
        /*StreamWriter SR = new StreamWriter("erroroutputfile.txt");
        foreach (var i in args)
        {
            SR.WriteLine(i);
        }*/
        int numberCondition = 0;
        var uiManager = FindObjectOfType<UIManager>();
        if (args.Length >= 4)
        {
            experiment.ID = args[2];
            Debug.Log("ID set to: " + args[2]);
            //SR.WriteLine("ID set to: " + args[2]);

            if (args[3] == "right")
            {
                experiment.right = true;
                Debug.Log("Eye is set to: right");
                //SR.WriteLine("Eye is set to: right");
            }
            else if (args[3] == "left")
            {
                experiment.left = true;
                Debug.Log("Eye is set to: left");
                //SR.WriteLine("Eye is set to: left");
            }
            else
            {
                experiment.right = true;
                Debug.Log("Eye not entered: defaulting to right");
                //SR.WriteLine("Eye not entered: defaulting to right");
            }

            try
            {
                experiment.addCondition(new Condition(true));                       //no matter what we will still need to add the first condition and block
                experiment.conditions[numberCondition].addBlocks(new Block(false));
                experiment.Mondrians.Add(new Mondrian(0, 0, true, 5, 15, 5, 15, 5000));       //here we add the default mondrian if the user don't want to make their own 
                experiment.mondsHaveBeenDrawn = false;
                //uiManager.uploadExperiment(args[1], numberCondition, numberBlock);
            }
            catch (IOException)
            {
                Debug.Log("IO Exception");
                //SR.WriteLine("IO Exception");
                //SR.Close();
                uploadGood = false;
            }
            catch (IndexOutOfRangeException)
            {
                Debug.Log("Index out of bounds");
                //SR.WriteLine("Index out of bounds");
                //SR.Close();
                uploadGood = false;
            }
            catch (NullReferenceException)
            {

                Debug.Log("Null reference Exception");
                //SR.WriteLine("Null reference Exception");
                //SR.Close();
                uploadGood = false;
            }

            //Console.WriteLine(args);

            if (uploadGood)
            {
                //SR.WriteLine("File accepted");
                //SR.Close();
                StartCoroutine(fullUpload());
            }
        }
    }

    IEnumerator fullUpload()
    {
        yield return new WaitUntil(() => experiment.successfulUpload);
        Experiment.current.sceneToBeLoaded = 4;
        SceneManager.LoadScene(3);
    }

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
