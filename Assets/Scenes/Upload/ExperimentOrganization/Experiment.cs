using System.Collections.Generic;
using UnityEngine;

//-------------------------------------------------------------Experiment Class------------------------------------------------------------------------------------------------
public class Experiment                                                 //Final class Experiment which contains a List of Conditions, which contains a List of blocks                       
{                                                                       //which contains a list of Trials                                                                                   
    //---------------------------------------------------------Attributes---------------------------------------------------------------------------------------------------
    public List<Condition> conditions;                                 //the array of conditions that are stored in the experiment                                                          
    public List<int> index;                                            //the is the order that the conditions are played out in                                                             
    public int numberOfConditions { get; set; }                        //document the number of conditions                                                                                  
    public bool right, left;                        //which eye is used
    public string ID;                               //the input ID which will be used for the output of the file.
    public bool hasUploaded;                        //whether the experiment has been uploaded yet or not for when the experiment returns to the 
    public bool successfulUpload;                   //if the experiment was uploaded successfully
    public bool mondsHaveBeenDrawn;                 //if the mondrians have been drawn yet
    public int sceneToBeLoaded;                     //the next scene that will be loaded
    public string path;                             //output path which is uploaded with the experiment
    public string inputPath;                        //path where the original csv was uploaded from
    public int count;                               //the number of trials
    public string palettePath;                      //the path of the palette
    public static Experiment current
    {
        get
        {
            if (instance == null)
                instance = new Experiment();
            return instance;
        }
    }

    private static Experiment instance;

    public List<Mondrian> Mondrians;
    //---------------------------------------------------------Functions----------------------------------------------------------------------------------------------------------
    public Experiment()                                                 //default constructor                                                                                               
    {                                                                   //                                                                                                                  
        numberOfConditions = 0;                                         //set the number of conditions to 0                                                                                 
        conditions = new List<Condition>();
        index = new List<int>();
        Mondrians = new List<Mondrian>();
        right = left = false;
        ID = "";
        hasUploaded = false;
        mondsHaveBeenDrawn = false;
        successfulUpload = false;
        sceneToBeLoaded = 0;
        path = "";
        inputPath = "";
        count = 0;
    }
    public void addCondition(Condition C)                               //adding a new condition to the main array of conditions                                                            
    {                                                                   //                                                                                                                  
        conditions.Add(C);                                              //add new condition                                                                                                 
        index.Add(this.numberOfConditions);                             //add new number to the index                                                                                       
        numberOfConditions++;                                           //increment the numberOfConditions                                                                                  
    }
    public void setConditionOrder(int a)                                //this function is able to take in a number such as 532641 and set it at the                                        
    {                                                                   //order for the list of conditions                                                                                  
        List<int> numbers = new List<int>();                            //first we initialize a list of integers                                                                            
        int x = 0;                                                      //has our placeholder                                                                                               
        if (this.numberOfConditions >= 2)
        {
            do                                                          //while our input still has numbers inside of it that need to be extracted                                          
            {
                x = (a % 10) - 1;                                       //take the modulous of a (getting the last digit in the number) and subtract by one to get that number as a index   
                a /= 10;                                                //divide by 10 to get rid of the number that we just took in                                                        
                numbers.Add(x);                                         //add the new number to the array of numbers                                                                        
            } while (a > 0);

            for (int i = 0; i < numbers.Count / 2; i++)                 //now we have to flip the numbers array to get the order in the correct orientation                                 
            {
                x = numbers[i];                                         //have our holder value x be set to i                                                                               
                numbers[i] = numbers[numbers.Count - i - 1];             //have i be set to j                                                                                                
                numbers[numbers.Count - i - 1] = x;                      //have j be set to x which was holding i's value                                                                    
            }

            for (int i = 0; i < numbers.Count; i++)                     //now we set the new order to be the index of the experiment                                                   
            {
                this.index[i] = numbers[i];
            }
            numberOfConditions = numbers.Count;
        }
    }
    public void printExperiment()
    {
        Debug.Log("---------------------------------------------------------------------------------------------------------Experiment Start");
        for (int i = 0; i < numberOfConditions; i++)                    //loop through list of blocks                                                                                       
        {
            Debug.Log("Condition Number " + i);                         //log the index number                                                                                              
            this.conditions[this.index[i]].printCondition();            //print the condition which will print the blocks                                                                   
        }
        Debug.Log("----------------------------------------------------------------------------------------------------------Experiment End");
    }

    public void printMondrians()
    {
        Debug.Log("Printing mondrians... ----------------------------");
        for (int i = 0; i < Mondrians.Count; ++i)
        {
            Debug.Log("Mond number: " + i + " start ------------------------");
            Mondrians[i].printMond();
            Debug.Log("Mond number: " + i + " end ------------------------");
        }
        Debug.Log("End of printint mondrians ------------------------");
    }

    public void clearExperiment()
    {
        conditions.Clear();
        Mondrians.Clear();
        index.Clear();
        hasUploaded = false;
        numberOfConditions = 0;
    }

    public void randomizeConditions()
    {
        int x, holder;

        for (int i = 0; i < numberOfConditions; i++)                //loop through the list of blocks                               
        {
            conditions[i].randomizeBlocks();
            if (conditions[i].random)
            {
                do
                {
                    x = Random.Range(0, numberOfConditions - 1);    //use x as a random variable                                    
                } while (!conditions[x].random);                    //while the random block is a block that can be randomized      
                holder = index[i];                             //have the holder hold the value of the index of block[i]        
                index[i] = index[x];                          //swap i and x                                                    
                index[x] = holder;                             //swap x and i                                                   
            }
        }
    }
}

