using UnityEngine;
public abstract class Item
{
    public bool hasMultipleStims { get; private set; }
    public int block { get; private set; }                            //organizational blocks, all trials labeled 1 go together in a single block  
    public bool random { get; private set; }                          //1 for a random sequence and 0 for no randomization                                                                                                            
    public string image { get; set; }                          //string to store the name of image                                                                               
    public string imagePath { get; set; }                               //file path to image
    public string image2 { get; set; }
    public string image2Path { get; set; }

    public Item(bool rand, int blk, string img, bool multi, string img2)
    {
        random = rand;
        block = blk;
        image = img;
        imagePath = "";
        hasMultipleStims = multi;
        image2 = img2;
        image2Path = "";
    }
    public abstract void printTrial();
}

//---------------Trial using modrians--------------------------------------
public class MondTrial : Item
{
    public int duration { get; private set; }                         //duration of trial in ms 
    public int flashDuration { get; private set; }                    //duration of flashing mondrians in ms   
    public float opacity { get; private set; }                          //% opacity to reach by the end of the trial                                                            
    public int maskDelay { get; private set; }                            //delay before the image appears                      
    public int staticDelay { get; private set; } //the time it will take to reach the given opacity in ms  
    public int mond { get; private set; }
    public MondTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, int mond, string img2) : base(rand, blk, img, multi, img2)
    {
        duration = dur;
        flashDuration = flshdur;
        opacity = opa;
        maskDelay = mskdly;
        opacity = opa;
        staticDelay = stcdly;
        this.mond = mond;

    }

    public override void printTrial()
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
        Debug.Log("---------------------end of trial print-------------------------");
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
    public int mond { get; private set; }
    public float flashPeriod { get; private set; }
    public FlashMondTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, int mond, float flash, string img2) : base(rand, blk, img, multi, img2)
    {
        duration = dur;
        flashDuration = flshdur;
        opacity = opa;
        maskDelay = mskdly;
        opacity = opa;
        staticDelay = stcdly;
        this.mond = mond;
        flashPeriod = flash;
    }

    public override void printTrial()
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
        Debug.Log("---------------------end of trial print-------------------------");
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
    public MaskTrial(bool rand, int blk, string img, bool multi, int dur, int flshdur, float opa, int mskdly, int stcdly, string mask, float flash, string img2) : base(rand, blk, img, multi, img2)
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
    }

    public override void printTrial()
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
        Debug.Log("---------------------end of trial print-------------------------");
    }
}

//--------------------------------instruction-----------------------------------------
public class Instruction : Item
{
    public int duration;
    public float responseTime;
    public Instruction(bool rand, int blk, string img, int dur) : base(rand, blk, img, false, "")
    {
        duration = dur;
        responseTime = 0;
    }

    public override void printTrial()
    {
        Debug.Log("---------------------start of trial print-----------------------");
        Debug.Log("Type = Instruction\n");
        Debug.Log("random = " + random + "\t");
        Debug.Log("block = " + block + "\t");
        Debug.Log("duration = " + duration + "\t");
        Debug.Log("image = " + image + "\t");
        Debug.Log("---------------------end of trial print-------------------------");
    }
}

//-------------------------------------Response trial------------------------------------------
public class Response : Item
{
    public string up, down, left, right, response;
    public float responseTime;
    public Response(bool rand, int blk, string img, string u, string d, string l, string r) : base(rand, blk, img, false, "")
    {
        up = u;
        down = d;
        left = l;
        right = r;
        response = "";
        responseTime = 0;
    }
    public override void printTrial()
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
}

//---------------------------------------------Break trial--------------------------------------------
public class Break : Item
{
    public int duration;
    public Break(bool rand, int blk, string img, int dur) : base(rand, blk, img, false, "")
    {
        duration = dur;
    }

    public override void printTrial()
    {
        Debug.Log("---------------------start of trial print-----------------------");
        Debug.Log("Type = Break\n");
        Debug.Log("random = " + random + "\t");
        Debug.Log("block = " + block + "\t");
        Debug.Log("duration = " + duration + "\t");
        Debug.Log("image = " + image + "\t");
        Debug.Log("---------------------end of trial print-------------------------");
    }
}
