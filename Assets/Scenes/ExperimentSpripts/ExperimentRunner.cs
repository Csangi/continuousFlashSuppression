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
    public Image progress;
    public GameObject progressText;

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

    //string holders which holder reponses and timing
    string responseHolder = "";
    string responseTiming = "";


    //bool for starting a response and a float for holding the time 
    bool startTimer;
    float responseTimer;
    string upRes = "", downRes = "", leftRes = "", rightRes = "";

    //exit bool for exiting half way through an experiment 
    bool exit;

    //checking for if the trial has finished running
    bool trialHasRun;

    //checking for if the entire experiment has finished as to not over lap 
    bool expIsRunning;

    //checking for if the picture has uploaded before starting the experiment
    bool picIsUploaded;

    //all response booleans
    bool response = false, introResponse = false;

    public RawImage rightPicImage, leftPicImage, leftMultiPicImage1, leftMultiPicImage2, rightMultiPicImage1, rightMultiPicImage2;

    // Start is called before the first frame update
    void Start()
    {
        right = left = expIsRunning = exit = response = expIsRunning = startTimer = false;       //setting all booleans to false incase user returns to this scene
        var createMond = FindObjectOfType<createMond>();                                                                                //getting to the mondrian script so it can draw on the blank images
                                                                                                                                        //if (!exp.mondsHaveBeenDrawn)              function not fully working properly so commented out for now
        createMond.updateMond(rightImgArr, leftImgArr);                                                                                 //calling the create mondrian script 
        right = Experiment.current.right;                                                                                               //getting correct eye dominance 
        left = Experiment.current.left;                                                                                                      //getting the whole experiement
        ID = Experiment.current.ID;                                                                                                     //grabbing id string for the output csv 
        exp.randomizeConditions();                                                                                                      //randomizing the experiment incase user returns and doesnt upload a new csv
        Debug.Log("number of conditions: " + exp.numberOfConditions);
        exp.printExperiment();
        exit = false;
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
                introResponse = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))       //escape back to creator
        {
            exit = true;
            Experiment.current.sceneToBeLoaded = 2;
            SceneManager.LoadScene(3);
        }

        if (startTimer)
            responseTimer += Time.deltaTime;
        else
            responseTimer = 0;

        if (Input.GetKeyDown(KeyCode.DownArrow))//&& !response)
        {
            responseHolder += "_" + downRes;
            responseTiming += "_" + responseTimer.ToString();
            response = true;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) // && !response)
        {
            responseHolder += "_" + upRes;
            responseTiming += "_" + responseTimer.ToString();
            response = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))// && !response)
        {
            responseHolder += "_" + rightRes;
            responseTiming += "_" + responseTimer.ToString();
            response = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))//&& !response)
        {
            responseHolder += "_" + leftRes;
            responseTiming += "_" + responseTimer.ToString();
            response = true;
        }
        /*
        if (Gamepad.current[GamepadButton.RightTrigger].isPressed)
            introResponse = true;

        if (Gamepad.current[GamepadButton.DpadDown].isPressed && !response)
        {
            responseHolder += "_" + downRes;
            responseTiming += "_" + responseTimer.ToString();
            response = true;
        }

        if (Gamepad.current[GamepadButton.DpadUp].isPressed && !response)
        {
            responseHolder += "_" + upRes;
            responseTiming += "_" + responseTimer.ToString();
            response = true;
        }

        if (Gamepad.current[GamepadButton.DpadRight].isPressed && !response)
        {
            responseHolder += "_" + rightRes;
            responseTiming += "_" + responseTimer.ToString();
            response = true;
        }

        if (Gamepad.current[GamepadButton.DpadLeft].isPressed && !response)
        {
            responseHolder += "_" + leftRes;
            responseTiming += "_" + responseTimer.ToString();
            response = true;
        }
        */
    }
    IEnumerator runExperiment(RawImage changeImage, RawImage[] ImgArr)
    {
        RawImage oppositeChangeImage, multiImage1, multiImage2;
        if (changeImage == rightPicImage)
        {
            oppositeChangeImage = leftPicImage;
            multiImage1 = rightMultiPicImage1;
            multiImage2 = rightMultiPicImage2;
        }
        else
        {
            oppositeChangeImage = rightPicImage;
            multiImage1 = leftMultiPicImage1;
            multiImage2 = leftMultiPicImage2;
        }

        float percentAmount = 0;
        expIsRunning = true;
        int counter = 0;
        int conditionIndex = 0, blockIndex = 0, trialIndex = 0;
        for (int i = 0; i < exp.numberOfConditions; ++i)
        {
            conditionIndex = exp.index[i];
            for (int j = 0; j < exp.conditions[conditionIndex].numberOfBlocks; ++j)
            {
                blockIndex = exp.conditions[conditionIndex].index[j];
                for (int k = 0; k < exp.conditions[conditionIndex].blocks[blockIndex].numberOfTrials; ++k, ++counter)
                {
                    trialHasRun = false;
                    picIsUploaded = false;
                    trialIndex = exp.conditions[conditionIndex].blocks[blockIndex].index[k];
                    percentAmount = (float)counter / (float)exp.count;
                    progress.fillAmount = percentAmount;
                    progressText.GetComponent<Text>().text = (100 * percentAmount).ToString("0.00") + "%";
                    if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is MondTrial)
                    {
                        if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].hasMultipleStims)
                        {
                            StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, multiImage1));
                            yield return new WaitUntil(() => picIsUploaded);
                            picIsUploaded = false;
                            StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2Path, multiImage2));
                            yield return new WaitUntil(() => picIsUploaded);
                            StartCoroutine(runMultiTrial(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MondTrial, multiImage1, multiImage2, ImgArr));
                            yield return new WaitUntil(() => trialHasRun);
                            yield return new WaitForSecondsRealtime(1);
                        }
                        else
                        {
                            StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, changeImage));
                            yield return new WaitUntil(() => picIsUploaded);

                            StartCoroutine(runTrial(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MondTrial, changeImage, ImgArr));
                            yield return new WaitUntil(() => trialHasRun);
                            yield return new WaitForSecondsRealtime(1);
                        }
                    }
                    else if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is FlashMondTrial)
                    {
                        if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].hasMultipleStims)
                        {
                            StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, multiImage1));
                            yield return new WaitUntil(() => picIsUploaded);
                            picIsUploaded = false;
                            StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2Path, multiImage2));
                            yield return new WaitUntil(() => picIsUploaded);
                            StartCoroutine(runMultiTrial(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as FlashMondTrial, multiImage1, multiImage2, ImgArr));
                            yield return new WaitUntil(() => trialHasRun);
                            yield return new WaitForSecondsRealtime(1);
                        }
                        else
                        {
                            StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, changeImage));
                            yield return new WaitUntil(() => picIsUploaded);

                            StartCoroutine(runTrial(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as FlashMondTrial, changeImage, ImgArr));
                            yield return new WaitUntil(() => trialHasRun);
                            yield return new WaitForSecondsRealtime(1);
                        }
                    }
                    else if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is MaskTrial)
                    {
                        if (exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].hasMultipleStims)
                        {
                            StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath, multiImage1));
                            yield return new WaitUntil(() => picIsUploaded);
                            picIsUploaded = false;
                            StartCoroutine(MyGetTexture(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2Path, multiImage2));
                            yield return new WaitUntil(() => picIsUploaded);
                            picIsUploaded = false;
                            Debug.Log((exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath);
                            StartCoroutine(MyGetTexture((exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath, oppositeChangeImage));
                            yield return new WaitUntil(() => picIsUploaded);

                            StartCoroutine(runMultiTrial(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial, multiImage1, multiImage2, oppositeChangeImage, ImgArr));
                            yield return new WaitUntil(() => trialHasRun);
                            yield return new WaitForSecondsRealtime(1);
                        }
                        else
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

                    writeTrial(exp.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex], conditionIndex, exp.conditions[conditionIndex].random, blockIndex, exp.conditions[conditionIndex].blocks[blockIndex].random, trialIndex, counter);
                }
            }
        }
        expIsRunning = false;
    }

    //A break is like a instruction, but there is no response
    IEnumerator runBreak(Break I, RawImage changeImage1, RawImage changeImage2)
    {
        changeImage1.enabled = true;
        changeImage2.enabled = true;
        yield return new WaitForSecondsRealtime(I.duration / 1000);

        changeImage1.enabled = false;
        changeImage2.enabled = false;
        trialHasRun = true;
        yield break;
    }

    //Response and instruction are both very similar only difference is we dont take the response down for the instruction
    IEnumerator runResponse(Response R, RawImage changeImage1, RawImage changeImage2)
    {
        changeImage1.enabled = true;        //first we turn on both of the images
        changeImage2.enabled = true;

        leftRes = R.left; rightRes = R.right; upRes = R.up; downRes = R.down;
        response = false;        //set the responses and which arrow to false 
        startTimer = true;                                              //we start the timer
        responseTiming = responseHolder = "";

        yield return new WaitUntil(() => response);                     //we wait until there is some response 
        R.responseTime = responseTimer;
        startTimer = false;
        R.response = responseHolder.Remove(0, 1);
        responseHolder = responseTiming = "";
        changeImage1.enabled = false;
        changeImage2.enabled = false;
        trialHasRun = true;
        response = false;
        yield break;
    }

    IEnumerator runInstruction(Instruction I, RawImage changeImage1, RawImage changeImage2)
    {
        introResponse = false;
        changeImage1.enabled = true;
        changeImage2.enabled = true;

        startTimer = true;
        yield return new WaitForSecondsRealtime(I.duration / 1000);
        yield return new WaitUntil(() => introResponse);
        if (exit)
            yield break;
        I.responseTime = responseTimer;
        startTimer = false;
        changeImage1.enabled = false;
        changeImage2.enabled = false;
        trialHasRun = true;
        introResponse = false;
        yield break;
    }

    IEnumerator runTrial(MondTrial T, RawImage changeImage, RawImage[] ImgArr)
    {
        changeImage.color = new Color(0f, 0f, 0f, 0f);
        changeImage.enabled = true;
        //num of iterations throught the  = Duration / flash duration 
        int numOfIt = T.duration / T.flashDuration;
        //mask delay in terms of i 
        int iMaskDelay = T.maskDelay / T.duration;
        //Flash Delay = static delay / flash duration
        int iStaticDelay = T.staticDelay / T.flashDuration;
        //opacity increase each trial = opacity / i static delay ) / 100 because the float value range for alpha is from 0 - 1
        float opacityIncrease = (float)(T.opacity / (numOfIt - iStaticDelay)) / 100;
        float opacity = new float();
        int holder = 0, x = 0;   //holds the last index in the array that is currently active
        float waitingPeriod = (float)T.flashDuration / 1000;        //the time between each waiting
        response = false;        //set the responses and which arrow to false 
        upRes = T.up; downRes = T.down; leftRes = T.left; rightRes = T.right;
        responseTiming = responseHolder = "";
        if (T.staticDelay == 0)                  //If the flash delay is 0 thend the opacity will be set to the set opacity at the beginning and it will 
        {                                               //not increase throughout
            changeImage.color = new Color(T.opacity, T.opacity, T.opacity, T.opacity);
            iStaticDelay = numOfIt + 1;
        }
        Debug.Log("running a trial");
        Debug.Log("opacity increase: " + opacityIncrease);
        Debug.Log("waiting period: " + waitingPeriod);
        Debug.Log("i static delay: " + iStaticDelay);
        Debug.Log("i Mask Delay: " + iMaskDelay);
        startTimer = true;
        for (int i = 0; i < numOfIt; ++i)   //i will be the number if iterations and we will use i as a flag to when each different action should start
        {
            if (i >= iMaskDelay)     //once i reaches this point it will begin the mondrian flashing
            {   //each new mondrian will be found using a do while loop
                Debug.Log("-------> " + iMaskDelay);
                do
                {
                    x = Random.Range(exp.Mondrians[T.mond].minRange, exp.Mondrians[T.mond].maxRange);
                } while (x == holder);
                ImgArr[x].enabled = true;
                ImgArr[holder].enabled = false;
                holder = x;
            }
            //once i reaches the flash delay it will begin fading in the image
            if (i >= iStaticDelay)
            {
                Debug.Log("-------> " + opacity);
                //opacity will increase with each iteration until it reaches the specified opacity
                opacity += opacityIncrease;
                changeImage.color = new Color(opacity, opacity, opacity, opacity);
            }

            if (exit)           //if trial exited
                yield break;

            if (response && T.isResponse && T.responseStop)     //if the trial is a response
                break;

            yield return new WaitForSecondsRealtime(waitingPeriod);     //we will wait for the specified waiting period
            Debug.Log(i + " " + x);
        }
        if (T.isResponse)
            if (responseHolder == "")
            {
                T.response = "Nothing entered by user";
                T.responseTime = "0";
            }
            else
            {
                Debug.Log(responseHolder.Remove(0, 1));
                T.response = System.String.Copy(responseHolder.Remove(0, 1));
                T.responseTime = System.String.Copy(responseTiming.Remove(0, 1));
            }
        upRes = downRes = leftRes = rightRes = responseTiming = responseHolder = "";
        response = startTimer = false;
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
        //num of iterations throught the  = Duration / flash duration 
        int numOfIt = T.duration / T.flashDuration;
        //mask delay in terms of i 
        int iMaskDelay = T.maskDelay / T.duration;
        //Flash Delay = static delay / flash duration
        int iStaticDelay = T.staticDelay / T.flashDuration;
        //opacity increase each trial = opacity / i static delay ) / 100 because the float value range for alpha is from 0 - 1
        float opacityIncrease = (float)(T.opacity / (numOfIt - iStaticDelay)) / 100;
        float opacity = new float();
        int holder = 0, x = 0; //holds the last index in the array that is currently active
        float flashPeriod = T.flashPeriod / 1000;     //the time in ms that each image is on    (image off screen time)
        float waitingPeriod = ((float)T.flashDuration - T.flashPeriod) / 1000;  //the total time waiting between each trial   (image on screen time)
        response = false;        //set the responses and which arrow to false 
        upRes = T.up; downRes = T.down; leftRes = T.left; rightRes = T.right;
        responseTiming = responseHolder = "";
        Debug.Log("Trial values:");
        Debug.Log("Static Delay: " + iStaticDelay);
        Debug.Log("opacity Increase: " + opacityIncrease);
        Debug.Log("flash Period: " + flashPeriod);
        Debug.Log("waiting Period: " + waitingPeriod);
        Debug.Log("iMask Delay: " + iMaskDelay);
        Debug.Log("num of Iterations: " + numOfIt);
        if (T.staticDelay == 0)                  //If the flash delay is 0 thend the opacity will be set to the set opacity at the beginning and it will 
        {                                               //not increase throughout
            changeImage.color = new Color(T.opacity, T.opacity, T.opacity, T.opacity);
            iStaticDelay = numOfIt + 1;
        }
        startTimer = true;
        Debug.Log("running a trial");
        for (int i = 0; i < numOfIt; ++i)   //i will be the number if iterations and we will use i as a flag to when each different action should start
        {
            if (i >= iMaskDelay)     //once i reaches this point it will begin the mondrian flashing
            {   //each new mondrian will be found using a do while loop
                do
                {
                    x = Random.Range(exp.Mondrians[T.mond].minRange, exp.Mondrians[T.mond].maxRange);
                } while (x == holder);
                ImgArr[holder].enabled = false;
                changeImage.enabled = false;
                yield return new WaitForSecondsRealtime(flashPeriod);
                changeImage.enabled = true;
                ImgArr[x].enabled = true;
                holder = x;
            }
            //once i reaches the flash delay it will begin fading in the image
            if (i >= iStaticDelay)
            {
                Debug.Log(opacity);
                //opacity will increase with each iteration until it reaches the specified opacity
                opacity += opacityIncrease;
                changeImage.color = new Color(opacity, opacity, opacity, opacity);
            }

            if (exit)           //if trial exited
                yield break;

            if (response && T.isResponse && T.responseStop)     //if the trial is a response
                break;

            yield return new WaitForSecondsRealtime(waitingPeriod);     //we will wait for the specified waiting period
            Debug.Log(i + " " + x);
        }
        if (T.isResponse)
            if (responseHolder == "")
            {
                T.response = "Nothing entered by user";
                T.responseTime = "0";
            }
            else
            {
                Debug.Log(responseHolder.Remove(0, 1));
                T.response = System.String.Copy(responseHolder.Remove(0, 1));
                T.responseTime = System.String.Copy(responseTiming.Remove(0, 1));
            }
        upRes = downRes = leftRes = rightRes = responseTiming = responseHolder = "";
        response = startTimer = false;
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
        //num of iterations throught the  = Duration / flash duration 
        int numOfIt = T.duration / T.flashDuration;
        //mask delay in terms of i 
        int iMaskDelay = T.maskDelay / T.duration;
        //Flash Delay = static delay / flash duration
        int iStaticDelay = T.staticDelay / T.flashDuration;
        //opacity increase each trial = opacity / i static delay ) / 100 because the float value range for alpha is from 0 - 1
        float opacityIncrease = (float)(T.opacity / (numOfIt - iStaticDelay)) / 100;
        float opacity = new float();
        float flashPeriod = T.flashPeriod / 1000;     //the time in ms that each image is on    (image off screen time)
        float waitingPeriod = ((float)T.flashDuration - T.flashPeriod) / 1000;  //the total time waiting between each trial   (image on screen time)
        response = false;        //set the responses and which arrow to false 
        upRes = T.up; downRes = T.down; leftRes = T.left; rightRes = T.right;
        responseTiming = responseHolder = "";
        Debug.Log("running a trial");
        if (T.staticDelay == 0)                  //If the flash delay is 0 thend the opacity will be set to the set opacity at the beginning and it will 
        {                                               //not increase throughout
            changeImage.color = new Color(T.opacity, T.opacity, T.opacity, T.opacity);
            iStaticDelay = numOfIt + 1;
        }
        startTimer = true;
        Debug.Log("running a trial" + flashPeriod + " " + waitingPeriod);
        for (int i = 0; i < numOfIt; ++i)   //i will be the number if iterations and we will use i as a flag to when each different action should start
        {
            if (i >= iStaticDelay) //once i reaches the flash delay it will begin fading in the image
            {
                Debug.Log(opacity);
                //opacity will increase with each iteration until it reaches the specified opacity
                opacity += opacityIncrease;
                changeImage.enabled = false;
                oppositeChangeImage.enabled = false;
                yield return new WaitForSecondsRealtime(flashPeriod);
                oppositeChangeImage.enabled = true;
                changeImage.enabled = true;
                changeImage.color = new Color(opacity, opacity, opacity, opacity);
            }
            else if (i >= iMaskDelay)
            {
                changeImage.enabled = false;
                oppositeChangeImage.enabled = false;
                yield return new WaitForSecondsRealtime(flashPeriod);
                oppositeChangeImage.enabled = true;
                changeImage.enabled = true;
            }


            if (exit)           //if trial exited
                yield break;

            if (response && T.isResponse && T.responseStop)     //if the trial is a response
                break;

            yield return new WaitForSecondsRealtime(waitingPeriod);     //we will wait for the specified waiting period
            Debug.Log(i);
        }
        if (T.isResponse)
            if (responseHolder == "")
            {
                T.response = "Nothing entered by user";
                T.responseTime = "0";
            }
            else
            {
                Debug.Log(responseHolder.Remove(0, 1));
                T.response = System.String.Copy(responseHolder.Remove(0, 1));
                T.responseTime = System.String.Copy(responseTiming.Remove(0, 1));
            }
        upRes = downRes = leftRes = rightRes = responseTiming = responseHolder = "";
        response = startTimer = false;
        changeImage.enabled = false;    //turn off the main image
        oppositeChangeImage.enabled = false;
        trialHasRun = true;
        changeImage.color = new Color(1f, 1f, 1f, 1f);
        yield break;
    }

    IEnumerator runMultiTrial(MondTrial T, RawImage changeImage1, RawImage changeImage2, RawImage[] ImgArr)
    {
        changeImage1.color = new Color(0f, 0f, 0f, 0f);
        changeImage2.color = new Color(0f, 0f, 0f, 0f);
        changeImage1.enabled = true;
        changeImage2.enabled = true;
        //num of iterations throught the  = Duration / flash duration 
        int numOfIt = T.duration / T.flashDuration;
        //mask delay in terms of i 
        int iMaskDelay = T.maskDelay / T.duration;
        //Flash Delay = static delay / flash duration
        int iStaticDelay = T.staticDelay / T.flashDuration;
        //opacity increase each trial = opacity / i static delay ) / 100 because the float value range for alpha is from 0 - 1
        float opacityIncrease = (float)(T.opacity / (numOfIt - iStaticDelay)) / 100;
        float opacity = new float();
        int holder = 0, x = 0; //holds the last index in the array that is currently active
        float waitingPeriod = (float)T.flashDuration / 1000;        //the time between each waiting
        response = false;        //set the responses  
        upRes = T.up; downRes = T.down; leftRes = T.left; rightRes = T.right;
        responseTiming = responseHolder = "";
        Debug.Log("running a trial");
        if (T.staticDelay == 0)                  //If the flash delay is 0 thend the opacity will be set to the set opacity at the beginning and it will 
        {                                               //not increase throughout
            changeImage1.color = new Color(T.opacity, T.opacity, T.opacity, T.opacity);
            changeImage2.color = new Color(T.opacity, T.opacity, T.opacity, T.opacity);
            iStaticDelay = numOfIt + 1;
        }
        startTimer = true;
        Debug.Log("running a trial");
        for (int i = 0; i < numOfIt; ++i)   //i will be the number if iterations and we will use i as a flag to when each different action should start
        {
            if (i >= iMaskDelay)     //once i reaches this point it will begin the mondrian flashing
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
            if (i >= iStaticDelay)
            {
                Debug.Log(opacity);
                //opacity will increase with each iteration until it reaches the specified opacity
                opacity += opacityIncrease;
                changeImage1.color = new Color(opacity, opacity, opacity, opacity);
                changeImage2.color = new Color(opacity, opacity, opacity, opacity);
            }

            if (exit)           //if trial exited
                yield break;

            if (response && T.isResponse && T.responseStop)     //if the trial is a response
                break;

            yield return new WaitForSecondsRealtime(waitingPeriod);     //we will wait for the specified waiting period
            Debug.Log(i + " " + x);
        }
        if (T.isResponse)
            if (responseHolder == "")
            {
                T.response = "Nothing entered by user";
                T.responseTime = "0";
            }
            else
            {
                Debug.Log(responseHolder.Remove(0, 1));
                T.response = System.String.Copy(responseHolder.Remove(0, 1));
                T.responseTime = System.String.Copy(responseTiming.Remove(0, 1));
            }
        upRes = downRes = leftRes = rightRes = responseTiming = responseHolder = "";
        response = startTimer = false;
        ImgArr[x].enabled = false;      //then turn off the last mondrian 
        changeImage1.enabled = false;    //turn off the main image
        changeImage2.enabled = false;
        trialHasRun = true;
        changeImage1.color = new Color(1f, 1f, 1f, 1f);
        changeImage2.color = new Color(1f, 1f, 1f, 1f);
        yield break;
    }

    IEnumerator runMultiTrial(FlashMondTrial T, RawImage changeImage1, RawImage changeImage2, RawImage[] ImgArr)
    {
        changeImage1.color = new Color(0f, 0f, 0f, 0f);
        changeImage2.color = new Color(0f, 0f, 0f, 0f);
        changeImage1.enabled = true;
        changeImage2.enabled = true;
        //num of iterations throught the  = Duration / flash duration 
        int numOfIt = T.duration / T.flashDuration;
        //mask delay in terms of i 
        int iMaskDelay = T.maskDelay / T.duration;
        //Flash Delay = static delay / flash duration
        int iStaticDelay = T.staticDelay / T.flashDuration;
        //opacity increase each trial = opacity / i static delay ) / 100 because the float value range for alpha is from 0 - 1
        float opacityIncrease = (float)(T.opacity / (numOfIt - iStaticDelay)) / 100;
        float opacity = new float();
        int holder = 0, x = 0; //holds the last index in the array that is currently active
        float flashPeriod = T.flashPeriod / 1000;     //the time in ms that each image is on    (image off screen time)
        float waitingPeriod = ((float)T.flashDuration - T.flashPeriod) / 1000;  //the total time waiting between each trial   (image on screen time)
        response = false;        //set the responses  
        upRes = T.up; downRes = T.down; leftRes = T.left; rightRes = T.right;
        responseTiming = responseHolder = "";
        Debug.Log("running a trial");
        if (T.staticDelay == 0)                  //If the flash delay is 0 thend the opacity will be set to the set opacity at the beginning and it will 
        {                                               //not increase throughout
            changeImage1.color = new Color(T.opacity, T.opacity, T.opacity, T.opacity);
            changeImage2.color = new Color(T.opacity, T.opacity, T.opacity, T.opacity);
            iStaticDelay = numOfIt + 1;
        }
        startTimer = true;
        Debug.Log("running a trial");
        for (int i = 0; i < numOfIt; ++i)   //i will be the number if iterations and we will use i as a flag to when each different action should start
        {
            if (i >= iMaskDelay)     //once i reaches this point it will begin the mondrian flashing
            {   //each new mondrian will be found using a do while loop
                do
                {
                    x = Random.Range(exp.Mondrians[T.mond].minRange, exp.Mondrians[T.mond].maxRange);
                } while (x == holder);
                ImgArr[holder].enabled = false;
                changeImage1.enabled = false;
                changeImage2.enabled = false;
                yield return new WaitForSecondsRealtime(flashPeriod);
                changeImage1.enabled = true;
                changeImage2.enabled = true;
                ImgArr[x].enabled = true;
                holder = x;
            }
            //once i reaches the flash delay it will begin fading in the image
            if (i >= iStaticDelay)
            {
                Debug.Log(opacity);
                //opacity will increase with each iteration until it reaches the specified opacity
                opacity += opacityIncrease;
                changeImage1.color = new Color(opacity, opacity, opacity, opacity);
                changeImage2.color = new Color(opacity, opacity, opacity, opacity);
            }

            if (exit)           //if trial exited
                yield break;

            if (response && T.isResponse && T.responseStop)         //if the trial is a response
                break;

            yield return new WaitForSecondsRealtime(waitingPeriod);     //we will wait for the specified waiting period
            Debug.Log(i + " " + x);
        }
        if (T.isResponse)
            if (responseHolder == "")
            {
                T.response = "Nothing entered by user";
                T.responseTime = "0";
            }
            else
            {
                Debug.Log(responseHolder.Remove(0, 1));
                T.response = System.String.Copy(responseHolder.Remove(0, 1));
                T.responseTime = System.String.Copy(responseTiming.Remove(0, 1));
            }
        upRes = downRes = leftRes = rightRes = responseTiming = responseHolder = "";
        response = startTimer = false;
        ImgArr[x].enabled = false;      //then turn off the last mondrian 
        changeImage1.enabled = false;    //turn off the main image
        changeImage2.enabled = false;
        trialHasRun = true;
        changeImage1.color = new Color(1f, 1f, 1f, 1f);
        changeImage2.color = new Color(1f, 1f, 1f, 1f);
        yield break;
    }

    IEnumerator runMultiTrial(MaskTrial T, RawImage changeImage1, RawImage changeImage2, RawImage oppositeChangeImage, RawImage[] ImgArr)
    {
        changeImage1.color = new Color(0f, 0f, 0f, 0f);
        changeImage1.enabled = true;
        changeImage2.color = new Color(0f, 0f, 0f, 0f);
        changeImage2.enabled = true;
        oppositeChangeImage.enabled = true;
        //num of iterations throught the  = Duration / flash duration 
        int numOfIt = T.duration / T.flashDuration;
        //mask delay in terms of i 
        int iMaskDelay = T.maskDelay / T.duration;
        //Flash Delay = static delay / flash duration
        int iStaticDelay = T.staticDelay / T.flashDuration;
        //opacity increase each trial = opacity / i static delay ) / 100 because the float value range for alpha is from 0 - 1
        float opacityIncrease = (float)(T.opacity / (numOfIt - iStaticDelay)) / 100;
        float opacity = new float();
        float flashPeriod = T.flashPeriod / 1000;     //the time in ms that each image is on    (image off screen time)
        float waitingPeriod = ((float)T.flashDuration - T.flashPeriod) / 1000;  //the total time waiting between each trial   (image on screen time)
        response = false;        //set the responses  
        upRes = T.up; downRes = T.down; leftRes = T.left; rightRes = T.right;
        responseTiming = responseHolder = "";
        Debug.Log("running a trial");

        if (T.staticDelay == 0)                  //If the flash delay is 0 thend the opacity will be set to the set opacity at the beginning and it will 
        {                                               //not increase throughout
            changeImage1.color = new Color(T.opacity, T.opacity, T.opacity, T.opacity);
            changeImage2.color = new Color(T.opacity, T.opacity, T.opacity, T.opacity);
            iStaticDelay = numOfIt + 1;
        }
        startTimer = true;
        Debug.Log("running a trial" + flashPeriod + " " + waitingPeriod);
        for (int i = 0; i < numOfIt; ++i)   //i will be the number if iterations and we will use i as a flag to when each different action should start
        {

            if (i >= iStaticDelay) //once i reaches the flash delay it will begin fading in the image
            {
                Debug.Log(opacity);
                //opacity will increase with each iteration until it reaches the specified opacity
                opacity += opacityIncrease;
                changeImage1.enabled = false;
                changeImage2.enabled = false;
                oppositeChangeImage.enabled = false;
                yield return new WaitForSecondsRealtime(flashPeriod);
                oppositeChangeImage.enabled = true;
                changeImage1.enabled = true;
                changeImage2.enabled = true;
                changeImage1.color = new Color(opacity, opacity, opacity, opacity);
                changeImage2.color = new Color(opacity, opacity, opacity, opacity);
            }
            else if (i >= iMaskDelay)
            {
                changeImage1.enabled = false;
                changeImage2.enabled = false;
                oppositeChangeImage.enabled = false;
                yield return new WaitForSecondsRealtime(flashPeriod);
                oppositeChangeImage.enabled = true;
                changeImage1.enabled = true;
                changeImage2.enabled = true;
            }

            if (exit)               //if trial exited
                yield break;

            if (response && T.isResponse && T.responseStop)     //if the trial is a response 
                break;

            yield return new WaitForSecondsRealtime(waitingPeriod);     //we will wait for the specified waiting period
            Debug.Log(i);
        }
        if (T.isResponse)
            if (responseHolder == "")
            {
                T.response = "Nothing entered by user";
                T.responseTime = "0";
            }
            else
            {
                Debug.Log(responseHolder.Remove(0, 1));
                T.response = System.String.Copy(responseHolder.Remove(0, 1));
                T.responseTime = System.String.Copy(responseTiming.Remove(0, 1));
            }
        upRes = downRes = leftRes = rightRes = responseTiming = responseHolder = "";
        response = startTimer = false;
        changeImage1.enabled = false;    //turn off the main image
        changeImage2.enabled = false;
        oppositeChangeImage.enabled = false;
        trialHasRun = true;
        changeImage1.color = new Color(1f, 1f, 1f, 1f);
        changeImage2.color = new Color(1f, 1f, 1f, 1f);
        yield break;
    }

    public void writeTrial(Item IT, int cond, bool condRand, int blk, bool blkRand, int trial, int counter)     //simple writer to an external csv named with the input ID
    {   //using the input path
        string fileName = "";
        if (exp.outputPath != string.Empty)
            fileName = exp.outputPath + "/" + ID + ".csv";
        else
            fileName = exp.directory + "/" + ID + ".csv";
        //System.IO.FileStream writer = new System.IO.FileStream(fileName, FileMode.Append);
        using (StreamWriter writer = File.AppendText(fileName))
        {
            if (!writerStart)
            {
                writer.Write("Trial Count,Condition,Cond Rand,Block,Block Rand,Trial Type,Input Order,Trial Rand,Image,Duration,Flash Duration,Opacity,Mask Delay,Static Delay,Mondrian,Response Time,Up,Down,Left,Right,Answer,Flash period,Multi Response\n");
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
            else if (IT is MondTrial)
            {
                MondTrial T = IT as MondTrial;
                writer.Write(++counter + ",");
                writer.Write(++cond + ",");
                writer.Write(condRand + ",");
                writer.Write(++blk + ",");
                writer.Write(blkRand + ",");
                if (T.hasMultipleStims)
                {
                    writer.Write("MultiMondrianTrial,");
                    writer.Write(++trial + ",");
                    writer.Write(T.random + ",");
                    writer.Write(T.image + "_" + T.image2 + ",");
                }
                else
                {
                    writer.Write("MondrianTrial,");
                    writer.Write(++trial + ",");
                    writer.Write(T.random + ",");
                    writer.Write(T.image + ",");
                }
                writer.Write(T.duration + ",");
                writer.Write(T.flashDuration + ",");
                writer.Write(T.opacity + ",");
                writer.Write(T.maskDelay + ",");
                writer.Write(T.staticDelay + ",");
                writer.Write(T.mond + ",");
                if (T.isResponse)
                {
                    writer.Write(T.responseTime + ",");
                    writer.Write(T.up + ",");
                    writer.Write(T.down + ",");
                    writer.Write(T.left + ",");
                    writer.Write(T.right + ",");
                    writer.Write(T.response + ",,");
                    writer.Write((!T.responseStop) + ",");
                    responseHolder = "";
                    responseTiming = "";
                }
                writer.Write("\n");
            }
            else if (IT is FlashMondTrial)
            {
                FlashMondTrial T = IT as FlashMondTrial;
                writer.Write(++counter + ",");
                writer.Write(++cond + ",");
                writer.Write(condRand + ",");
                writer.Write(++blk + ",");
                writer.Write(blkRand + ",");
                if (T.hasMultipleStims)
                {
                    writer.Write("MultiMondrianTrial,");
                    writer.Write(++trial + ",");
                    writer.Write(T.random + ",");
                    writer.Write(T.image + "_" + T.image2 + ",");
                }
                else
                {
                    writer.Write("MondrianTrial,");
                    writer.Write(++trial + ",");
                    writer.Write(T.random + ",");
                    writer.Write(T.image + ",");
                }
                writer.Write(T.duration + ",");
                writer.Write(T.flashDuration + ",");
                writer.Write(T.opacity + ",");
                writer.Write(T.maskDelay + ",");
                writer.Write(T.staticDelay + ",");
                writer.Write(T.mond + ",");
                if (T.isResponse)
                {
                    writer.Write(T.responseTime + ",");
                    writer.Write(T.up + ",");
                    writer.Write(T.down + ",");
                    writer.Write(T.left + ",");
                    writer.Write(T.right + ",");
                    writer.Write(T.response + ",");
                    responseHolder = "";
                    responseTiming = "";
                }
                else
                    writer.Write(",,,,,,");
                writer.Write(T.flashPeriod + ",");
                writer.Write((!T.responseStop) + "\n");
            }
            else if (IT is MaskTrial)
            {
                MaskTrial T = IT as MaskTrial;
                writer.Write(++counter + ",");
                writer.Write(++cond + ",");
                writer.Write(condRand + ",");
                writer.Write(++blk + ",");
                writer.Write(blkRand + ",");
                if (T.hasMultipleStims)
                {
                    writer.Write("MultiMaskTrial,");
                    writer.Write(++trial + ",");
                    writer.Write(T.random + ",");
                    writer.Write(T.image + "&" + T.image2 + ",");
                }
                else
                {
                    writer.Write("MaskTrial,");
                    writer.Write(++trial + ",");
                    writer.Write(T.random + ",");
                    writer.Write(T.image + ",");
                }
                writer.Write(T.duration + ",");
                writer.Write(T.flashDuration + ",");
                writer.Write(T.opacity + ",");
                writer.Write(T.maskDelay + ",");
                writer.Write(T.staticDelay + ",");
                writer.Write(T.mask + ",");
                if (T.isResponse)
                {
                    writer.Write(T.responseTime + ",");
                    writer.Write(T.up + ",");
                    writer.Write(T.down + ",");
                    writer.Write(T.left + ",");
                    writer.Write(T.right + ",");
                    writer.Write(T.response + ",");
                    responseHolder = "";
                    responseTiming = "";
                }
                else
                    writer.Write(",,,,,,");
                writer.Write(T.flashPeriod + ",");
                writer.Write((!T.responseStop) + "\n");
            }
            writer.Close();
        }
    }
}