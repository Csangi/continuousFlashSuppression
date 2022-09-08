using UnityEngine;
public abstract class Item
{
    public int block { get; private set; }                            //organizational blocks, all trials labeled 1 go together in a single block  
    public bool random { get; private set; }                          //1 for a random sequence and 0 for no randomization                                                                                                            
    public string image { get; set; }                          //string to store the name of image                                                                               
    public string imagePath { get; set; }                               //file path to image                                                                 

    public Item(bool rand, int blk, string img)
    {
        random = rand;
        block = blk;
        image = img;
        imagePath = "";
    }
    public abstract void printTrial();
}

public class MondTrial : Item
{
    public int duration { get; private set; }                         //duration of trial in ms 
    public int flashDuration { get; private set; }                    //duration of flashing mondrians in ms   
    public float opacity { get; private set; }                          //% opacity to reach by the end of the trial                                                            
    public int delay { get; private set; }                            //delay before the image appears                      
    public int timeToReachOpacity { get; private set; } //the time it will take to reach the given opacity in ms  
    public int mond { get; private set; }
    public MondTrial(bool rand, int blk, string img, int dur, int flshdur, float opa, int dly, int TTRO, int mond) : base(rand, blk, img)
    {
        duration = dur;
        flashDuration = flshdur;
        opacity = opa;
        delay = dly;
        opacity = opa;
        timeToReachOpacity = TTRO;
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
        Debug.Log("opacity = " + opacity + "\t");
        Debug.Log("delay = " + delay + "\n");
        Debug.Log("time to reach opacity = " + timeToReachOpacity + "\n");
        Debug.Log("Mondrian number = " + mond + "\n");
        Debug.Log("---------------------end of trial print-------------------------");
    }
}

public class FlashMondTrial : Item
{
    public int duration { get; private set; }                         //duration of trial in ms 
    public int flashDuration { get; private set; }                    //duration of flashing mondrians in ms   
    public float opacity { get; private set; }                          //% opacity to reach by the end of the trial                                                            
    public int delay { get; private set; }                            //delay before the image appears                      
    public int timeToReachOpacity { get; private set; } //the time it will take to reach the given opacity in ms  
    public int mond { get; private set; }
    public FlashMondTrial(bool rand, int blk, string img, int dur, int flshdur, float opa, int dly, int TTRO, int mond) : base(rand, blk, img)
    {
        duration = dur;
        flashDuration = flshdur;
        opacity = opa;
        delay = dly;
        opacity = opa;
        timeToReachOpacity = TTRO;
        this.mond = mond;
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
        Debug.Log("opacity = " + opacity + "\t");
        Debug.Log("delay = " + delay + "\n");
        Debug.Log("time to reach opacity = " + timeToReachOpacity + "\n");
        Debug.Log("Mondrian number = " + mond + "\n");
        Debug.Log("---------------------end of trial print-------------------------");
    }
}

public class MaskTrial : Item
{
    public int duration { get; private set; }                         //duration of trial in ms 
    public int flashDuration { get; private set; }                    //duration of flashing mondrians in ms   
    public float opacity { get; private set; }                          //% opacity to reach by the end of the trial                                                            
    public int delay { get; private set; }                            //delay before the image appears                      
    public int timeToReachOpacity { get; private set; } //the time it will take to reach the given opacity in ms  
    public string mask { get; set; }
    public string maskPath { get; set; }
    public MaskTrial(bool rand, int blk, string img, int dur, int flshdur, float opa, int dly, int TTRO, string mask) : base(rand, blk, img)
    {
        duration = dur;
        flashDuration = flshdur;
        opacity = opa;
        delay = dly;
        opacity = opa;
        timeToReachOpacity = TTRO;
        this.mask = mask;
        maskPath = "";
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
        Debug.Log("opacity = " + opacity + "\t");
        Debug.Log("delay = " + delay + "\n");
        Debug.Log("time to reach opacity = " + timeToReachOpacity + "\n");
        Debug.Log("Mask name = " + mask + "\n");
        Debug.Log("---------------------end of trial print-------------------------");
    }
}

public class Instruction : Item
{
    public int duration;
    public float responseTime;
    public Instruction(bool rand, int blk, string img, int dur) : base(rand, blk, img)
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

public class Response : Item
{
    public string up, down, left, right, response;
    public float responseTime;
    public Response(bool rand, int blk, string img, string u, string d, string l, string r) : base(rand, blk, img)
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

public class Break : Item
{
    public int duration;
    public Break(bool rand, int blk, string img, int dur) : base(rand, blk, img)
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
