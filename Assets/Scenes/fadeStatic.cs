using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEditor;


public class fadeStatic : MonoBehaviour
{
    //exp variables
    private bool finished; //to check if game is finished or not, used for dev tools

    private RawImage theImg;
    public bool fade;
    //this gets the info from the UIManager script, which gets all the csv values
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
    public List<int> maxOp = new List<int>(); 
    public string path;
    //get right and left eye toggles
    public Toggle rightEye;
    public Toggle leftEye;

    //used to keep track of time for each subtrial. resets to zero once it 
    //meets the duration set time
    public float counter;

    //used to keep track of which subtrial we are on
    private int iter;

    //for the number of trials
    private int size;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    //runs a single csv file.
    void Start()
    {
        finished = false;
        counter = 0;
        iter = 0;
        //set all the lists equal to the lists grabbed bu  UIManager
        uiVars = GameObject.Find("UIManager");
        trialType = uiVars.GetComponent<UIManager>().trialType;
        block = uiVars.GetComponent<UIManager>().block;
        blockRand = uiVars.GetComponent<UIManager>().blockRand;
        trialNo = uiVars.GetComponent<UIManager>().trialNo;
        trialRand = uiVars.GetComponent<UIManager>().trialRand;
        duration = uiVars.GetComponent<UIManager>().duration;
        flash = uiVars.GetComponent<UIManager>().flash;
        img = uiVars.GetComponent<UIManager>().img;
        opacity = uiVars.GetComponent<UIManager>().opacity;
        delay = uiVars.GetComponent<UIManager>().delay;
        order = uiVars.GetComponent<UIManager>().order;
        maxOp = uiVars.GetComponent<UIManager>().maxOp;
        //set toggles
        rightEye = uiVars.GetComponent<UIManager>().rightEye;
        leftEye = uiVars.GetComponent<UIManager>().leftEye;
        //get path for the folder we are working in
        path = uiVars.GetComponent<UIManager>().path;

        size = trialNo.Count;

        //if right eye is dominant, show mondrians to right eye and image to left
        if(rightEye.isOn == true)
        {
            theImg = GameObject.Find("staticLeft").GetComponent<RawImage>();
            //turnoff the other side image
            RawImage turnOff = GameObject.Find("staticRight").GetComponent<RawImage>();
            turnOff.enabled = false;
        }

        //if left eye is dom, show monds to left eye and image to right
        if(leftEye.isOn == true)
        {
            theImg = GameObject.Find("staticRight").GetComponent<RawImage>();
            //turn off other side image
            RawImage turnOff = GameObject.Find("staticLeft").GetComponent<RawImage>();
            turnOff.enabled = false;
        }

        theImg.enabled = false;
        

        //after setting all values, call the coroutine to actually run trials in file.
        //make float value from the THIRD duration number, second will be instructions.
        float repeatRate = (float)duration[3]/1000;
        //invokerepeating(methodNAme, start time, repeatrate);
        InvokeRepeating("RunTrial", 0.0f, repeatRate);
    }

    //used for dev tools
    void Update() 
    {
        if(finished)
        {
            //to quick restart it
            if(Input.GetKeyDown(KeyCode.Space))
            {
                iter = 0;
                finished = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("End");
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            //show menu
            Debug.Log("menu");
        }
    }

    void RunTrial()
    {
        int i = iter;
        Debug.Log("iter: " + i);
        if(i == size)
        {
            Debug.Log("Finished.");
            finished = true;
            theImg.enabled = false;
            
            //call finishing scene
            SceneManager.LoadScene("End");
        }
        else
        {
            string imgPath = ImgPath(order[i]);
            //Debug.Log(imgPath);
            LoadImg(imgPath);

            //a 0 duration value means the subtrial is an instruction, so we need to handle that differently
            if(duration[order[i]] == 0)
            {   
                //TODO: instructions
                // Debug.Log(duration[order[i]]);
                //StartCoroutine(ShowInstructions());
                iter++;
            }
            else
            {
                //trial order: delay time, fade in time, static time.
                                //delay     maxOp          duration - (delay + maxOp)
                //then add a small delay inbetween each trial?? 
                //start fading in image
                StartCoroutine(FadeImage(delay[order[i]], maxOp[order[i]], duration[order[i]], opacity[order[i]]));
                
                //StartCoroutine(WaitTimes(500));
                //TODO: before next trial, enter delay time 
                iter++;
            }
            
        }

        return;
    }

    string ImgPath(int loc)
    {
        //we must tokenize the original path in order to format it correctly
        string[] tok = path.Split('\\');

        string imgPath = "";
        //we want to ignore the last token, because thats the csv file. 
        for(int i = 0; i < tok.Length - 1; i++)
        {
            //Debug.Log(i + ": " + tok[i]);
            imgPath += tok[i] + "\\";
        }
        //finally, now that the csv file is removed from the string, add the img file name
        imgPath += img[loc];
        //Debug.Log(imgPath);
        return imgPath;
    }

    void LoadImg(string imgPath)
    {
        //to access the image on a local computer, read it in using a byte array
        byte[] bytes = System.IO.File.ReadAllBytes(imgPath);
        //convert byte array to texture
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        //assign new texture to theImg variable to replcae/update the pictures
        theImg.GetComponent<RawImage>().texture = tex;
    }

//https://forum.unity.com/threads/simple-ui-animation-fade-in-fade-out-c.439825/
    IEnumerator FadeImage(int delay, int maxOp, int dur, float op)
    {
        //becasue we have to use the invokereapeating function, we have to make a custom
        //fade to fit into the time constraints and opacity.

        //divide by 100 to get percentage
        float opac = (float)op/100.0f;
        theImg.enabled = true;
        
        //use steps to increment the opacity of the picture. use maxOp because that is the amount of time it fades
        float steps = opac/(float)dur;
        //DOUBLE CHECK ON ANOTHER PC: unity refresh rate seems to be stuck at around 200ms which is .20f for steps.

        for (float i = 0; i <= opac; i += .25f)
        {
            Debug.Log(i);
            // set color with i as alpha
            theImg.color = new Color(1, 1, 1, i);
            yield return new WaitForSeconds(dur / 1000);
        }
    }

    IEnumerator WaitTimes(int ms)
    {
        Debug.Log("now waiting");
        int s = ms /1000;
        yield return new WaitForSeconds(s);
    }

    /*IEnumerator ShowInstructions()
    {
        //todo: grab instruction text?
        if(EditorUtility.DisplayDialog("Instructions", "message", "ok", "cancel"))
        {
            Debug.Log("ok");
            yield return null;
        } 
        else if(!EditorUtility.DisplayDialog("Instructions", "message", "ok", "cancel"))
        {
            Debug.Log("cancel");
            yield return null;
        }
    }*/
}
