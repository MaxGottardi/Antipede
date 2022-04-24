using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleBag
{

    //based on this explanation https://gamedevelopment.tutsplus.com/tutorials/shuffle-bags-making-random-feel-more-random--gamedev-1249
    public GameObject[] shuffleList; //list of all elements wanting to pick from
    int currPos = -1;

    public GameObject getNext()//the next item in the list to get
    {
        if (currPos < 0)
        {
            currPos = shuffleList.Length - 1;
        }
        int randValue = Random.Range(0, currPos);
        //swapp the random item and the current item
        GameObject temp = shuffleList[randValue];
        shuffleList[randValue] = shuffleList[currPos];
        shuffleList[currPos] = temp;

        currPos--;
        return temp;
    }
}
