using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;
using Firebase;
using Firebase.Extensions;


public class GlobalLeaderboard : MonoBehaviour
{
    // Start is called before the first frame updatep
    public GameObject CellPrefab;
    async Task<List<LeaderboardUser>> getGlobalLeaderboard()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        Query colRef = db.Collection("leaderboard");
        Query leaderboardQuery = colRef.OrderBy("UserId").OrderByDescending("Score");
        QuerySnapshot leaderboardQuerySs = await leaderboardQuery.GetSnapshotAsync();
        var leaderboardUsers = new List<LeaderboardUser>();
        foreach (DocumentSnapshot documentSnapshot in leaderboardQuerySs.Documents)
        {
            var leaderboardUser = documentSnapshot.ConvertTo<LeaderboardUser>();
            leaderboardUsers.Add(leaderboardUser);
        }

        return leaderboardUsers;
    }
    
    async void listGloabalLeaderboard()
    {
        List<LeaderboardUser> leaderboardUsers = await getGlobalLeaderboard();
        int position = 1;
        foreach (var leaderboardUser in leaderboardUsers)
        {
            GameObject obj = Instantiate(CellPrefab,gameObject.transform.GetChild(0));
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = position.ToString();
            obj.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = leaderboardUser.UserName;
            obj.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = leaderboardUser.Score.ToString();
            position++;
        }
    }

    private void OnEnable()
    {
        listGloabalLeaderboard();
    }
    
    private void OnDisable()
    {
        while (transform.GetChild(0).childCount > 0) {
            DestroyImmediate(transform.GetChild(0).GetChild(0).gameObject);
        }
    }
}