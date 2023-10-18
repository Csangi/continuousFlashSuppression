using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class UIManager : MonoBehaviour
{
    //these control the toggles for left and right eye on the left side of the screen
    public Toggle rightEye;
    public Toggle leftEye;

    //for some reason, CreateLog wont recognize the toggles. so use this public bool
    public GameObject dominantEyeError;

    //to get the csv file destination
    private string path;

    //the mondrian csv path
    private string mondPath;

    //Experiment object system
    public Experiment experiment = Experiment.current; //= Experiment.current;

    //test image
    public RawImage rawImage;

    private bool uploadSuccessfull;

    private bool hasStarted;

    public GameObject idInputfield;
    public GameObject outputPathInputfield;
    public GameObject conditionOrderInputfield;
    public GameObject idTextDisplay;
    public GameObject conditionOrderTextDisplay;
    public GameObject uploadErrorText;
    public GameObject outputPathErrorText;

    private void starting()
    {
        Debug.Log("Waiting HERE AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        if (experiment.hasUploaded)
            returnedFromExp();

        if (experiment.args.Length > 1 && !experiment.hasUploaded)
        {
            Debug.Log("Uploading Args");
            experiment.ID = experiment.args[2];

            if (experiment.args[3] == "right")
            {
                experiment.right = true;
                Debug.Log("Eye is set to: right");
                //SR.WriteLine("Eye is set to: right");
            }
            else if (experiment.args[3] == "left")
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

            if (experiment.args.Length >= 5)
            {
                if (Directory.Exists(experiment.args[4]))
                    experiment.outputPath = experiment.args[4];
            }

            uploadSuccessfull = true;
            int numberBlock = 0, numberCondition = 0;
            //set path to empty string
            path = "";
            mondPath = "";

            //this actually opens the filepanel to allow the user to choose a file. the last variable is the file type - limiting to csv for now
            try
            {
                if (experiment.hasUploaded)                     //if the user has already uploaded an experiment we want to clear that out so we can
                    experiment.clearExperiment();               //enter the new data as it is most likely that ther are reuploading becuase of an error
                experiment.addCondition(new Condition(true));                       //no matter what we will still need to add the first condition and block
                experiment.conditions[numberCondition].addBlocks(new Block(false));
                if (experiment.Mondrians.Count() == 0)
                    experiment.Mondrians.Add("0", new Mondrian("0", 1, true, 5, 15, 5, 15, 5000));       //here we add the default mondrian if the user don't want to make their own 
                experiment.mondsHaveBeenDrawn = false;
                //convert string[] into normal string
                uploadExperiment(experiment.args[1], numberCondition, numberBlock);
            }
            catch (IOException)
            {
                uploadErrorText.GetComponent<Text>().color = Color.red;
                uploadErrorText.GetComponent<Text>().text = "There was an error with the upload (make sure that the csv is closed in all editors, the file cannot be read from 2 places at once";
                uploadSuccessfull = false;
            }
            catch (IndexOutOfRangeException)
            {
                uploadErrorText.GetComponent<Text>().color = Color.red;
                uploadErrorText.GetComponent<Text>().text = "Something was wrong with the formating of your upload please make sure that conditions, blocks, and trials are in sequential order";
                uploadSuccessfull = false;
            }
            catch (NullReferenceException)
            {
                uploadErrorText.GetComponent<Text>().color = Color.red;
                uploadErrorText.GetComponent<Text>().text = "Something was wrong with the formating of your upload please make sure that conditions, blocks, and trials are in sequential order";
                uploadSuccessfull = false;
            }

            if (experiment.args.Length == 6)
            {
                changeConditionOrder(experiment.args[5]);
            }

            if (uploadSuccessfull)
            {
                Experiment.current.sceneToBeLoaded = 4;
                SceneManager.LoadScene(3);
            }

            returnedFromExp();
            Debug.Log("Was the upload a success?: " + uploadSuccessfull);
        }

    }

    public void Start()
    {
        starting();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartGame();
    }

            private void returnedFromExp()
    {
        if (experiment.successfulUpload)
        {
            uploadSuccessfull = true;
            uploadErrorText.GetComponent<Text>().color = Color.white;
            uploadErrorText.GetComponent<Text>().text = "Upload Successfull";
        }

        if (experiment.right)                         //if only the right eye is on 
        {
            GameObject.Find("rightEye").GetComponent<Toggle>().enabled = true;
            dominantEyeError.GetComponent<Text>().color = Color.white;
            dominantEyeError.GetComponent<Text>().text = "Right eye dominance is " + experiment.right;
        }
        else if (experiment.left)                          //same logic for if the right eye is on
        {
            GameObject.Find("leftEye").GetComponent<Toggle>().enabled = true;
            dominantEyeError.GetComponent<Text>().color = Color.white;
            dominantEyeError.GetComponent<Text>().text = "Left eye dominance is " + experiment.left;
        }

        if (experiment.outputPath != string.Empty)
            if (Directory.Exists(experiment.outputPath))
            {
                outputPathErrorText.GetComponent<Text>().color = Color.white;
                outputPathErrorText.GetComponent<Text>().text = experiment.outputPath;
            }
            else
            {
                outputPathErrorText.GetComponent<Text>().color = Color.red;
                outputPathErrorText.GetComponent<Text>().text = "Could not find directory";
            }

        if (!String.IsNullOrEmpty(experiment.ID))
        {
            idTextDisplay.GetComponent<Text>().color = Color.white;
            idTextDisplay.GetComponent<Text>().text = experiment.ID;
        }
        //experiment.printExperiment();
    }

    //---------------------------------------------------Upload caller------------------------------------------------------------------------------
    //upload file libarary docs: https://github.com/quangdungtr/UnityStandaloneFileBrowser
    public void upload()
    {
        uploadSuccessfull = true;
        int numberBlock = 0, numberCondition = 0;
        //set path to empty string
        path = "";
        string CSVpath = "";
        mondPath = "";
        string[] holderPath;

        //this actually opens the filepanel to allow the user to choose a file. the last variable is the file type - limiting to csv for now
        try
        {
            holderPath = SFB.StandaloneFileBrowser.OpenFilePanel("Open File", "", "csv", true);
            if (experiment.hasUploaded)                     //if the user has already uploaded an experiment we want to clear that out so we can
                experiment.clearExperiment();               //enter the new data as it is most likely that ther are reuploading becuase of an error
            experiment.addCondition(new Condition(true));                       //no matter what we will still need to add the first condition and block
            experiment.conditions[numberCondition].addBlocks(new Block(false));
            if (experiment.Mondrians.Count() == 0)
                experiment.Mondrians.Add("0", new Mondrian("0", 1, true, 5, 15, 5, 15, 5000));       //here we add the default mondrian if the user don't want to make their own 
            experiment.mondsHaveBeenDrawn = false;
            //convert string[] into normal string
            for (int i = 0; i < holderPath.Length; i++)
                CSVpath += holderPath[i];
            uploadExperiment(CSVpath, numberCondition, numberBlock);
            if (uploadSuccessfull)
            {
                uploadErrorText.GetComponent<Text>().text = "Upload Successfull";
                uploadErrorText.GetComponent<Text>().color = Color.white;
            }
        }
        catch (IOException)
        {
            uploadErrorText.GetComponent<Text>().color = Color.red;
            uploadErrorText.GetComponent<Text>().text = "There was an error with the upload (make sure that the csv is closed in all editors, the file cannot be read from 2 places at once";
            experiment.clearExperiment();
            uploadSuccessfull = false;
        }
        catch (IndexOutOfRangeException)
        {
            uploadErrorText.GetComponent<Text>().color = Color.red;
            uploadErrorText.GetComponent<Text>().text = "Something was wrong with the formating of your upload please make sure that conditions, blocks, and trials are in sequential order";
            experiment.clearExperiment();
            uploadSuccessfull = false;
        }
        catch (NullReferenceException)
        {
            uploadErrorText.GetComponent<Text>().color = Color.red;
            uploadErrorText.GetComponent<Text>().text = "Something was wrong with the formating of your upload please make sure that conditions, blocks, and trials are in sequential order";
            experiment.clearExperiment();
            uploadSuccessfull = false;
        }
    }

    //---------------------------------------------------Experiment Uploader-------------------------------------------------------------------------------
    private void uploadExperiment(string holderPath, int numberCondition, int numberBlock)
    {
        string up = "up", down = "down", left = "left", right = "right";
        string mond = "0";
        string mask = "";
        int block0 = 0;             //blocks
        int v = 0;                  //the version or type of trial 0/instruction 1/Trial 2/Break 3/Response
        int trialNumber = 0;        //trial number
        int trialRand = 0;     //trial randomization
        bool response = false;              //if the normal trials take in response
        int duration = 0;           //duration of img showing
        int flash = 0;              //flash duration for mondrians
        string img = "empty";       //the image name
        int opacity = 0;            //max opacity the image reaches
        int maskDelay = 0;              //delay till next img. nothing shows in this time. in the beginning
        int staticDelay = 0;              //the time in ms to get to max opacity. 
        int flashing = 0;          //the time the flashing image is off the screen
        string img2 = "";
        bool responseStop = true;
        int quadrant = 0;
        int TTRO = -1;
        experiment.paletteUploadNeeded = false; 

        path = holderPath;
        Debug.Log(path);
        experiment.inputPath = path;
        int counter = 0;
        if (path.Length != 0)
        {
            //if a valid file is chosen, we use a streamreader to actually get the data.
            //streamreader docs: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-read-text-from-a-file
            using (var sr = new StreamReader(path))
            {
                bool EOF = false;           //EOF/End Of File
                while (!EOF)                        //while we havent reached the end of the line, keep going
                {
                    string data = sr.ReadLine();    //read a single line at a time 
                    if (data == null)               //if null then we reached the end of the file
                    {
                        EOF = true;
                        break;
                    }
                    var values = data.Split(',');           //split by commas, store in array of strings
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (counter > 0)            //this removes the first line of the csv, which will be the title of the column
                        {
                            int result = 0;         //this is used for string to int conversion in the try block below
                            //for 7,13,14,15,16,17 we do not need to convert as they are strings
                            if (i != 7 || i != 13 || i != 14 || i != 15 || i != 16 || i != 17)
                            {
                                if (!String.IsNullOrEmpty(values[i]))       //if the values are not null 
                                    try
                                    {
                                        //convert string to int value to add to list
                                        result = Int32.Parse(values[i]);
                                    }
                                    catch (FormatException)
                                    {
                                        Debug.Log("Error parsing value." + i + " " + counter + " ." + values[i] + ".");
                                    }
                            }
                            if (!String.IsNullOrEmpty(values[i]))
                            {
                                switch (i)                          //switch statement depanding on the column of the csv
                                {
                                    case 0:                         //condition
                                        if (counter != 1)           //we will build the experiment as the csv is being uploaded so we do not want to enter dummy values
                                        {                           //we will wait until the first column, third row (first row of csv is ignored so second row of what is read) 
                                            switch (v)              //and we will enter the values of the privous row before we read the next values in. 
                                            {
                                                case 0:
                                                    //Instruction(bool rand, int blk, Version type, string img, string imgpth, int dur)
                                                    experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new Instruction(trialRand, block0, img, duration));
                                                    break;
                                                case 1:
                                                    //Break(bool rand, int blk, Version type, string img, int dur)
                                                    experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new Break(trialRand, block0, img, duration));
                                                    break;
                                                case 2:
                                                    //Response(bool rand, int blk, Version type, string img, string u, string d, string l, string r)
                                                    if (!response)
                                                    {
                                                        up = "up"; down = "down"; left = "left"; right = "right";
                                                    }
                                                    experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new Response(trialRand, block0, img, up, down, left, right));
                                                    //if values are not uploaded for the responses these will be the default values entered
                                                    break;
                                                case 3:     //mondTrials
                                                    if (flashing == 0)
                                                    {
                                                        //MondTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, int mond, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                                                        experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new MondTrial(trialRand, block0, img, false, duration, flash, opacity, maskDelay, staticDelay, mond, "", up, down, left, right, response, responseStop, TTRO, quadrant));
                                                    }
                                                    else
                                                    {
                                                        //FlashMondTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, int mond, float flash, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                                                        experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new FlashMondTrial(trialRand, block0, img, false, duration, flash, opacity, maskDelay, staticDelay, mond, flashing, "", up, down, left, right, response, responseStop, TTRO, quadrant));
                                                    }
                                                    break;
                                                case 4:
                                                    //MaskTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, string mask, float flash, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                                                    if (flashing == 0)
                                                        flashing = (int)Math.Round(flash * 0.1f);
                                                    experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new MaskTrial(trialRand, block0, img, false, duration, flash, opacity, maskDelay, staticDelay, mask, flashing, "", up, down, left, right, response, responseStop, TTRO, quadrant));
                                                    break;
                                                case 5:     //multi mond trials
                                                    if (flashing == 0)
                                                    {
                                                        //MondTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, int mond, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                                                        experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new MondTrial(trialRand, block0, img, true, duration, flash, opacity, maskDelay, staticDelay, mond, img2, up, down, left, right, response, responseStop, TTRO, quadrant));
                                                    }
                                                    else
                                                    {
                                                        //FlashMondTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, int mond, float flash, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                                                        experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new FlashMondTrial(trialRand, block0, img, true, duration, flash, opacity, maskDelay, staticDelay, mond, flashing, img2, up, down, left, right, response, responseStop, TTRO, quadrant));
                                                    }
                                                    break;
                                                case 6:
                                                    //MaskTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, string mask, float flash, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                                                    if (flashing == 0)
                                                        flashing = (int)Math.Round(flash * 0.1f);
                                                    experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new MaskTrial(trialRand, block0, img, true, duration, flash, opacity, maskDelay, staticDelay, mask, flashing, img2, up, down, left, right, response, responseStop, TTRO, quadrant));
                                                    break;
                                            }
                                            experiment.count++;
                                            up = down = left = right = "";
                                            response = false;
                                            responseStop = true;
                                            flashing = 0;
                                            mond = "0";
                                            quadrant = 0;
                                            TTRO = -1;
                                        }
                                        if (result != numberCondition + 1)      //if the next trial is not the same condition as the privious trial
                                        {                                       //we will add a new condition (we add one because everything is uploaded counting from 1 and not 0
                                            experiment.addCondition(new Condition(false));              //add new condition (bool will be changed once uploaded
                                            numberCondition++;                                          //increase the numbercondition
                                            experiment.conditions[numberCondition].addBlocks(new Block(false));     //make a new block 
                                            numberBlock = 0;                                                        //set the number block to 0 
                                        }
                                        break;
                                    case 1:                         //condition random
                                        experiment.conditions[numberCondition].random = Convert.ToBoolean(result);   //this will technically means that the last input is what really matters but ¯\_(ツ)_/¯
                                        break;
                                    case 2:                         //block
                                        if (result != numberBlock + 1)          //same logic as the conditions
                                        {
                                            experiment.conditions[numberCondition].addBlocks(new Block(false));
                                            numberBlock++;
                                        }
                                        block0 = numberBlock;
                                        break;
                                    case 3:                         //block.random
                                        experiment.conditions[numberCondition].blocks[numberBlock].random = Convert.ToBoolean(result);
                                        break;
                                    case 4:                         //trial type
                                        v = result;                             //the input goes into these holders and they come together to create a trial in the end
                                        break;
                                    case 5:                         //order of the trials
                                        trialNumber = result;
                                        break;
                                    case 6:                         //trial.random
                                        trialRand = result;
                                        break;
                                    case 7:                         //trial.image
                                        if (v == 5 || v == 6)
                                        {
                                            var images = values[i].Split('_');
                                            img = images[0];
                                            img2 = images[1];
                                        }
                                        else
                                            img = values[i];
                                        break;
                                    case 8:                         //trial.duration
                                        duration = result;
                                        break;
                                    case 9:                         //trial.flashDuration
                                        flash = result;
                                        break;
                                    case 10:                         //trial.opacity
                                        opacity = result;
                                        break;
                                    case 11:                         //trial.maskdelay
                                        maskDelay = result;
                                        break;
                                    case 12:                        //trial.staticdelay
                                        staticDelay = result;
                                        break;
                                    case 13:
                                        if ((v == 3 || v == 5))
                                        {
                                            if (String.IsNullOrEmpty(mondPath) && !String.IsNullOrEmpty(values[i]))     //code has been changed so that the name of the mond file is 
                                            {   //hard coded as mask.csv (used to be mond.csv) from the same directory as the upload csv
                                                mond = values[i];              //take the input 
                                                uploadMondrians();                          //upload the mondrians to the mond array in the experiment holder
                                                experiment.Mondrians[mond].isUsed = true;   //have to upload here because we need to know which mondrians we will use and 
                                            }                                               //it would be ineffient to have to go through the whole experiment later to set this flag
                                            else if (!String.IsNullOrEmpty(values[i]))      //also there is no need to mess with the index because mond 0 is set to be the default mondrian
                                            {                                               //as far as the user knows they will be counting from 1 up 
                                                mond = values[i];
                                                experiment.Mondrians[mond].isUsed = true;
                                            }
                                        }
                                        else if (v == 4 || v == 6)
                                            mask = values[i];
                                        break;
                                    case 14:
                                        if (values[i] != "")
                                            response = true;
                                        up = values[i];
                                        break;
                                    case 15:
                                        if (values[i] != "")
                                            response = true;
                                        down = values[i];
                                        break;
                                    case 16:
                                        if (values[i] != "")
                                            response = true;
                                        left = values[i];
                                        break;
                                    case 17:
                                        if (values[i] != "")
                                            response = true;
                                        right = values[i];
                                        break;
                                    case 18:
                                        flashing = result;
                                        break;
                                    //case 19:
                                    case 19:
                                        TTRO = result - maskDelay;
                                        break;
                                    case 20:
                                        quadrant = result;
                                        break;
                                    case 21:
                                        responseStop = !Convert.ToBoolean(result);
                                        break;
                                }
                            }
                        }
                    }

                    //to keep track of what line we are on. we are using the assumation that the first line will be titles,
                    //the nexts lines are data.
                    counter++;
                }
                sr.Close();
            }
            switch (v)              //and we will enter the values of the privous row before we read the next values in. 
            {
                case 0:
                    //Instruction(bool rand, int blk, Version type, string img, string imgpth, int dur)
                    experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new Instruction(trialRand, block0, img, duration));
                    break;
                case 1:
                    //Break(bool rand, int blk, Version type, string img, int dur)
                    experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new Break(trialRand, block0, img, duration));
                    break;
                case 2:
                    //Response(bool rand, int blk, Version type, string img, string u, string d, string l, string r)
                    if (!response)
                    {
                        up = "up"; down = "down"; left = "left"; right = "right";
                    }
                    experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new Response(trialRand, block0, img, up, down, left, right));
                    //if values are not uploaded for the responses these will be the default values entered
                    break;
                case 3:     //mondTrials
                    if (flashing == 0)
                    {
                        //MondTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, int mond, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                        experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new MondTrial(trialRand, block0, img, false, duration, flash, opacity, maskDelay, staticDelay, mond, "", up, down, left, right, response, responseStop, TTRO, quadrant));
                    }
                    else
                    {
                        //FlashMondTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, int mond, float flash, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                        experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new FlashMondTrial(trialRand, block0, img, false, duration, flash, opacity, maskDelay, staticDelay, mond, flashing, "", up, down, left, right, response, responseStop, TTRO, quadrant));
                    }
                    break;
                case 4:
                    //MaskTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, string mask, float flash, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                    if (flashing == 0)
                        flashing = (int)Math.Round(flash * 0.1f);
                    experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new MaskTrial(trialRand, block0, img, false, duration, flash, opacity, maskDelay, staticDelay, mask, flashing, "", up, down, left, right, response, responseStop, TTRO, quadrant));
                    break;
                case 5:     //multi mond trials
                    if (flashing == 0)
                    {
                        //MondTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, int mond, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                        experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new MondTrial(trialRand, block0, img, true, duration, flash, opacity, maskDelay, staticDelay, mond, img2, up, down, left, right, response, responseStop, TTRO, quadrant));
                    }
                    else
                    {
                        //FlashMondTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, int mond, float flash, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                        experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new FlashMondTrial(trialRand, block0, img, true, duration, flash, opacity, maskDelay, staticDelay, mond, flashing, img2, up, down, left, right, response, responseStop, TTRO, quadrant));
                    }
                    break;
                case 6:
                    //MaskTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, string mask, float flash, string img2, string u, string d, string l, string r, bool resp, bool stop) : base(rand, blk, img, multi, img2)
                    if (flashing == 0)
                        flashing = (int)Math.Round(flash * 0.1f);
                    experiment.conditions[numberCondition].blocks[numberBlock].addTrial(new MaskTrial(trialRand, block0, img, true, duration, flash, opacity, maskDelay, staticDelay, mask, flashing, img2, up, down, left, right, response, responseStop, TTRO, quadrant));
                    break;
            }
            //experiment.count++;
            int numOfMonds = 0;

            foreach (KeyValuePair<string, Mondrian> item in experiment.Mondrians)        //Testing to see how many mondrians are used as the limit is 5
                if (item.Value.isUsed)
                    ++numOfMonds;

            if (numOfMonds > 5)                    //if more than 5 then reset them
            {
                uploadSuccessfull = false;
                uploadErrorText.GetComponent<Text>().color = Color.red;
                uploadErrorText.GetComponent<Text>().text = "Too many Noise Masks used.";
            }

            experiment.hasUploaded = true;          //set this to true so if there is an error and the file needs to be reuploaded the program will know to clear it beforehand
            experiment.randomizeConditions();       //randomize everything, with be randomized many more times but this is mostly just for checking 
            uploadImages();                    //run the image upload
            //experiment.printExperiment();           //print to the debug screen.
            Debug.Log("uploaded images");

            uploadMultiStims();
            //experiment.printExperiment();           //print to the debug screen.
            Debug.Log("uploaded multi stims");

            uploadMasks();                     //run the image upload for the mask trials. 
            experiment.printExperiment();           //print to the debug screen.
            Debug.Log("uploaded masks");

            if (uploadSuccessfull)
            {
                experiment.successfulUpload = true;
            }
        }
        else
        {
            uploadErrorText.GetComponent<Text>().color = Color.red;
            uploadErrorText.GetComponent<Text>().text = "Something was wrong with the path which was uploaded. ";
            uploadSuccessfull = false;
        }
    }

    public void simulateTrials()
    {
        int conditionIndex = 0, blockIndex = 0, trialIndex = 0, counter = 0;
        string fileName = "";
        if (experiment.outputPath != string.Empty)
            fileName = experiment.outputPath + "/" + experiment.ID + ".csv";
        else
            fileName = experiment.csvOriginDirectory + "/" + experiment.ID + ".csv";

        using (StreamWriter writer = File.AppendText(fileName))
        {
            writer.Write("ERROR,Trial Count,Condition,Cond Rand,Block,Block Rand,Trial Type,Input Order,Trial Rand,Image,Duration,Flash Duration,Opacity,Mask Delay,Static Delay,Mask,Response Time,Up,Down,Left,Right,Answer,Flash period,Multi Response\n");
            for (int i = 0; i < experiment.numberOfConditions; ++i)
            {
                conditionIndex = experiment.index[i];
                for (int j = 0; j < experiment.conditions[conditionIndex].numberOfBlocks; ++j)
                {
                    blockIndex = experiment.conditions[conditionIndex].index[j];
                    for (int k = 0; k < experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials; ++k, ++counter)
                    {
                        trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[k];
                        experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].simCheck();
                        writer.Write(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].error + "," +
                            experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].printTrial(conditionIndex, experiment.conditions[conditionIndex].random, blockIndex, experiment.conditions[conditionIndex].blocks[blockIndex].random, trialIndex, counter) );
                    }
                }
            }
            writer.Close();
        }
    }



    //---------------------------------------------------Mondrian upload caller----------------------------------------------------------------
    public void uploadMondrians()
    {
        string imagePath = path, palettePath;                                                //its called image path because I copied my code from the image uploader
        while (!imagePath[imagePath.Length - 1].Equals('\\'))                   //we want to remove the original file name so that we can find the directory and navigate to the correct location
        {
            imagePath = imagePath.Remove(imagePath.Length - 1, 1);
        }
        Debug.Log(imagePath);                                               //the mond file should always be in the same directory as the original csv in a folder called mondrian
        mondPath = imagePath + "mask.csv";                                      //now we can find the mond csv
        palettePath = imagePath + "colorPalette.csv";
        if (File.Exists(mondPath))
            readMondrians();
        else
        {
            uploadErrorText.GetComponent<Text>().color = Color.red;
            uploadErrorText.GetComponent<Text>().text = "ERROR:: Noise Mask file could not be found. Path... " + mondPath;
            uploadSuccessfull = false;
        }

        if (!File.Exists(palettePath))
        {
            uploadErrorText.GetComponent<Text>().color = Color.red;
            uploadErrorText.GetComponent<Text>().text = "ERROR:: Palette file could not be found. Path... " + palettePath;
            uploadSuccessfull = false;
        }

        experiment.printMondrians();
    }

    //-------------------------------------------------------Mondrian reader----------------------------------------------------------------------
    private void readMondrians()     //exact logic from previous upload 
    {
        Debug.Log(mondPath);
        int counter = 0, shape = 0, minW = 0, maxW = 0, minH = 0, maxH = 0, Density = 0;
        string key = "", palette = "0";
        bool pix = false;
        if (mondPath.Length != 0)
        {
            using (var sr = new StreamReader(mondPath))
            {
                bool EOF = false;
                while (!EOF)
                {
                    string data = sr.ReadLine();
                    if (data == null)
                    {
                        EOF = true;
                        break;
                    }
                    var values = data.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (counter > 0)
                        {
                            switch (i)
                            {
                                case 0:                         //here we make our default mondrain and take in the palette
                                    if (counter > 1)
                                        experiment.Mondrians.Add(key, new Mondrian(palette, shape, pix, minW, maxW, minH, maxH, Density));
                                    key = values[i];
                                    break;
                                case 1:                         //here we take in the shape
                                    if (values[i] != "0")
                                        experiment.paletteUploadNeeded = true; 
                                    palette = values[i];
                                    break;
                                case 2:
                                    shape = Int32.Parse(values[i]);
                                    break;
                                case 3:                         //minimum width
                                    if (Int32.Parse(values[i]) == 0)
                                        pix = false;
                                    else
                                        pix = true;
                                    break;
                                case 4:
                                    minW = Int32.Parse(values[i]);
                                    break;
                                case 5:                         //maximum width
                                    maxW = Int32.Parse(values[i]);
                                    break;
                                case 6:                         //minimum height
                                    minH = Int32.Parse(values[i]);
                                    break;
                                case 7:                         //maximum height
                                    maxH = Int32.Parse(values[i]);
                                    break;
                                case 8:                         //density
                                    Density = Int32.Parse(values[i]);
                                    break;
                            }
                        }
                    }
                    counter++;
                }
                sr.Close();
            }
            experiment.Mondrians.Add(key, new Mondrian(palette, shape, pix, minW, maxW, minH, maxH, Density));
        }
    }

    //-----------------------------------------------------------Image uploader (sorry)--------------------------------------------------------------------------------------------------------
    //This code is really complex, the idea was so that the user could upload a file with a symbol which corresponds to the type of randomization that the images in that file would
    //receive. Also the user could also upload just an image name making the logic much more confusing and that much deeper. If you are reading this good luck. This code is also optimized 
    //to take in the mask images, so fuck me I guess. 
    private void uploadImages()
    {
        bool successfulRead = false;                        //essentially we will remove from the trialsToBeRandomized array and repopulate instead of the imageArr
        List<int> trialsToBeRandomized = new List<int>();   //this will save the trial index of the trials that will need images grabbed from the text file
        List<string> notFoundImages = new List<string>();   //for testing that all the files exist all the ones that don't will be added here and displayed
        List<string> imageArr = new List<string>();         //this is the array that we are writing into from the text file
        string imagePath = path;                            //holds the path of each blocks txt path
        Debug.Log(path);
        string holderImagePath;                             //holds the directory of the original csv
        string test;                                        //string we write into when we read the txt 
        int conditionIndex = 0, blockIndex = 0, trialIndex = 0;
        while (!imagePath[imagePath.Length - 1].Equals('\\'))       //remove the original csv file name so we can get the directory
        {
            imagePath = imagePath.Remove(imagePath.Length - 1, 1);
        }
        experiment.csvOriginDirectory = imagePath;
        Debug.Log(imagePath);
        imagePath += "Stimuli\\";                           //all images will be in the same directory in a file named stimuli
        holderImagePath = imagePath;                                                                //save the image path somewhere else as imagepath will be manipulated throughout the algorthem 
        for (int i = 0; i < experiment.numberOfConditions; i++)                                     //loop through conditions
        {
            conditionIndex = experiment.index[i];                                                           //have the different indexes of all the cond/blo/tri so it does not need to be referenced everytime 
            for (int j = 0; j < experiment.conditions[conditionIndex].numberOfBlocks; j++)                               //loop blocks
            {
                blockIndex = experiment.conditions[conditionIndex].index[j];
                for (int k = 0; !successfulRead && k < experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials && uploadSuccessfull; k++)      //loop trials
                {
                    trialsToBeRandomized.Clear();           //clear these arrays from the privous trials
                    imageArr.Clear();
                    trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[k];                     //getting the trial index
                    imagePath = experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image;      //finding the imagepath for each trial

                    if (imagePath[0] == '$' || imagePath[0] == '#' || imagePath[0] == '&')          //testing to see if the image path is a txt or img
                    {                                                                               //ie. if it doesn't have a symbol it isn't a txt
                        experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype = imagePath[0]; //grab the symbol for later
                        imagePath = holderImagePath + imagePath.Remove(0, 1);                       //create path to the file and remove the symbol from the file name 
                        if (imagePath.Length != 0)
                        {
                            using (var sr = new StreamReader(imagePath))            //here we read in the file to an array, a little more simple than the csv just line by line
                            {
                                while (!sr.EndOfStream)
                                {
                                    test = sr.ReadLine();
                                    imageArr.Add(test);                             //add the image names to the image arr
                                    Debug.Log(test);
                                }
                            }
                            successfulRead = true;
                        }

                        if (successfulRead)             //if the upload was successful
                        {
                            for (int h = k; h < experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials; h++)
                            {   //here we are finding the trials that are pulling from the list and saving them in an array for when we make the image file path
                                trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[h];
                                if (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image[0] == experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype)
                                {
                                    trialsToBeRandomized.Add(trialIndex);
                                }
                                else   //If the image is not to be randomized then we just use the image that it was uploaded with
                                {
                                    experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath = holderImagePath + experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image;
                                }
                            }

                            if (experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype == '#')     //going through list in order that it is presented
                            {
                                for (int h = 0, y = 0; h < trialsToBeRandomized.Count; h++, y++)
                                {
                                    if (y >= imageArr.Count)            //if the number of trials is more than the number of images just loop back through the beginning of the images
                                        y = 0;                          //TLDR: if we reach the end of the file, reset
                                    experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].imagePath = holderImagePath + imageArr[y];
                                    experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].image = imageArr[y];
                                }
                            }

                            else if (experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype == '&')    //randomly pick out pictures from the list
                            {
                                for (int h = 0; h < trialsToBeRandomized.Count; h++)
                                {
                                    int x = Random.Range(0, imageArr.Count);        //finds a random image in the list
                                    experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].imagePath = holderImagePath + imageArr[x];
                                    experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].image = imageArr[x];
                                }
                            }

                            else if (experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype == '$')    //randomly pick out pictures from the list equally
                            {
                                var imageRange = Enumerable.Range(0, imageArr.Count).ToList();              //here we are making an index for the image arr that we can pull from and 
                                for (int h = 0; h < trialsToBeRandomized.Count; h++)                        //repopulate if the array is empty because we will remove if it is chosen
                                {
                                    int x = Random.Range(0, imageRange.Count);      //here we find a random in the range of imageRange as to not cause an null pointer exception 
                                    //the index that is chosen is not the index used for the random trial but rather the image that is used for the image range which will choose an image in the file
                                    experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].imagePath = holderImagePath + imageArr[imageRange[x]];
                                    experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].image = imageArr[imageRange[x]];
                                    imageRange.RemoveRange(x, 1);       //remove that image index so it cannot be chosen again
                                    if (imageRange.Count == 0)          //if we have gone through the whole list then repopulate the list with more indexes
                                        imageRange = Enumerable.Range(0, imageArr.Count).ToList();
                                }
                            }
                        }
                        else        //if the read was not successful we need to flag the upload successfull flag 
                        {
                            uploadErrorText.GetComponent<Text>().color = Color.red;
                            uploadErrorText.GetComponent<Text>().text = "ERROR:: Upload file could not be found/read properly. Path... " + imagePath;
                            uploadSuccessfull = false;
                        }
                        for (int h = 0; h < experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials; h++)
                        {           //here we test to see if each images exists
                            trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[h];
                            if (!File.Exists(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath))
                            {           //if not we flag the upload successful and add it to a list of images which we will display to the user
                                notFoundImages.Add(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image);
                                uploadSuccessfull = false;
                            }
                        }
                        if (!uploadSuccessfull)
                        {           //if the upload was unsuccessfull we will show the user which images were not found
                            uploadErrorText.GetComponent<Text>().color = Color.red;
                            uploadErrorText.GetComponent<Text>().text = "ERROR:: Some images could not be found... ";
                            foreach (string thing in notFoundImages)
                            {
                                uploadErrorText.GetComponent<Text>().text += thing + ", ";
                            }
                        }
                    }
                    else
                    {       //if a symbol was not given we just add the image. 
                        experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath = holderImagePath + experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image;
                        if (!File.Exists(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath))
                        {          //check to make sure that the image exists
                            uploadSuccessfull = false;
                            uploadErrorText.GetComponent<Text>().color = Color.red;
                            uploadErrorText.GetComponent<Text>().text = "ERROR:: Some images could not be found... " + experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image + ".";
                        }
                    }
                }
                successfulRead = false;
            }
        }
    }

    //-----------------------------------------------------------Mask uploader (also sorry)--------------------------------------------------------------------------------------------------------
    //basically the same logic as the image uploader except with a lot more checking to make sure the trial is a mask trial
    //to take in the mask images, so fuck me I guess. 
    private void uploadMasks()
    {
        bool successfulRead = false;                        //essentially we will remove from the trialsToBeRandomized array and repopulate instead of the imageArr
        List<int> trialsToBeRandomized = new List<int>();   //this will save the trial index of the trials that will need images grabbed from the text file
        List<string> notFoundImages = new List<string>();   //for testing that all the files exist all the ones that don't will be added here and displayed
        List<string> imageArr = new List<string>();         //this is the array that we are writing into from the text file
        string imagePath = path;                            //holds the path of each blocks txt path
        string holderImagePath;                             //holds the directory of the original csv
        string test;                                        //string we write into when we read the txt 
        int conditionIndex = 0, blockIndex = 0, trialIndex = 0;
        while (!imagePath[imagePath.Length - 1].Equals('\\'))       //remove the original csv file name so we can get the directory
        {
            imagePath = imagePath.Remove(imagePath.Length - 1, 1);
        }
        Debug.Log(imagePath);
        imagePath += "Stimuli\\";                                   //all images will be in the same directory in a file named stimuli
        holderImagePath = imagePath;                                //save the image path somewhere else as imagepath will be manipulated throughout the algorthem 
        for (int i = 0; i < experiment.numberOfConditions; i++)     //loop through conditions
        {
            conditionIndex = experiment.index[i];                                                           //have the different indexes of all the cond/blo/tri so it does not need to be referenced everytime 
            for (int j = 0; j < experiment.conditions[conditionIndex].numberOfBlocks; j++)                               //loop blocks
            {
                blockIndex = experiment.conditions[conditionIndex].index[j];
                for (int k = 0; !successfulRead && k < experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials && uploadSuccessfull; k++)      //loop trials
                {
                    trialsToBeRandomized.Clear();                                                                       //clear these arrays from the privous trials
                    imageArr.Clear();
                    Debug.Log(k + " " + experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials);
                    trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[k];                     //getting the trial index

                    if (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is MaskTrial)
                    {
                        imagePath = (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).mask;      //finding the imagepath for each trial

                        if (imagePath[0] == '$' || imagePath[0] == '#' || imagePath[0] == '&')          //testing to see if the image path is a txt or img
                        {                                                                               //ie. if it doesn't have a symbol it isn't a txt
                            experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype = imagePath[0]; //grab the symbol for later
                            imagePath = holderImagePath + imagePath.Remove(0, 1);                       //create path to the file and remove the symbol from the file name 
                            if (imagePath.Length != 0)
                            {
                                using (var sr = new StreamReader(imagePath))            //here we read in the file to an array, a little more simple than the csv just line by line
                                {
                                    while (!sr.EndOfStream)
                                    {
                                        test = sr.ReadLine();
                                        imageArr.Add(test);                             //add the image names to the image arr
                                        Debug.Log(test);
                                    }
                                }
                                successfulRead = true;
                            }

                            if (successfulRead)             //if the upload was successful
                            {
                                for (int h = k; h < experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials; h++)
                                {   //here we are finding the trials that are pulling from the list and saving them in an array for when we make the image file path
                                    trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[h];
                                    if (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is MaskTrial)
                                    {
                                        if ((experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).mask[0] == experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype)
                                        {
                                            trialsToBeRandomized.Add(trialIndex);
                                        }
                                        else   //If the image is not to be randomized then we just use the image that it was uploaded with
                                        {
                                            (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath = holderImagePath + (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).mask;
                                            Debug.Log((experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath);
                                        }
                                        Debug.Log(experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype + " " + (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).mask[0]);
                                        Debug.Log((experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).mask);
                                    }
                                }

                                if (experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype == '#')     //going through list in order that it is presented
                                {
                                    for (int h = 0, y = 0; h < trialsToBeRandomized.Count; h++, y++)
                                    {
                                        if (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]] is MaskTrial)
                                        {
                                            if (y >= imageArr.Count)            //if the number of trials is more than the number of images just loop back through the beginning of the images
                                                y = 0;                          //TLDR: if we reach the end of the file, reset
                                            (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]] as MaskTrial).maskPath = holderImagePath + imageArr[y];
                                            (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]] as MaskTrial).mask = imageArr[y];
                                        }
                                    }
                                }
                                else if (experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype == '&')    //randomly pick out pictures from the list
                                {
                                    for (int h = 0; h < trialsToBeRandomized.Count; h++)
                                    {
                                        int x = Random.Range(0, imageArr.Count);        //finds a random image in the list
                                        (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]] as MaskTrial).maskPath = holderImagePath + imageArr[x];
                                        (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]] as MaskTrial).mask = imageArr[x];
                                    }
                                }
                                else if (experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype == '$')    //randomly pick out pictures from the list equally
                                {

                                    var imageRange = Enumerable.Range(0, imageArr.Count).ToList();              //here we are making an index for the image arr that we can pull from and 
                                    for (int h = 0; h < trialsToBeRandomized.Count; h++)                        //repopulate if the array is empty because we will remove if it is chosen
                                    {
                                        int x = Random.Range(0, imageRange.Count);      //here we find a random in the range of imageRange as to not cause an null pointer exception 
                                                                                        //the index that is chosen is not the index used for the random trial but rather the image that is used for the image range which will choose an image in the file
                                        (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]] as MaskTrial).maskPath = holderImagePath + imageArr[imageRange[x]];
                                        (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]] as MaskTrial).mask = imageArr[imageRange[x]];
                                        imageRange.RemoveRange(x, 1);       //remove that image index so it cannot be chosen again
                                        if (imageRange.Count == 0)          //if we have gone through the whole list then repopulate the list with more indexes
                                            imageRange = Enumerable.Range(0, imageArr.Count).ToList();
                                    }

                                }
                            }
                            else        //if the read was not successful we need to flag the upload successfull flag 
                            {
                                uploadErrorText.GetComponent<Text>().color = Color.red;
                                uploadErrorText.GetComponent<Text>().text = "ERROR:: Upload file could not be found/read properly. Path... " + imagePath;
                                uploadSuccessfull = false;
                            }
                            for (int h = 0; h < experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials; h++)
                            {           //here we test to see if each images exists
                                trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[h];
                                if (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] is MaskTrial)
                                {
                                    Debug.Log((experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath);
                                    if (!File.Exists((experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath))
                                    {           //if not we flag the upload successful and add it to a list of images which we will display to the user
                                        notFoundImages.Add((experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).mask);
                                        uploadSuccessfull = false;
                                        Debug.Log("Failed: " + (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath);
                                    }
                                }
                            }
                            if (!uploadSuccessfull)
                            {           //if the upload was unsuccessfull we will show the user which images were not found
                                uploadErrorText.GetComponent<Text>().color = Color.red;
                                uploadErrorText.GetComponent<Text>().text = "ERROR:: Some images could not be found... ";
                                foreach (string thing in notFoundImages)
                                {
                                    uploadErrorText.GetComponent<Text>().text += thing + ", ";
                                }
                            }
                        }
                        else
                        {       //if a symbol was not given we just add the image. 
                            (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath = holderImagePath + (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).mask;
                            if (!File.Exists((experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).maskPath))
                            {          //check to make sure that the image exists
                                uploadSuccessfull = false;
                                uploadErrorText.GetComponent<Text>().color = Color.red;
                                uploadErrorText.GetComponent<Text>().text = "ERROR:: Some images could not be found... " + (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex] as MaskTrial).mask + ".";
                            }
                        }
                    }
                }
                successfulRead = false;
            }
        }
    }


    //-------------------------------------------------------------Multi Stim Mask Trials-------------------------------------------------------------------
    private void uploadMultiStims()
    {
        bool successfulRead = false;                        //essentially we will remove from the trialsToBeRandomized array and repopulate instead of the imageArr
        List<int> trialsToBeRandomized = new List<int>();   //this will save the trial index of the trials that will need images grabbed from the text file
        List<string> notFoundImages = new List<string>();   //for testing that all the files exist all the ones that don't will be added here and displayed
        List<string> imageArr = new List<string>();         //this is the array that we are writing into from the text file
        string imagePath = path;                            //holds the path of each blocks txt path
        string holderImagePath;                             //holds the directory of the original csv
        string test;                                        //string we write into when we read the txt 
        int conditionIndex = 0, blockIndex = 0, trialIndex = 0;
        while (!imagePath[imagePath.Length - 1].Equals('\\'))       //remove the original csv file name so we can get the directory
        {
            imagePath = imagePath.Remove(imagePath.Length - 1, 1);
        }
        Debug.Log(imagePath);
        imagePath += "Stimuli\\";                                   //all images will be in the same directory in a file named stimuli
        holderImagePath = imagePath;                                //save the image path somewhere else as imagepath will be manipulated throughout the algorthem 
        for (int i = 0; i < experiment.numberOfConditions; i++)     //loop through conditions
        {
            conditionIndex = experiment.index[i];                                                           //have the different indexes of all the cond/blo/tri so it does not need to be referenced everytime 
            for (int j = 0; j < experiment.conditions[conditionIndex].numberOfBlocks; j++)                               //loop blocks
            {
                blockIndex = experiment.conditions[conditionIndex].index[j];
                for (int k = 0; !successfulRead && k < experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials && uploadSuccessfull; k++)      //loop trials
                {
                    trialsToBeRandomized.Clear();                                                                       //clear these arrays from the privous trials
                    imageArr.Clear();
                    Debug.Log(k + " " + experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials);
                    trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[k];                     //getting the trial index

                    if (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].hasMultipleStims)
                    {
                        imagePath = experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2;      //finding the imagepath for each trial

                        if (imagePath[0] == '$' || imagePath[0] == '#' || imagePath[0] == '&')          //testing to see if the image path is a txt or img
                        {                                                                               //ie. if it doesn't have a symbol it isn't a txt
                            experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype = imagePath[0]; //grab the symbol for later
                            imagePath = holderImagePath + imagePath.Remove(0, 1);                       //create path to the file and remove the symbol from the file name 
                            if (imagePath.Length != 0)
                            {
                                using (var sr = new StreamReader(imagePath))            //here we read in the file to an array, a little more simple than the csv just line by line
                                {
                                    while (!sr.EndOfStream)
                                    {
                                        test = sr.ReadLine();
                                        imageArr.Add(test);                             //add the image names to the image arr
                                        Debug.Log(test);
                                    }
                                }
                                successfulRead = true;
                            }

                            if (successfulRead)             //if the upload was successful
                            {
                                for (int h = k; h < experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials; h++)
                                {   //here we are finding the trials that are pulling from the list and saving them in an array for when we make the image file path
                                    trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[h];
                                    if (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].hasMultipleStims)
                                    {
                                        if (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2[0] == experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype)
                                        {
                                            trialsToBeRandomized.Add(trialIndex);
                                        }
                                        else   //If the image is not to be randomized then we just use the image that it was uploaded with
                                        {
                                            experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2Path = holderImagePath + experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2;
                                            Debug.Log(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2Path);
                                        }
                                        Debug.Log(experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype + " " + experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2[0]);
                                        Debug.Log(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2);
                                    }
                                }

                                if (experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype == '#')     //going through list in order that it is presented
                                {
                                    for (int h = 0, y = 0; h < trialsToBeRandomized.Count; h++, y++)
                                    {
                                        if (y >= imageArr.Count)            //if the number of trials is more than the number of images just loop back through the beginning of the images
                                            y = 0;                          //TLDR: if we reach the end of the file, reset
                                        experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].image2Path = holderImagePath + imageArr[y];
                                        experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].image2 = imageArr[y];
                                    }
                                }
                                else if (experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype == '&')    //randomly pick out pictures from the list
                                {
                                    for (int h = 0; h < trialsToBeRandomized.Count; h++)
                                    {
                                        int x = Random.Range(0, imageArr.Count);        //finds a random image in the list
                                        experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].image2Path = holderImagePath + imageArr[x];
                                        experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].image2 = imageArr[x];
                                    }
                                }
                                else if (experiment.conditions[conditionIndex].blocks[blockIndex].imageRandomizationtype == '$')    //randomly pick out pictures from the list equally
                                {
                                    var imageRange = Enumerable.Range(0, imageArr.Count).ToList();              //here we are making an index for the image arr that we can pull from and 
                                    for (int h = 0; h < trialsToBeRandomized.Count; h++)                        //repopulate if the array is empty because we will remove if it is chosen
                                    {
                                        int x = Random.Range(0, imageRange.Count);      //here we find a random in the range of imageRange as to not cause an null pointer exception 
                                                                                        //the index that is chosen is not the index used for the random trial but rather the image that is used for the image range which will choose an image in the file
                                        experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].image2Path = holderImagePath + imageArr[imageRange[x]];
                                        experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialsToBeRandomized[h]].image2 = imageArr[imageRange[x]];
                                        imageRange.RemoveRange(x, 1);       //remove that image index so it cannot be chosen again
                                        if (imageRange.Count == 0)          //if we have gone through the whole list then repopulate the list with more indexes
                                            imageRange = Enumerable.Range(0, imageArr.Count).ToList();
                                    }
                                }
                            }
                            else        //if the read was not successful we need to flag the upload successfull flag 
                            {
                                uploadErrorText.GetComponent<Text>().color = Color.red;
                                uploadErrorText.GetComponent<Text>().text = "ERROR:: Upload file could not be found/read properly. Path... " + imagePath;
                                uploadSuccessfull = false;
                            }
                            for (int h = 0; h < experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials; h++)
                            {           //here we test to see if each images exists
                                trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[h];
                                if (experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].hasMultipleStims)
                                {
                                    Debug.Log(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2Path);
                                    if (!File.Exists(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2Path))
                                    {           //if not we flag the upload successful and add it to a list of images which we will display to the user
                                        notFoundImages.Add(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2);
                                        uploadSuccessfull = false;
                                        Debug.Log("Failed: " + experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2Path);
                                    }
                                }
                            }
                            if (!uploadSuccessfull)
                            {           //if the upload was unsuccessfull we will show the user which images were not found
                                uploadErrorText.GetComponent<Text>().color = Color.red;
                                uploadErrorText.GetComponent<Text>().text = "ERROR:: Some images could not be found... ";
                                foreach (string thing in notFoundImages)
                                {
                                    uploadErrorText.GetComponent<Text>().text += thing + ", ";
                                }
                            }
                        }
                        else
                        {       //if a symbol was not given we just add the image. 
                            experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2Path = holderImagePath + experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2;
                            if (!File.Exists(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2Path))
                            {          //check to make sure that the image exists
                                uploadSuccessfull = false;
                                uploadErrorText.GetComponent<Text>().color = Color.red;
                                uploadErrorText.GetComponent<Text>().text = "ERROR:: Some images could not be found... " + experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].image2 + ".";
                            }
                        }
                    }
                }
                successfulRead = false;
            }
        }
    }

    public void inputConditionOrder()
    {
        changeConditionOrder(conditionOrderInputfield.GetComponent<Text>().text);
    }

    //-------------------------------------------------------Changing order of the conditions---------------------------------------------------------------
    private void changeConditionOrder(string conditionOrder)                   //this function is for changing the order of conditions
    {
        if (!uploadSuccessfull)
        {
            conditionOrderTextDisplay.GetComponent<Text>().color = Color.red;
            conditionOrderTextDisplay.GetComponent<Text>().text = "No file was uploaded or there was an error with the upload";
        }
        else
        {
            List<int> numbers = new List<int>();
            int integer = 0, x = 0, y = 0;
            bool highNumberInput = false;
            StringBuilder sb = new StringBuilder();
            Color errorColor = new Color(144, 0, 0, 255), normalColor = new Color(255, 255, 255, 255);
            bool parsed = int.TryParse(conditionOrder, out integer);    //we use the int.TryParse to see if the input was integers
            if (parsed)             //if the input was an integer 
            {
                y = integer;
                do                                  //we add all the integers into an array called numbers
                {
                    x = y % 10;
                    y /= 10;
                    numbers.Add(x);
                } while (y > 0);

                for (int i = 0; i < numbers.Count / 2; i++)         //we then have to reverse the array because the numbers were taken in from right to left
                {
                    x = numbers[i];
                    numbers[i] = numbers[numbers.Count - i - 1];
                    numbers[numbers.Count - i - 1] = x;
                }

                sb.Append("Condition order: ");                     //we add the "Condition Order to the stringbuilder

                for (int i = 0; i < numbers.Count; i++)             //we then build out the string for the output in the GUI
                {
                    if (i == 0)
                    {
                        sb.Append(numbers[i] + ", ");
                    }
                    else if (i == numbers.Count - 1)
                    {
                        sb.Append(numbers[i] + ".");
                    }
                    else
                    {
                        sb.Append(numbers[i] + ", ");
                    }
                }

                for (int i = 0; i < experiment.numberOfConditions; i++)     //we then test to see if any of the numbers are bigger the number of conditions
                {
                    try
                    {
                        if (numbers[i] > experiment.numberOfConditions)
                        {
                            highNumberInput = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {

                    }
                }

                if (numbers.Count <= experiment.numberOfConditions && !highNumberInput)     //if the number of inputs is the same size or smaller than the number of conditions
                {                                                                           //and if the input was not larger than the number of conditions
                    experiment.setConditionOrder(integer);                                  //we set the order and change the color of the GUI output to white
                    experiment.printExperiment();
                    conditionOrderTextDisplay.GetComponent<Text>().color = normalColor;
                }
                else if (highNumberInput)                                                   //if the input was a higher number we put an error message
                {
                    sb.Clear();                                                             //first clear the stringbuilder
                    sb.Append("ERROR: input was out of bounds");                            //add error message
                    conditionOrderTextDisplay.GetComponent<Text>().color = errorColor;      //change output color
                }
                else
                {                                                                           //if its not either of those then there were more integers than the number of conditions
                    sb.Clear();
                    sb.Append("ERROR: input contained more integers than the number of conditions");
                    conditionOrderTextDisplay.GetComponent<Text>().color = errorColor;
                }
            }
            else
            {
                sb.Append("ERROR: incorrect input please use integers.");                   //if int.TryParse comes back false we flag this error
                conditionOrderTextDisplay.GetComponent<Text>().color = errorColor;
            }
            conditionOrderTextDisplay.GetComponent<Text>().text = sb.ToString();            //then finally we print to the GUI
        }
    }

    //-------------------------------------------------------ID input-----------------------------------------------------
    public void inputID()
    {
        experiment.ID = idInputfield.GetComponent<Text>().text;
        idTextDisplay.GetComponent<Text>().color = Color.white;
        idTextDisplay.GetComponent<Text>().text = experiment.ID;
    }

    //---------------------------------------------------Path Input--------------------------------------------------------
    public void inputOutputPath()
    {
        string[] stringHolderPath;
        string holderPath = "";
        try
        {
            stringHolderPath = SFB.StandaloneFileBrowser.OpenFolderPanel("Open File", "", false);
            foreach (var i in stringHolderPath)
                holderPath += i;
            Debug.Log(holderPath);
            experiment.outputPath = holderPath;
        }
        catch (IOException)
        {
            outputPathErrorText.GetComponent<Text>().color = Color.red;
            outputPathErrorText.GetComponent<Text>().text = "There was an error with the upload (make sure that the csv is closed in all editors, the file cannot be read from 2 places at once";
        }
        catch (IndexOutOfRangeException)
        {
            outputPathErrorText.GetComponent<Text>().color = Color.red;
            outputPathErrorText.GetComponent<Text>().text = "Something was wrong with the formating of your path upload (IOOR)";
        }
        catch (NullReferenceException)
        {
            outputPathErrorText.GetComponent<Text>().color = Color.red;
            outputPathErrorText.GetComponent<Text>().text = "Something was wrong with the formating of your path upload (NRE)";
        }
    }

    //--------------------------------------------------Dominant eye setter------------------------------------------------
    public void setDominantEye()                        //here we have the dominant eye input
    {
        rightEye = GameObject.Find("rightEye").GetComponent<Toggle>();
        leftEye = GameObject.Find("leftEye").GetComponent<Toggle>();

        if (rightEye.isOn && leftEye.isOn)              //if both eyes are turned on we flag an error
        {
            experiment.left = false;
            experiment.right = false;
            dominantEyeError.GetComponent<Text>().color = Color.red;
            dominantEyeError.GetComponent<Text>().text = "ERROR: Both eyes are set to dominant";
        }
        else if (rightEye.isOn)                         //if only the right eye is on 
        {
            experiment.right = true;                    //we set the right eye to true
            experiment.left = false;                    //and the left eye to false for if the user corrected a double input
            dominantEyeError.GetComponent<Text>().color = Color.white;
            dominantEyeError.GetComponent<Text>().text = "Right eye dominance is " + experiment.right;
        }
        else if (leftEye.isOn)                          //same logic for if the right eye is on
        {
            experiment.left = true;
            experiment.right = false;
            dominantEyeError.GetComponent<Text>().color = Color.white;
            dominantEyeError.GetComponent<Text>().text = "Left eye dominance is " + experiment.left;
        }
        else                                            //if nothing is on we make the output nothing
        {
            experiment.left = false;
            experiment.right = false;
            dominantEyeError.GetComponent<Text>().text = "";
        }
    }

    //---------------------------------------------Start game button caller----------------------------------------------------------
    public void StartGame()
    {
        bool successfullInputByUser = true;

        if (!experiment.right && !experiment.left)
        {
            dominantEyeError.GetComponent<Text>().color = Color.red;
            dominantEyeError.GetComponent<Text>().text = "Please select a dominant eye before starting the trials";
            successfullInputByUser = false;
        }

        if (String.IsNullOrEmpty(experiment.ID))
        {
            idTextDisplay.GetComponent<Text>().fontSize = 10;
            idTextDisplay.GetComponent<Text>().color = Color.red;
            idTextDisplay.GetComponent<Text>().text = "Please enter the participant's ID";
            successfullInputByUser = false;
        }

        if (!uploadSuccessfull)
        {
            uploadErrorText.GetComponent<Text>().color = Color.red;
            uploadErrorText.GetComponent<Text>().text = "No useable file was uploaded";
            successfullInputByUser = false;
        }

        if (successfullInputByUser)
        {
            Experiment.current.sceneToBeLoaded = 4;
            SceneManager.LoadScene(3);
            //TODO: add check display that prints out which file and which dom eye
        }
        return;
    }

    public void backToOpening()
    {
        Experiment.current.sceneToBeLoaded = 0;
        SceneManager.LoadScene(0);
    }

    //values for testing the images not in final product but still here if anyone downloads the unity files
    public int CI = 0, BI = 0, TI = 0;
    //----------------------------------------------------image testing 
    public void testingImages()
    {
        int conditionIndex = experiment.index[CI];
        int blockIndex = experiment.conditions[conditionIndex].index[BI];
        int trialIndex = experiment.conditions[conditionIndex].blocks[blockIndex].index[TI];
        StartCoroutine(GetTexture(conditionIndex, blockIndex, trialIndex));
        TI++;
        if (TI == experiment.conditions[conditionIndex].blocks[blockIndex].numberOfTrials)
        {
            TI = 0;
            BI++;
            if (BI == experiment.conditions[conditionIndex].numberOfBlocks)
            {
                BI = 0;
                CI++;
                if (CI == experiment.numberOfConditions)
                {
                    CI = 0;
                }
            }
        }
        Debug.Log(CI + " " + BI + " " + TI);
    }

    //Testing image input from the file system
    IEnumerator<UnityWebRequestAsyncOperation> GetTexture(int conditionIndex, int blockIndex, int trialIndex)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath);
        yield return www.SendWebRequest();
        new WaitForSecondsRealtime(1);
        if (www.isHttpError || www.isNetworkError)
        {
            Debug.Log(www.error);
            Debug.Log(experiment.conditions[conditionIndex].blocks[blockIndex].trials[trialIndex].imagePath);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            rawImage.texture = myTexture;
        }
    }

    //for testing the mondrians
    /*
    public void testingMondrian()
    {   //Mondrian(int palette, int sh, int minWidth, int maxWidth, int minHeight, int maxHeight, int density)
        List<Mondrian> m = new List<Mondrian>();
        m.Add(new Mondrian(0, 0, true, 5, 25, 5, 25, 10000));     //ellipse
        m.Add(new Mondrian(0, 1, true, 1, 1, 1, 1, 10000));       //rectangle
        m.Add(new Mondrian(0, 2, true, 5, 15, 5, 15, 10000));     //triangle
        m.Add(new Mondrian(0, 3, true, 5, 15, 5, 15, 10000));     //pixelated
        m.Add(new Mondrian(0, 4, true, 5, 25, 5, 25, 10000));     //circle
        m.Add(new Mondrian(0, 5, true, 5, 10, 5, 10, 10000));     //square
        m.Add(new Mondrian(0, 6, true, 5, 15, 5, 15, 10000));     //mixed

        createMond.makeBlank(rawImage.texture as Texture2D);

        List<List<Color>> colors = new List<List<Color>>();
        List<Color> row = new List<Color>();
        row.Add(new Color(255, 0, 0));      //red
        row.Add(new Color(0, 255, 0));      //green
        row.Add(new Color(0, 0, 255));      //blue
        row.Add(new Color(255, 0, 255));    //magenta
        row.Add(new Color(255, 255, 0));    //yellow
        row.Add(new Color(0, 255, 255));    //cyan
        colors.Add(row);
        int rand = Random.Range(0, m.Count);
        rawImage.enabled = true;
        if (m[rand].shape == Shape.ellipse)
        {
            rawImage.texture = createMond.DrawEllipse(rawImage.texture as Texture2D, colors[m[rand].palette], m[rand]);
            Debug.Log("ellipse");
        }
        else if (m[rand].shape == Shape.rectangle)
        {
            rawImage.texture = createMond.DrawRectangle(rawImage.texture as Texture2D, colors[m[rand].palette], m[rand]);
            Debug.Log("rectangle");
        }
        else if (m[rand].shape == Shape.triangle)
        {
            rawImage.texture = createMond.DrawTriangle(rawImage.texture as Texture2D, colors[m[rand].palette], m[rand]);
            Debug.Log("triangle");
        }
        else if (m[rand].shape == Shape.pixelated)
        {
            rawImage.texture = createMond.DrawPixelated(rawImage.texture as Texture2D, colors[m[rand].palette], m[rand]);
            Debug.Log("Pixelated");
        }
        else if (m[rand].shape == Shape.circle)
        {
            rawImage.texture = createMond.DrawCircle(rawImage.texture as Texture2D, colors[m[rand].palette], m[rand]);
            Debug.Log("Circle");
        }
        else if (m[rand].shape == Shape.square)
        {
            rawImage.texture = createMond.DrawSquare(rawImage.texture as Texture2D, colors[m[rand].palette], m[rand]);
            Debug.Log("Square");
        }
        else if (m[rand].shape == Shape.mixed)
        {
            rawImage.texture = createMond.DrawMixed(rawImage.texture as Texture2D, colors[m[rand].palette], m[rand]);
            Debug.Log("Mixed");
        }
    }
    */
}

