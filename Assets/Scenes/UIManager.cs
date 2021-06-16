using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEditor;
using Random=UnityEngine.Random;


public class UIManager : MonoBehaviour
{
    //create our public lists that will be appended once a file is read
    public List<int> trialType = new List<int>(); //instruction on not
    public List<int> block = new List<int>(); //blocks
    public List<int> blockRand = new List<int>(); //for randomization purposes
    public List<int> trialNo = new List<int>(); //trial number
    public List<int> trialRand = new List<int>(); //trial randomization
    public List<int> duration = new List<int>(); //duration of img showing
    public List<int> flash = new List<int>(); //flash duration for mondrians
    public List<string> img = new List<string>(); //the image name
    public List<int> opacity = new List<int>(); //max opacity the image reaches
    public List<int> delay = new List<int>(); //delay till next img. nothing shows in this time. in the beginning
    public List<int> maxOp = new List<int>(); //the time in ms to get to max opacity. 

    //this list will hold the random order of how to present the trials. this way each trial
    //can be random for a single CSV file.
    public List<int> order = new List<int>();

    //these control the toggles for left and right eye on the left side of the screen
    public Toggle rightEye;
    public Toggle leftEye;

    //for some reason, CreateLog wont recognize the toggles. so use this public bool
    public string eye;

    //to get the file destination
    public string path;

    //user ID
    public string ID;

    //upload file libarary docs: https://github.com/quangdungtr/UnityStandaloneFileBrowser
    public void upload()
    {
        //set path to epty string
        path = "";

        //this actually opens the filepanel to allow the user to choose a file. the last variable is the file type - limiting to csv for now
        string[] holderPath = SFB.StandaloneFileBrowser.OpenFilePanel("Open File", "", "csv", true);

        //convert string[] into normal string
        for(int i = 0; i < holderPath.Length; i++)
            path += holderPath[i];

        Debug.Log(path);

        if (path.Length != 0)
        {
            //if a valid file is chosen, we use a streamreader to actually get the data.
            //streamreader docs: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-read-text-from-a-file
            using(var sr = new StreamReader(path))
            {
                //now we can actually use the data
                //Debug.Log(sr.ReadToEnd());
                bool EOF = false;
                int counter = 0;

                //while we havent reached the end of the line, keep going
                while(!EOF)
                {
                    //read a single line at a time 
                    string data = sr.ReadLine();

                    //if null then we reached the end of the file
                    if(data == null)
                    {
                        EOF = true;
                        break;
                    }

                    //split by commas, store in array of strings
                    var values = data.Split(',');

                    //put values into public lists. 
                    /*
                    0 == trialType
                    1 == block
                    2 == blockRand
                    3 == trialNo
                    4 -- trialRand
                    5 -- duration
                    6 -- flash
                    7 -- img
                    8 --opacity
                    9 -- delay
                    */
                    for(int i = 0; i < values.Length; i++)
                    {
                        //Debug.Log("Counter:" + counter + " i: " + i);
                        //this removes the first line of the csv, which will be the title of the column
                        if(counter > 0)
                        {
                            //this is used for string to int conversion in the try block below
                            int result = 0;

                            //since 7 is the image path we dont need to convert that into an int
                            if(i != 7)
                            {
                                try
                                {
                                    //Debug.Log(values[i]);
                                    //convert string to int value to add to list
                                    result = Int32.Parse(values[i]);
                                }
                                catch (FormatException)
                                {
                                    Debug.Log("Error parsing value.");
                                }
                                //Debug.Log(i + " " + result);
                            }

                            switch(i)
                            {
                                case 0:
                                    trialType.Add(result);
                                    break;
                                case 1:
                                    block.Add(result);
                                    break;
                                case 2:
                                    blockRand.Add(result);
                                    break;
                                case 3:
                                    trialNo.Add(result);
                                    break;
                                case 4:
                                    trialRand.Add(result);
                                    break;
                                case 5:
                                    duration.Add(result);
                                    break;
                                case 6:
                                    flash.Add(result);
                                    break;
                                case 7:
                                    if(values[i] == "NAN")
                                        Debug.Log("none");
                                    else
                                        img.Add(values[i]);
                                    break;
                                case 8:
                                    opacity.Add(result);
                                    break;
                                case 9:
                                    delay.Add(result);
                                    break;
                                case 10:
                                    maxOp.Add(result);
                                    break;
                            }
                            
                            
                            
                        }
                    }
                    
                    //to keep track of what line we are on. we are using the assumation that the first line will be titles,
                    //the nexts lines are data.
                    counter++;
                }
                
                //finally, pass the created lists to the random function to create a random trial.
                RandomGen();
                sr.Close();
            }
        }
        else
        {
            //EditorUtility.DisplayDialog("Error!", "fuck", "OK");
        }
    }

    //using UnityEngine.Random for random
    public void RandomGen()
    {
        int size = trialNo.Count; //use this to hold previous random numbers so we dont get repeats
        int x = Random.Range(0, size);//initial rand
        order.Add(x);
        for(int i = 0; i < size - 1; i++)
        {
            //look for a random number till its not a repeat
            do
            {
                x = Random.Range(0, size);
            } while (order.Contains(x));
            //then add it to the prev list
            order.Add(x);
        }
        return;
    }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void StartGame() 
    {
        rightEye = GameObject.Find("rightEye").GetComponent<Toggle>();
        leftEye = GameObject.Find("leftEye").GetComponent<Toggle>();

        if(rightEye.isOn == true)
            eye = "Right";
        else
            eye = "Left";

        InputField idHolder = GameObject.Find("idHolder").GetComponent<InputField>();
        ID = idHolder.text;

        if(trialNo.Count < 1)
            Debug.Log("no file");
        else
        {
            //TODO: add check display that prints out which file and which dom eye
            SceneManager.LoadScene("exp");
        }
        return;
    }
}
