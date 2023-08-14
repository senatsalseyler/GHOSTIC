using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase;
using Firebase.Extensions;

public class ScoreScriptNew : MonoBehaviour
{
    public int scoreValue = 0;
    public Text scoreComp;
    
    public Transform player;
    private int playerPosY;
    [HideInInspector]
    public int finalScore;

    public void FixedUpdate()
    {
        playerPosY = Mathf.CeilToInt(player.position.y * 10 - 21);
        //playerPosY = (int) player.position.y * 10;
        finalScore = playerPosY + scoreValue;
        if (finalScore >= 0)
        {
            scoreComp.text = "" + finalScore;
        }
        else
        {
            scoreComp.text = "" + 0;
        }
    }

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
        finalScore += this.scoreValue;
    }

    public async void WriteScore()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        CollectionReference colRef = db.Collection("leaderboard");
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            return;
        }

        LeaderboardUser leaderboardUser = new LeaderboardUser()
            { Score = finalScore, TimeStamp = new DateTime(), UserId = user.UserId, UserName = user.DisplayName,Platform = Application.platform.ToString() };
        await colRef.AddAsync(leaderboardUser);
    }
}