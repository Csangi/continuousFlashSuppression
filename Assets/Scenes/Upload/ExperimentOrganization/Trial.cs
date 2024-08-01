using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class Item
{
    public bool hasMultipleStims { get; private set; }
    public int block { get; private set; }                            //organizational blocks, all trials labeled 1 go together in a single block  
    public int random { get; private set; }                          //1 for a random sequence and 0 for no randomization                                                                                                            
    public string image { get; set; }                          //string to store the name of image                                                                               
    public string imagePath { get; set; }                               //file path to image
    public string image2 { get; set; }
    public string image2Path { get; set; }
    public string error { get; set; }
    public List<string> PassThroughColumns = new List<string>();
    public Item(int rand, int blk, string img, bool multi, string img2, List<string> passthrough)
    {
        error = "";
        random = rand;
        block = blk;
        image = img;
        imagePath = "";
        hasMultipleStims = multi;
        image2 = img2;
        image2Path = "";
        PassThroughColumns = passthrough;
    }
    public abstract void printTrialToConsole();
    public abstract void simCheck();
    public abstract string printTrial(int cond, bool condRand, int blk, bool blkRand, int trial, int counter);
}

//---------------Trial using modrians--------------------------------------
public class MondTrial : Item
{
    public int duration { get; private set; }                         //duration of trial in ms 
    public int flashDuration { get; private set; }                    //duration of flashing mondrians in ms   
    public float opacity { get; private set; }                          //% opacity to reach by the end of the trial                                                            
    public int maskDelay { get; private set; }                            //delay before the image appears                      
    public int staticDelay { get; private set; } //the time it will take to reach the given opacity in ms  
    public string mond { get; private set; }
    public string left { get; private set; }
    public string right { get; private set; }
    public string up { get; private set; }
    public string down { get; private set; }
    public string response { get; set; }
    public int timeToReachOpacity { get; set; }
    public int quadrant { get; set; }
    public string responseTime { get; set; }
    public bool isResponse { get; private set; }  //if the trial takes in reponse
    public bool responseStop { get; private set; }  //if the trial should end after the response is taken in
    //default constructor
    public MondTrial(int rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, string mond, string img2, string u, string d, string l, string r, bool resp, bool stop, int TTRO, int quad, List<string> passthrough) : base(rand, blk, img, multi, img2, passthrough)
    {
        duration = dur;
        flashDuration = flshdur;
        opacity = opa;
        maskDelay = mskdly;
        opacity = opa;
        staticDelay = stcdly;
        this.mond = mond;
        left = l;
        right = r;
        up = u;
        down = d;
        isResponse = resp;
        responseStop = stop;
        response = responseTime = "";
        timeToReachOpacity = TTRO;
        quadrant = quad;
    }

    public override string printTrial(int cond, bool condRand, int blk, bool blkRand, int trial, int counter)
    {
        string writer = "";
        writer += ++cond + ",";
        writer += condRand + ",";
        writer += ++blk + ",";
        writer += blkRand + ",";
        if (hasMultipleStims)
        {
            writer += "multi_stim_noise_as_mask,";
            writer += ++counter + ",";
            writer += ++trial + ",";
            writer += random + ",";
            writer += image + "_" + image2 + ",";
        }
        else
        {
            writer += "noise_as_mask,";
            writer += ++counter + ",";
            writer += ++trial + ",";
            writer += random + ",";
            writer += image + ",";
        }
        writer += duration + ",";
        writer += flashDuration + ",";
        writer += opacity + ",";
        writer += maskDelay + ",";
        writer += staticDelay + ",";
        writer += mond + ",";
        writer += responseTime + ",";
        writer += up + ",";
        writer += down + ",";
        writer += left + ",";
        writer += right + ",";
        writer += response + ",";
        writer += ",";
        writer += timeToReachOpacity + ",";
        writer += quadrant + ",";
        writer += (!responseStop) + ",";
        writer += DateTime.Now + ",";
        foreach ( string pass in PassThroughColumns)
        {
            writer += pass + ",";
        }
        writer += "\n";
        return writer; 
    }
    public override void printTrialToConsole()
    {
        Debug.Log("---------------------start of mond trial print-----------------------");
        Debug.Log("Type = Trial\n");
        Debug.Log("random = " + random + "\t");
        Debug.Log("block = " + block + "\t");
        Debug.Log("duration = " + duration + "\t");
        Debug.Log("flash duration = " + flashDuration + "\t");
        Debug.Log("image = " + image + "\t");
        if (hasMultipleStims)
            Debug.Log("image 2 = " + image2 + "\t");
        Debug.Log("opacity = " + opacity + "\t");
        Debug.Log("mask delay = " + maskDelay + "\n");
        Debug.Log("static delay = " + staticDelay + "\n");
        Debug.Log("Mondrian number = " + mond + "\n");
        Debug.Log("Response = " + isResponse + "\n");
        Debug.Log("Time to Reach Opacity = " + timeToReachOpacity + "\n");
        Debug.Log("quadrant = " + quadrant + "\n");
        if (isResponse)
        {
            Debug.Log("Left = " + left + "\n");
            Debug.Log("Right = " + right + "\n");
            Debug.Log("Up = " + up + "\n");
            Debug.Log("Down = " + down + "\n");
        }
        Debug.Log("---------------------end of trial print-------------------------");
    }
    public override void simCheck()
    {
        if (duration % flashDuration != 0)
            error += "Duration must be divisible by flash duration.";
        if (maskDelay % flashDuration != 0)
            error += "Mask delay must be divisible by flash duration.";
        if (staticDelay % flashDuration != 0)
            error += "Static delay must be divisible by flash duration.";
        if (maskDelay > duration)
            error += "Mask delay must be less than the duration.";
        if (staticDelay > duration)
            error += "Static delay must be less than duration.";
        if (staticDelay < maskDelay)
            error += "Mask delay must be less than the static delay.";
    }
}

//-------------------A mond trial with the same flashing from the mask trial----------------------------------------
public class FlashMondTrial : Item
{
    public int duration { get; private set; }                         //duration of trial in ms 
    public int flashDuration { get; private set; }                    //duration of flashing mondrians in ms   
    public float opacity { get; private set; }                          //% opacity to reach by the end of the trial                                                            
    public int maskDelay { get; private set; }                            //delay before the image appears                      
    public int staticDelay { get; private set; } //the time it will take to reach the given opacity in ms  
    public string mond { get; private set; }           //which mond the trial uses
    public float flashPeriod { get; private set; }
    public string left { get; private set; }       //if the trial takes in response what are the responses
    public string right { get; private set; }
    public string up { get; private set; }
    public string down { get; private set; }
    public int timeToReachOpacity { get; set; }
    public int quadrant { get; set; }
    public string response { get; set; }
    public string responseTime { get; set; }
    public bool isResponse { get; private set; }  //if the trial takes in reponse
    public bool responseStop { get; private set; }  //if the trial should end after the response is taken in
    //default constructor
    public FlashMondTrial(int rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, string mond, float flash, string img2, string u, string d, string l, string r, bool resp, bool stop, int TTRO, int quad, List<string> passthrough) : base(rand, blk, img, multi, img2, passthrough)
    {
        duration = dur;
        flashDuration = flshdur;
        opacity = opa;
        maskDelay = mskdly;
        opacity = opa;
        staticDelay = stcdly;
        this.mond = mond;
        flashPeriod = flash;
        left = l;
        right = r;
        up = u;
        down = d;
        isResponse = resp;
        responseStop = stop;
        response = responseTime = "";
        timeToReachOpacity = TTRO;
        quadrant = quad;
    }
    public override string printTrial(int cond, bool condRand, int blk, bool blkRand, int trial, int counter)
    {
        string writer = "";
        writer += ++cond + ",";
        writer += condRand + ",";
        writer += ++blk + ",";
        writer += blkRand + ",";
        if (hasMultipleStims)
        {
            writer += "multi_stim_noise_as_mask,";
            writer += ++counter + ",";
            writer += ++trial + ",";
            writer += random + ",";
            writer += image + "_" + image2 + ",";
        }
        else
        {
            writer += "noise_as_mask,";
            writer += ++counter + ",";
            writer += ++trial + ",";
            writer += random + ",";
            writer += image + ",";
        }
        writer += duration + ",";
        writer += flashDuration + ",";
        writer += opacity + ",";
        writer += maskDelay + ",";
        writer += staticDelay + ",";
        writer += mond + ",";
        writer += responseTime + ",";
        writer += up + ",";
        writer += down + ",";
        writer += left + ",";
        writer += right + ",";
        writer += response + ",";
        writer += flashPeriod + ",";
        writer += timeToReachOpacity + ",";
        writer += quadrant + ",";
        writer += (!responseStop) + ",";
        writer += DateTime.Now + ",";
        foreach (string pass in PassThroughColumns)
        {
            writer += pass + ",";
        }
        writer += "\n";
        return writer; 
    }
    //debug print statement for error checking
    public override void printTrialToConsole()
    {
        Debug.Log("---------------------start of flash mond trial print-----------------------");
        Debug.Log("Type = Trial\n");
        Debug.Log("random = " + random + "\t");
        Debug.Log("block = " + block + "\t");
        Debug.Log("duration = " + duration + "\t");
        Debug.Log("flash duration = " + flashDuration + "\t");
        Debug.Log("image = " + image + "\t");
        if (hasMultipleStims)
            Debug.Log("image 2 = " + image2 + "\t");
        Debug.Log("opacity = " + opacity + "\t");
        Debug.Log("mask delay = " + maskDelay + "\n");
        Debug.Log("static delay = " + staticDelay + "\n");
        Debug.Log("Mondrian number = " + mond + "\n");
        Debug.Log("Flash Period = " + flashPeriod + "\n");
        Debug.Log("Response = " + isResponse + "\n");
        Debug.Log("Time to Reach Opacity = " + timeToReachOpacity + "\n");
        Debug.Log("quadrant = " + quadrant + "\n");
        if (isResponse)
        {
            Debug.Log("Left = " + left + "\n");
            Debug.Log("Right = " + right + "\n");
            Debug.Log("Up = " + up + "\n");
            Debug.Log("Down = " + down + "\n");
        }
        Debug.Log("---------------------end of trial print-------------------------");
    }
    public override void simCheck()
    {
        if (duration % flashDuration != 0)
            error += "Duration must be divisible by flash duration.";
        if (maskDelay % flashDuration != 0)
            error += "Mask delay must be divisible by flash duration.";
        if (staticDelay % flashDuration != 0)
            error += "Static delay must be divisible by flash duration.";
        if (maskDelay > duration)
            error += "Mask delay must be less than the duration.";
        if (staticDelay > duration)
            error += "Static delay must be less than duration.";
        if (staticDelay < maskDelay)
            error += "Mask delay must be less than the static delay.";
        if (flashPeriod > flashDuration)
            error += "Blank period must be less than flash duration.";
    }
}

//-------------------------A trial which uses a mask image rather than a mondrian--------------------------------------
public class MaskTrial : Item
{
    public float flashPeriod { get; private set; }                          //the time that the image is on the screen for
    public int duration { get; private set; }                         //duration of trial in ms 
    public int flashDuration { get; private set; }                    //duration of flashing mondrians in ms   
    public float opacity { get; private set; }                          //% opacity to reach by the end of the trial                                                            
    public int maskDelay { get; private set; }                            //delay before the image appears                      
    public int staticDelay { get; private set; } //the time it will take to reach the given opacity in ms  
    public string mask { get; set; }
    public string maskPath { get; set; }
    public string left { get; private set; }
    public string right { get; private set; }
    public string up { get; private set; }
    public string down { get; private set; }
    public int timeToReachOpacity { get; set; }
    public int quadrant { get; set; }
    public string response { get; set; }
    public string responseTime { get; set; }
    public bool isResponse { get; private set; }  //if the trial takes in reponse
    public bool responseStop { get; private set; }  //if the trial should end after the response is taken in
    //default constructor
    public MaskTrial(int rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, string mask, float flash, string img2, string u, string d, string l, string r, bool resp, bool stop, int TTRO, int quad, List<string> passthrough) : base(rand, blk, img, multi, img2, passthrough)
    {
        duration = dur;
        flashDuration = flshdur;
        opacity = opa;
        maskDelay = mskdly;
        opacity = opa;
        staticDelay = stcdly;
        this.mask = mask;
        maskPath = "";
        flashPeriod = flash;
        left = l;
        right = r;
        up = u;
        down = d;
        isResponse = resp;
        responseStop = stop;
        response = responseTime = "";
        timeToReachOpacity = TTRO;
        quadrant = quad;
    }
    public override string printTrial(int cond, bool condRand, int blk, bool blkRand, int trial, int counter)
    {
        string writer = "";
        writer += ++cond + ",";
        writer += condRand + ",";
        writer += ++blk + ",";
        writer += blkRand + ",";
        if (hasMultipleStims)
        {
            writer += "multi_stim_object_as_mask,";
            writer += ++counter + ",";
            writer += ++trial + ",";
            writer += random + ",";
            writer += image + "_" + image2 + ",";
        }
        else
        {
            writer += "object_as_mask,";
            writer += ++counter + ",";
            writer += ++trial + ",";
            writer += random + ",";
            writer += image + ",";
        }
        writer += duration + ",";
        writer += flashDuration + ",";
        writer += opacity + ",";
        writer += maskDelay + ",";
        writer += staticDelay + ",";
        writer += mask + ",";
        writer += responseTime + ",";
        writer += up + ",";
        writer += down + ",";
        writer += left + ",";
        writer += right + ",";
        writer += response + ",";
        writer += flashPeriod + ",";
        writer += timeToReachOpacity + ",";
        writer += quadrant + ",";
        writer += (!responseStop) + ",";
        writer += DateTime.Now + ",";
        foreach (string pass in PassThroughColumns)
        {
            writer += pass + ",";
        }
        writer += "\n";
        return writer; 
    }
    public override void printTrialToConsole()
    {
        Debug.Log("---------------------start of mask trial print-----------------------");
        Debug.Log("Type = Trial\n");
        Debug.Log("random = " + random + "\t");
        Debug.Log("block = " + block + "\t");
        Debug.Log("duration = " + duration + "\t");
        Debug.Log("flash duration = " + flashDuration + "\t");
        Debug.Log("image = " + image + "\t");
        if (hasMultipleStims)
            Debug.Log("image 2 = " + image2 + "\t");
        Debug.Log("opacity = " + opacity + "\t");
        Debug.Log("mask delay = " + maskDelay + "\n");
        Debug.Log("static delay = " + staticDelay + "\n");
        Debug.Log("Mask name = " + mask + "\n");
        Debug.Log("Flash period = " + flashPeriod + "\n");
        Debug.Log("Response = " + isResponse + "\n");
        Debug.Log("Time to Reach Opacity = " + timeToReachOpacity + "\n");
        Debug.Log("quadrant = " + quadrant + "\n");
        if (isResponse)
        {
            Debug.Log("Left = " + left + "\n");
            Debug.Log("Right = " + right + "\n");
            Debug.Log("Up = " + up + "\n");
            Debug.Log("Down = " + down + "\n");
        }
        Debug.Log("---------------------end of trial print-------------------------");
    }
    public override void simCheck()
    {
        if (duration % flashDuration != 0)
            error += "Duration must be divisible by flash duration.";
        if (maskDelay % flashDuration != 0)
            error += "Mask delay must be divisible by flash duration.";
        if (staticDelay % flashDuration != 0)
            error += "Static delay must be divisible by flash duration.";
        if (maskDelay > duration)
            error += "Mask delay must be less than the duration.";
        if (staticDelay > duration)
            error += "Static delay must be less than duration.";
        if (staticDelay < maskDelay)
            error += "Mask delay must be less than the static delay.";
        if (flashPeriod > flashDuration)
            error += "Blank period must be less than flash duration.";
    }
}

//--------------------------------instruction-----------------------------------------
public class Instruction : Item
{
    public int duration;
    public float responseTime;
    public Instruction(int rand, int blk, string img, int dur, List<string> passThrough) : base(rand, blk, img, false, "", passThrough)
    {
        duration = dur;
        responseTime = 0;
    }
    public override string printTrial(int cond, bool condRand, int blk, bool blkRand, int trial, int counter)
    {
        string writer = "";
        writer += ++cond + ",";
        writer += condRand + ",";
        writer += ++blk + ",";
        writer += blkRand + ",";
        writer += "instruction,";
        writer += ++counter + ",";
        writer += ++trial + ",";
        writer += random + ",";
        writer += image + ",";
        writer += duration + ",";
        writer += ",,,,,";
        writer += responseTime + ",";
        writer += ",,,,,,,,,";
        writer += DateTime.Now + ",";
        foreach (string pass in PassThroughColumns)
        {
            writer += pass + ",";
        }
        writer += "\n";
        return writer; 
    }
    public override void printTrialToConsole()
    {
        Debug.Log("---------------------start of trial print-----------------------");
        Debug.Log("Type = Instruction\n");
        Debug.Log("random = " + random + "\t");
        Debug.Log("block = " + block + "\t");
        Debug.Log("duration = " + duration + "\t");
        Debug.Log("image = " + image + "\t");
        Debug.Log("---------------------end of trial print-------------------------");
    }
    public override void simCheck()
    {
        if (duration < 0)
            error += "duration must be greater than zero";
    }
}

//-------------------------------------Response trial------------------------------------------
public class Response : Item
{
    public string up, down, left, right, response;
    public float responseTime;
    public Response(int rand, int blk, string img, string u, string d, string l, string r, List<string> passThrough) : base(rand, blk, img, false, "", passThrough)
    {
        up = u;
        down = d;
        left = l;
        right = r;
        response = "";
        responseTime = 0;
    }
    public override string printTrial(int cond, bool condRand, int blk, bool blkRand, int trial, int counter)
    {
        string writer = "";
        writer += ++cond + ",";
        writer += condRand + ",";
        writer += ++blk + ",";
        writer += blkRand + ",";
        writer += "response,";
        writer += ++counter + ",";
        writer += ++trial + ",";
        writer += random + ",";
        writer += image + ",,,,,,,";
        writer += responseTime + ",";
        writer += up + ",";
        writer += down + ",";
        writer += left + ",";
        writer += right + ",";
        writer += response + ",";
        writer += ",,,,";
        writer += DateTime.Now.ToString() + ",";
        foreach (string pass in PassThroughColumns)
        {
            writer += pass + ",";
        }
        writer += "\n";
        return writer;
    }
    public override void printTrialToConsole()
    {
        Debug.Log("---------------------start of trial print-----------------------");
        Debug.Log("Type = Response\n");
        Debug.Log("random = " + random + "\t");
        Debug.Log("block = " + block + "\t");
        Debug.Log("image = " + image + "\t");
        Debug.Log("up = " + up + "\t");
        Debug.Log("down = " + down + "\t");
        Debug.Log("left = " + left + "\t");
        Debug.Log("right = " + right + "\t");
        Debug.Log("---------------------end of trial print-------------------------");
    }
    public override void simCheck()
    {

    }
}

//---------------------------------------------Break trial--------------------------------------------
public class Break : Item
{
    public int duration;
    public Break(int rand, int blk, string img, int dur, List<string> passThrough) : base(rand, blk, img, false, "", passThrough)
    {
        duration = dur;
    }
    public override string printTrial(int cond, bool condRand, int blk, bool blkRand, int trial, int counter)
    {
        string writer = "";
        writer += ++cond + ",";
        writer += condRand + ",";
        writer += ++blk + ",";
        writer += blkRand + ",";
        writer += "break,";
        writer += ++counter + ",";
        writer += ++trial + ",";
        writer += random + ",";
        writer += image + ",";
        writer += duration + ",";
        writer += ",,,,,,,,,,,,,,,";
        writer += DateTime.Now + ",";
        foreach (string pass in PassThroughColumns)
        {
            writer += pass + ",";
        }
        writer += "\n";
        return writer; 
    }
    public override void printTrialToConsole()
    {
        Debug.Log("---------------------start of trial print-----------------------");
        Debug.Log("Type = Break\n");
        Debug.Log("random = " + random + "\t");
        Debug.Log("block = " + block + "\t");
        Debug.Log("duration = " + duration + "\t");
        Debug.Log("image = " + image + "\t");
        Debug.Log("---------------------end of trial print-------------------------");
    }
    public override void simCheck()
    {
        if (duration < 0)
            error += "Duration must be greater than zero.";
    }
}