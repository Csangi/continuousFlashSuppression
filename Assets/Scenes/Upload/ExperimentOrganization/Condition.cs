using System.Collections.Generic;
using UnityEngine;

//------------------------------------------------Condition Class-----------------------------------------------------
public class Condition
{
    //-------------------------------------Attributes---------------------------------------------------------------------
    public List<Block> blocks;                                     //stored list of blocks                                          
    public List<int> index;                                        //and array of integers to keep the order of the index to make   
    public int numberOfBlocks { get; private set; }                 //counter of the number of total blocks                         
    public bool random { get; set; }                        //whether the blocks in the condition should be randomized              
    //-------------------------------------Functions----------------------------------------------------------------------
    public Condition(bool R)                                        //peramiterized constructor                                     
    {
        numberOfBlocks = 0;
        this.random = R;
        blocks = new List<Block>();                                 //initialize the array so that we can avoid a null reference exeption  
        index = new List<int>();
    }
    public void addBlocks(Block B)                                  //function to add new blocks to condition                       
    {
        blocks.Add(B);                                              //concatinate new block onto end of block list                  
        index.Add(numberOfBlocks);                                  //add the index of the new number to the index array so it may  
        numberOfBlocks++;                                           //increment index of blocks                                     
                                                                    //be randomized later                                           
    }
    public void printCondition()                                    //function to print whole condition using previous functions    
    {
        Debug.Log("----------------------------------------------------------------------------------Condition Start");
        for (int i = 0; i < numberOfBlocks; i++)                     //loop through list of blocks                                 
        {
            Debug.Log("Block Number " + i);                         //log the index number                                          
            this.blocks[this.index[i]].printBlock();                //print the block which will print the trials                   
        }
        Debug.Log("-------------------------------------------------------------------------------------Condition End");
    }
    public void randomizeBlocks()                                   //function to randomize the blocks inside of the condition      
    {

        int x, holder;

        for (int i = 0; i < numberOfBlocks; i++)                //loop through the list of blocks                              
        {
            blocks[i].randomizeTrials();
            if (blocks[i].random)
            {
                do
                {
                    x = Random.Range(0, numberOfBlocks - 1);    //use x as a random variable                                    
                } while (!blocks[x].random);                    //while the random block is a block that can be randomized      
                holder = index[i];                             //have the holder hold the value of the index of block[i]        
                index[i] = index[x];                          //swap i and x                                                    
                index[x] = holder;                             //swap x and i                                                   
            }
        }
    }
}
