using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;
using Firebase;
using Firebase.Extensions;


public class Leaderboard : MonoBehaviour
{
    // Start is called before the first frame update

    async Task<List<LeaderboardUser>> getGlobalLeaderboard()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        CollectionReference colRef = db.Collection("leaderboard");
        Query leaderboardQuery = colRef.OrderBy("account_id").OrderByDescending("score");
        QuerySnapshot leaderboardQuerySs = await leaderboardQuery.GetSnapshotAsync();
        var leaderboardUsers = new List<LeaderboardUser>();
        foreach (DocumentSnapshot documentSnapshot in leaderboardQuerySs.Documents)
        {
            var documentDict = documentSnapshot.ToDictionary();
            var json = JsonUtility.ToJson(documentDict);
            LeaderboardUser leaderboardUser = JsonUtility.FromJson<LeaderboardUser>(json);
            leaderboardUsers.Append(leaderboardUser);
        }

        return leaderboardUsers;
    } 
    
    async Task<List<LeaderboardUser>> getPersonalLeaderboard()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        CollectionReference colRef = db.Collection("leaderboard");
        Query leaderboardQuery = colRef.WhereEqualTo("account_id","").OrderByDescending("score");
        QuerySnapshot leaderboardQuerySs = await leaderboardQuery.GetSnapshotAsync();
        var leaderboardUsers = new List<LeaderboardUser>();
        foreach (DocumentSnapshot documentSnapshot in leaderboardQuerySs.Documents)
        {
            var documentDict = documentSnapshot.ToDictionary();
            var json = JsonUtility.ToJson(documentDict);
            LeaderboardUser leaderboardUser = JsonUtility.FromJson<LeaderboardUser>(json);
            leaderboardUsers.Append(leaderboardUser);
        }

        return leaderboardUsers;
    }
}