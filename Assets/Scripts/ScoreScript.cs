using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScoreScript : MonoBehaviour
{
    private int scoreValue = 0;
    public Text scoreComp;
    

    public void IncreaseScore(int score)
    {
        int tempScore = this.scoreValue + score;
        if(tempScore < 0)
        {
            this.scoreValue = 0;
        }
        else
        {
            this.scoreValue = tempScore;
        }
        scoreComp.text = "" + this.scoreValue;
    }
}
