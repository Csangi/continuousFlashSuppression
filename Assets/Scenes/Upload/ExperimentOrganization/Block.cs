using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------Block Class-------------------------------------------------------------------------------
public class Block
{
    //----------------------------------------Different Attributes---------------------------------------------------------------------------------------------------------
    public List<Item> trials;                                   //added an array of trials to block so they can be better organized and randomized                          
    public List<int> index;                                      //we are using an array of integers to keep track of the order of index as to not use assignment opperator  
    public int numberOfTrials { get; private set; }               //counter of the number of trials                                                                      
    public bool random { get; set; }                              //Whether the block should be randomized with other blocks in the condition                             
    public char imageRandomizationtype { get; set; }
    //-------------------------------------------Functions----------------------------------------------------------------------------------------------------------------
    public Block(bool R)                                          //standard constructor                                                                                                    
    {
        this.numberOfTrials = 0;
        this.random = R;
        this.trials = new List<Item>();                          //initialize array to avoid null reference exeption                                                     
        this.index = new List<int>();
    }
    public void addTrial(Item T)                                 //This is to add a new trial everytime there is an input                                                     
    {
        trials.Add(T);                                            //add the new trial to the array of trials                                                               
        index.Add(numberOfTrials);                                //add the new trial to the index, index = numberOfTrials -1 as indexes start at 0 rather than 1         
        numberOfTrials++;                                         //increment the number of trials to keep a good accurate index                                           
    }
    public void randomizeTrials()                                 //function to randomize trials given whether they were supposed to be randomized                          
    {
        int x, holder;
        Debug.Log("We in this bitch 1");
        if (numberOfTrials > 1)
            for (int i = 0; i < numberOfTrials; i++)                      //loop through the array of trials                                                                        
            {//Make a random variable x that has a range between 0 and number of trials -1 (-1 because index starts at 0) and makes a new random number every loop
                Debug.Log("We in this bitch 2");
                if (this.trials[i].random != 0)//if the trial was set to be randomized and is a version trial                                            
                {
                    do
                    {//Make a random variable x that has a range between 0 and number of trials -1 (-1 because index starts at 0) and makes a new random number every loop      
                        x = Random.Range(0, numberOfTrials);
                    } while (this.trials[x].random != this.trials[i].random);//while the random trial is also a trial that can be randomized
                    Debug.Log(i + "is switching with " + x);
                    Debug.Log(this.trials[i].random + "<-Their random numbers->" + this.trials[x].random);
                    holder = this.index[i];
                    this.index[i] = this.index[x];
                    this.index[x] = holder;
                }
            }
    }

    public void printBlock()                                      //function to print out our Block using the previous Trial printer function                               
    {
        Debug.Log("---------------------------------------------------------------start of block");
        for (int i = 0; i < numberOfTrials; i++)
        {
            Debug.Log("Trial number " + i);
            this.trials[this.index[i]].printTrial();
        }
        Debug.Log("-----------------------------------------------------------------end of block");
    }
}
