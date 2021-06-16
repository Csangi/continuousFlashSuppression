using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;

public class CreateLog : MonoBehaviour
{
    static GameObject uiVars;
    public List<int> trialType = new List<int>(); //instruction on not
    public List<int> block = new List<int>(); //blocks
    public List<int> blockRand = new List<int>(); //for randomization purposes
    public List<int> trialNo = new List<int>(); //trial number
    public List<int> trialRand = new List<int>(); //trial randomization
    public List<int> duration = new List<int>(); //duration of theImg showing
    public List<int> flash = new List<int>(); //flash duration for mondrians
    public List<string> img = new List<string>(); //the image name
    public List<int> opacity = new List<int>(); //max opacity the image reaches
    public List<int> delay = new List<int>(); //delay till next theImg. nothing shows in this time.
    public List<int> order = new List<int>(); //just for the orde/randomization. this is a work in progress
    public string path;
    public string eye;

    public Toggle rightEye;
    public Toggle leftEye;
    public string ID;


    //for the number of trials
    private int size;
    // Start is called before the first frame update
    void Start()
    {
        //set all the lists equal to the lists grabbed bu  UIManager
        uiVars = GameObject.Find("UIManager");

        block = uiVars.GetComponent<UIManager>().block;
        trialNo = uiVars.GetComponent<UIManager>().trialNo;
        img = uiVars.GetComponent<UIManager>().img;
        order = uiVars.GetComponent<UIManager>().order;
        //set toggles
        rightEye = uiVars.GetComponent<UIManager>().rightEye;
        leftEye = uiVars.GetComponent<UIManager>().leftEye;
        //get path for the folder we are working in
        path = uiVars.GetComponent<UIManager>().path;
        ID = uiVars.GetComponent<UIManager>().ID;
        eye = uiVars.GetComponent<UIManager>().eye;
        size = trialNo.Count;

        //we need the path of the file to save the csv to
        string filePath = GetPath(path, ID);
        StreamWriter wr = new StreamWriter(filePath);

        //print the dominant eye first
        wr.WriteLine("Dominant eye:," + eye);

        //headers
        wr.WriteLine("Block, Trial, Order, Image");

        for(int i = 1; i < size; i++)
        {
            wr.WriteLine(block[i] + "," + trialNo[i] + "," + (order[i] + 1) + "," + img[i]);
        }

        //and close file
        wr.Close();
    }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    string GetPath(string path, string ID)
    {
        string[] tok = path.Split('\\');

        string filePath = "";
        //we want to ignore the last token, because thats the orignal csv file. 
        for(int i = 0; i < tok.Length - 1; i++)
        {
            //Debug.Log(i + ": " + tok[i]);
            filePath += tok[i] + "\\";
        }
        //add the "output_logs/ID.csv" to the end.
        filePath += "output_logs";
        //make sure output_logs file exist before returning
        if(!File.Exists(filePath))
            Directory.CreateDirectory(filePath);

        filePath = filePath + "\\" + ID + ".csv";

        return filePath;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
