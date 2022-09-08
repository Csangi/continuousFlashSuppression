using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExperimentRunner : MonoBehaviour
{

    public Experiment exp = Experiment.current;

    //our 2 raw image arrays corrisponding to the 2 image arrays in the project
    public RawImage[] rightImgArr;
    public RawImage[] leftImgArr;

    //these will be the trackers of condition index, block index, and trial index
    int CI, BI, TI;

    //right and left dominace 
    public bool right = false;
    public bool left = false;

    //string for the ID
    public string ID = "";

    //a boolean for if the writer has started or not
    bool writerStart = false;


    //bool for starting a response and a float for holding the time 
    bool startTimer;
    float responseTimer;

    //exit bool for exiting half way through an experiment 
    bool exit;

    //checking for if the trial has finished running
    bool trialHasRun;

    //checking for if the entire experiment has finished as to not over lap 
    bool expIsRunning;

    //checking for if the picture has uploaded before starting the experiment
    bool picIsUploaded;

    //all response booleans
    bool response;
    bool upArr, downArr, leftArr, rightArr;

    public RawImage rightPicImage;
    public RawImage leftPicImage;

    // Start is called before the first frame update
    void Start()
    {
        right = left = expIsRunning = exit = response = upArr = downArr = leftArr = rightArr = expIsRunning = startTimer = false;       //setting all booleans to false incase user returns to this scene
        var createMond = FindObjectOfType<createMond>();                                                                                //getting to the mondrian script so it can draw on the blank images
        if (!exp.mondsHaveBeenDrawn)
            createMond.updateMond(rightImgArr, leftImgArr);                                                                                 //calling the create mondrian script 
        right = Experiment.current.right;                                                                                               //getting correct eye dominance 
        left = Experiment.current.left;                                                                                                      //getting the whole experiement
        ID = Experiment.current.ID;                                                                                                     //grabbing id string for the output csv 
        exp.randomizeConditions();                                                                                                      //randomizing the experiment incase user returns and doesnt upload a new csv
        Debug.Log("number of conditions: " + exp.numberOfConditions);
    }

    //using web request to get images from the users file system
    IEnumerator<UnityWebRequestAsyncOperation> MyGetTexture(string imagePath, RawImage changeImage)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + imagePath);
        yield return www.SendWebRequest();
        new WaitForSecondsRealtime(1);
        if (www.isHttpError || www.isNetworkError)
        {
            Debug.Log(www.error);
            Debug.Log(imagePath);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            changeImage.texture = myTexture;
            picIsUploaded = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))        //space to start
        {
            Debug.Log("Trying to run experiment " + expIsRunning + " " + left + " " + right);
            if (left && !expIsRunning)
                StartCoroutine(runExperiment(rightPicImage, leftImgArr));       //start the experiment sending the different images
            else if (right && !expIsRunning)
                StartCoroutine(runExperiment(leftPicImage, rightImgArr));
            else
                response = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))       //escape back to creator
        {
            exit = true;
            Experiment.current.sceneToBeLoaded = 2;
            SceneManager.LoadScene(3);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && !response)
        {
            response = true;
            downArr = true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && !response)
        {
            response = true;
            upArr = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && !response)
        {
            response = true;
            rightArr = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && !response)
        {
            response = true;
            leftArr = true;
        }

        if (startTimer)
            responseTimer += Time.deltaTime;
        else
            responseTimer = 0;
    }
    IEnumerator runExperiment(RawImage changeImage, RawImage[] ImgArr)
    {
        RawImage oppositeChangeImage;
        if (changeImage == rightPicImage)
            oppositeChangeImage = leftPicImage;
        else
            oppositeChangeImage = rightPicImage;

        expIsRunning = true;
        int counter = 0;
        int conditionIndex = 0, blockIndex = 0, trialIndex = 0;
        for (int i = 0; i < exp.numberOfConditions; ++i)
        {
            conditionIndex = exp.index[i];
            for (int j = 0; j < exp.conditions[i].numberOfBlocks; ++j)
            {
                blockIndex = exp.conditions[exp.index[i]].index[j];
                for (int k = 0; k < exp.conditions[i].blocks[j].numberOfTrials; ++k, ++counter)
                {
                    trialHasRun = false;
                    picIsUploaded = false;
                    trialIndex = exp.conditions[exp.index[i]].blocks[exp.conditions[exp.index[i]].index[j]].index[k];
                    writeTrial(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex], conditionIndex, exp.conditions[conditionIndex].random, blockIndex, exp.conditions[conditionIndex].blocks[blockIndex].random, trialIndex, counter);
                    if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is MondTrial)
                    {
                        StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, changeImage));
                        yield return new WaitUntil(() => picIsUploaded);

                        StartCoroutine(runTrial(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MondTrial, changeImage, ImgArr));
                        yield return new WaitUntil(() => trialHasRun);
                        yield return new WaitForSecondsRealtime(1);
                    }
                    else if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is FlashMondTrial)
                    {
                        StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, changeImage));
                        yield return new WaitUntil(() => picIsUploaded);

                        StartCoroutine(runTrial(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as FlashMondTrial, changeImage, ImgArr));
                        yield return new WaitUntil(() => trialHasRun);
                        yield return new WaitForSecondsRealtime(1);
                    }
                    else if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is MaskTrial)
                    {
                        StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, changeImage));
                        yield return new WaitUntil(() => picIsUploaded);
                        picIsUploaded = false;
                        Debug.Log((exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath);
                        StartCoroutine(MyGetTexture((exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath, oppositeChangeImage));
                        yield return new WaitUntil(() => picIsUploaded);

                        StartCoroutine(runTrial(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial, changeImage, oppositeChangeImage, ImgArr));
                        yield return new WaitUntil(() => trialHasRun);
                        yield return new WaitForSecondsRealtime(1);
                    }
                    else if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is Response)
                    {
                        StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, rightPicImage));
                        yield return new WaitUntil(() => picIsUploaded);
                        leftPicImage.texture = rightPicImage.texture;

                        StartCoroutine(runResponse(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as Response, rightPicImage, leftPicImage));
                        yield return new WaitUntil(() => trialHasRun);
                        yield return new WaitForSecondsRealtime(1);
                    }
                    else if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is Instruction)
                    {
                        StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, rightPicImage));
                        yield return new WaitUntil(() => picIsUploaded);
                        leftPicImage.texture = rightPicImage.texture;

                        StartCoroutine(runInstruction(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as Instruction, rightPicImage, leftPicImage));
                        yield return new WaitUntil(() => trialHasRun);
                        yield return new WaitForSecondsRealtime(1);
                    }
                    else if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is Break)
                    {
                        StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, rightPicImage));
                        yield return new WaitUntil(() => picIsUploaded);
                        leftPicImage.texture = rightPicImage.texture;

                        StartCoroutine(runBreak(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as Break, rightPicImage, leftPicImage));
                        yield return new WaitUntil(() => trialHasRun);
                        yield return new WaitForSecondsRealtime(1);
                    }

                    if (exit)
                        yield break;
                }
            }
        }
        expIsRunning = false;
    }

    IEnumerator runBreak(Break I, RawImage changeImage1, RawImage changeImage2)
    {
        changeImage1.enabled = true;
        changeImage2.enabled = true;
        yield return new WaitForSecondsRealtime(I.duration / 1000);
        if (exit)
            yield break;
        changeImage1.enabled = false;
        changeImage2.enabled = false;
        trialHasRun = true;
        yield break;
    }

    IEnumerator runInstruction(Instruction I, RawImage changeImage1, RawImage changeImage2)
    {
        response = false;
        changeImage1.enabled = true;
        changeImage2.enabled = true;
        startTimer = true;
        yield return new WaitForSecondsRealtime(I.duration / 1000);
        yield return new WaitUntil(() => response);
        if (exit)
            yield break;
        I.responseTime = responseTimer;
        startTimer = false;
        changeImage1.enabled = false;
        changeImage2.enabled = false;
        trialHasRun = true;
        response = false;
        yield break;
    }


    IEnumerator runResponse(Response R, RawImage changeImage1, RawImage changeImage2)
    {
        changeImage1.enabled = true;
        changeImage2.enabled = true;

        response = leftArr = rightArr = upArr = downArr = false;
        startTimer = true;

        yield return new WaitUntil(() => response);
        R.responseTime = responseTimer;
        startTimer = false;
        if (leftArr)
            R.response = R.left;
        else if (rightArr)
            R.response = R.right;
        else if (downArr)
            R.response = R.down;
        else if (upArr)
            R.response = R.up;
        else
            R.response = "Something went wrong";

        if (exit)
            yield break;

        yield return new WaitForSecondsRealtime(2);

        changeImage1.enabled = false;
        changeImage2.enabled = false;
        trialHasRun = true;
        response = false;
        yield break;
    }

    IEnumerator runTrial(MondTrial T, RawImage changeImage, RawImage[] ImgArr)
    {
        changeImage.color = new Color(0f, 0f, 0f, 0f);
        changeImage.enabled = true;
        //num of iterations throught the  = Duration / flash duration 
        int numOfIt = T.duration / T.flashDuration;
        //Delay in terms of i (i in the for loop that is)
        int dITOI = T.delay / T.duration;
        //Flash Delay = (duration - time to reach opacity) / flash duration
        int flashDelay = (T.duration - T.timeToReachOpacity) / T.flashDuration;
        //opacity increase each trial = opacity / (number of iterations / flash delay) / 100 because the float value range for alpha is from 0 - 1
        float opacityIncrease = (T.opacity / (numOfIt - flashDelay)) / 100;
        float opacity = new float();
        int holder = 0, x = 0; //holds the last index in the array that is currently active
        float waitingPeriod = (float)T.flashDuration / 1000;
        Debug.Log("running a trial");
        for (int i = 0; i < numOfIt; ++i)   //i will be the number if iterations and we will use i as a flag to when each different action should start
        {
            if (i >= dITOI)     //once i reaches this point it will begin the mondrian flashing
            {   //each new mondrian will be found using a do while loop
                do
                {
                    x = Random.Range(exp.Mondrians[T.mond].minRange, exp.Mondrians[T.mond].maxRange);
                } while (x == holder);
                ImgArr[x].enabled = true;
                ImgArr[holder].enabled = false;
                holder = x;
            }
            //once i reaches the flash delay it will begin fading in the image
            if (i >= flashDelay)
            {
                Debug.Log(opacity);
                //opacity will increase with each iteration until it reaches the specified opacity
                opacity += opacityIncrease;
                changeImage.color = new Color(opacity, opacity, opacity, opacity);
            }

            if (exit)
                yield break;

            yield return new WaitForSecondsRealtime(waitingPeriod);     //we will wait for the specified waiting period
            Debug.Log(i + " " + x);
        }
        ImgArr[x].enabled = false;      //then turn off the last mondrian 
        changeImage.enabled = false;    //turn off the main image
        trialHasRun = true;
        changeImage.color = new Color(1f, 1f, 1f, 1f);
        yield break;
    }

    IEnumerator runTrial(FlashMondTrial T, RawImage changeImage, RawImage[] ImgArr)
    {
        changeImage.color = new Color(0f, 0f, 0f, 0f);
        changeImage.enabled = true;
        float flashPeriod = 10;
        //num of iterations throught the  = Duration / flash duration 
        int numOfIt = T.duration / T.flashDuration;
        //Delay in terms of i (i in the for loop that is)
        int dITOI = T.delay / T.duration;
        //Flash Delay = (duration - time to reach opacity) / flash duration
        int flashDelay = (T.duration - T.timeToReachOpacity) / T.flashDuration;
        //opacity increase each trial = opacity / (number of iterations / flash delay) / 100 because the float value range for alpha is from 0 - 1
        float opacityIncrease = (T.opacity / (numOfIt - flashDelay)) / 100;
        float opacity = new float();
        int holder = 0, x = 0; //holds the last index in the array that is currently active
        float waitingPeriod = ((float)T.flashDuration - flashPeriod) / 1000;
        Debug.Log("running a trial");
        for (int i = 0; i < numOfIt; ++i)   //i will be the number if iterations and we will use i as a flag to when each different action should start
        {
            if (i >= dITOI)     //once i reaches this point it will begin the mondrian flashing
            {   //each new mondrian will be found using a do while loop
                do
                {
                    x = Random.Range(exp.Mondrians[T.mond].minRange, exp.Mondrians[T.mond].maxRange);
                } while (x == holder);
                ImgArr[holder].enabled = false;
                changeImage.enabled = false;
                yield return new WaitForSecondsRealtime(flashPeriod / 1000);
                changeImage.enabled = true;
                ImgArr[x].enabled = true;
                holder = x;
            }
            //once i reaches the flash delay it will begin fading in the image
            if (i >= flashDelay)
            {
                Debug.Log(opacity);
                //opacity will increase with each iteration until it reaches the specified opacity
                opacity += opacityIncrease;
                changeImage.color = new Color(opacity, opacity, opacity, opacity);
            }

            if (exit)
                yield break;

            yield return new WaitForSecondsRealtime(waitingPeriod);     //we will wait for the specified waiting period
            Debug.Log(i + " " + x);
        }
        ImgArr[x].enabled = false;      //then turn off the last mondrian 
        changeImage.enabled = false;    //turn off the main image
        trialHasRun = true;
        changeImage.color = new Color(1f, 1f, 1f, 1f);
        yield break;
    }

    IEnumerator runTrial(MaskTrial T, RawImage changeImage, RawImage oppositeChangeImage, RawImage[] ImgArr)
    {
        changeImage.color = new Color(0f, 0f, 0f, 0f);
        changeImage.enabled = true;
        oppositeChangeImage.enabled = true;
        float flashPeriod = 10;
        //num of iterations throught the  = Duration / flash duration 
        int numOfIt = T.duration / T.flashDuration;
        //Flash Delay = (duration - time to reach opacity) / flash duration
        int flashDelay = (T.duration - T.timeToReachOpacity) / T.flashDuration;
        //opacity increase each trial = opacity / (number of iterations / flash delay) / 100 because the float value range for alpha is from 0 - 1
        float opacityIncrease = (T.opacity / (numOfIt - flashDelay)) / 100;
        float opacity = new float();
        float waitingPeriod = ((float)T.flashDuration - flashPeriod) / 1000;
        Debug.Log("running a trial");
        for (int i = 0; i < numOfIt; ++i)   //i will be the number if iterations and we will use i as a flag to when each different action should start
        {
            //once i reaches the flash delay it will begin fading in the image
            if (i >= flashDelay)
            {
                Debug.Log(opacity);
                //opacity will increase with each iteration until it reaches the specified opacity
                opacity += opacityIncrease;
                changeImage.enabled = false;
                oppositeChangeImage.enabled = false;
                yield return new WaitForSecondsRealtime(flashPeriod / 1000);
                oppositeChangeImage.enabled = true;
                changeImage.enabled = true;
                changeImage.color = new Color(opacity, opacity, opacity, opacity);
            }
            else
            {
                changeImage.enabled = false;
                oppositeChangeImage.enabled = false;
                yield return new WaitForSecondsRealtime(flashPeriod / 1000);
                oppositeChangeImage.enabled = true;
                changeImage.enabled = true;
            }

            if (exit)
                yield break;

            yield return new WaitForSecondsRealtime(waitingPeriod);     //we will wait for the specified waiting period
            Debug.Log(i);
        }
        changeImage.enabled = false;    //turn off the main image
        oppositeChangeImage.enabled = false;
        trialHasRun = true;
        changeImage.color = new Color(1f, 1f, 1f, 1f);
        yield break;
    }

    public void writeTrial(Item IT, int cond, bool condRand, int blk, bool blkRand, int trial, int counter)     //simple writer to an external csv named with the input ID
    {   //using the input path
        string fileName = exp.path + ID + ".csv";
        StreamWriter writer = new StreamWriter(fileName, true);
        if (!writerStart)
        {
            writer.Write("Trial Count,Condition,Cond Rand,Block,Block Rand,Trial Type,Input Order,Trial Rand,Image,Duration,Flash Duration,Opacity,Delay,Time To Reach Opacity,Mondrian,Response Time,Up,Down,Left,Right,Answer,\n");
            writerStart = true;
        }
        if (IT is Instruction)
        {
            Instruction I = IT as Instruction;
            writer.Write(++counter + ",");
            writer.Write(++cond + ",");
            writer.Write(condRand + ",");
            writer.Write(++blk + ",");
            writer.Write(blkRand + ",");
            writer.Write("Instruction,");
            writer.Write(++trial + ",");
            writer.Write(I.random + ",");
            writer.Write(I.image + ",");
            writer.Write(I.duration + ",,,,,,");
            writer.Write(I.responseTime + ",\n");
        }
        else if (IT is MondTrial)
        {
            MondTrial T = IT as MondTrial;
            writer.Write(++counter + ",");
            writer.Write(++cond + ",");
            writer.Write(condRand + ",");
            writer.Write(++blk + ",");
            writer.Write(blkRand + ",");
            writer.Write("Trial,");
            writer.Write(++trial + ",");
            writer.Write(T.random + ",");
            writer.Write(T.image + ",");
            writer.Write(T.duration + ",");
            writer.Write(T.flashDuration + ",");
            writer.Write(T.opacity + ",");
            writer.Write(T.delay + ",");
            writer.Write(T.timeToReachOpacity + ",");
            writer.Write(T.mond + ",\n");
        }
        else if (IT is FlashMondTrial)
        {
            FlashMondTrial T = IT as FlashMondTrial;
            writer.Write(++counter + ",");
            writer.Write(++cond + ",");
            writer.Write(condRand + ",");
            writer.Write(++blk + ",");
            writer.Write(blkRand + ",");
            writer.Write("FlashMondTrial,");
            writer.Write(++trial + ",");
            writer.Write(T.random + ",");
            writer.Write(T.image + ",");
            writer.Write(T.duration + ",");
            writer.Write(T.flashDuration + ",");
            writer.Write(T.opacity + ",");
            writer.Write(T.delay + ",");
            writer.Write(T.timeToReachOpacity + ",");
            writer.Write(T.mond + ",\n");
        }
        else if (IT is MaskTrial)
        {
            MaskTrial T = IT as MaskTrial;
            writer.Write(++counter + ",");
            writer.Write(++cond + ",");
            writer.Write(condRand + ",");
            writer.Write(++blk + ",");
            writer.Write(blkRand + ",");
            writer.Write("MaskTrial,");
            writer.Write(++trial + ",");
            writer.Write(T.random + ",");
            writer.Write(T.image + ",");
            writer.Write(T.duration + ",");
            writer.Write(T.flashDuration + ",");
            writer.Write(T.opacity + ",");
            writer.Write(T.delay + ",");
            writer.Write(T.timeToReachOpacity + ",");
            writer.Write(T.mask + ",\n");
        }
        else if (IT is Response)
        {
            Response R = IT as Response;
            writer.Write(++counter + ",");
            writer.Write(++cond + ",");
            writer.Write(condRand + ",");
            writer.Write(++blk + ",");
            writer.Write(blkRand + ",");
            writer.Write("Response,");
            writer.Write(++trial + ",");
            writer.Write(R.random + ",");
            writer.Write(R.image + ",,,,,,,");
            writer.Write(R.responseTime + ",");
            writer.Write(R.up + ",");
            writer.Write(R.down + ",");
            writer.Write(R.left + ",");
            writer.Write(R.right + ",");
            writer.Write(R.response + ",\n");
        }
        else if (IT is Break)
        {
            Break B = IT as Break;
            writer.Write(++counter + ",");
            writer.Write(++cond + ",");
            writer.Write(condRand + ",");
            writer.Write(++blk + ",");
            writer.Write(blkRand + ",");
            writer.Write("Break,");
            writer.Write(++trial + ",");
            writer.Write(B.random + ",");
            writer.Write(B.image + ",");
            writer.Write(B.duration + ",\n");
        }
        writer.Close();
    }
}