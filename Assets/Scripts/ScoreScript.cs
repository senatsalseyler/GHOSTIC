using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase;
using Firebase.Extensions;

public class ScoreScript : MonoBehaviour
{
    public int scoreValue = 0;
    public Text scoreComp;


    public void IncreaseScore(int score)
    {
        int tempScore = this.scoreValue + score;
        if (tempScore < 0)
        {
            this.scoreValue = 0;
        }
        else
        {
            this.scoreValue = tempScore;
        }

        scoreComp.text = "" + this.scoreValue;
    }

    public async void WriteScore()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        CollectionReference colRef = db.Collection("leaderboard");
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        string uid = "";
        if (user != null)
        {
            return;
        }

        LeaderboardUser leaderboardUser = new LeaderboardUser()
            { Score = scoreValue, TimeStamp = new DateTime(), UserId = uid };
        await colRef.AddAsync(leaderboardUser);
    }
}