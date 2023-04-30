using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;
using Firebase;
using Firebase.Extensions;


public class PersonalLeaderboard : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CellPrefab;

    async Task<List<LeaderboardUser>> getPersonalLeaderboard()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        string uid = "";
        if (user != null) {
            uid = user.UserId;
        }
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        CollectionReference colRef = db.Collection("leaderboard");
        Query leaderboardQuery = colRef.WhereEqualTo("UserId",uid).OrderByDescending("Score");
        QuerySnapshot leaderboardQuerySs = await leaderboardQuery.GetSnapshotAsync();
        var leaderboardUsers = new List<LeaderboardUser>();
        foreach (DocumentSnapshot documentSnapshot in leaderboardQuerySs.Documents)
        {
            var leaderboardUser = documentSnapshot.ConvertTo<LeaderboardUser>();
            leaderboardUsers.Append(leaderboardUser);
        }


        return leaderboardUsers;
    }

    async void listPersonalLeaderboard()
    {
        List<LeaderboardUser> leaderboardUsers = await getPersonalLeaderboard();
        int position = 1;
        foreach (var leaderboardUser in leaderboardUsers)
        {
            GameObject obj = Instantiate(CellPrefab);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.SetParent(this.gameObject.transform.GetChild(0));
            obj.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = position.ToString();
            obj.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = leaderboardUser.UserId;
            obj.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = leaderboardUser.Score.ToString();
            position++;
        }
    } 
    
    private void OnEnable()
    {
        listPersonalLeaderboard();
    }
    
    private void OnDisable()
    {
        while (transform.GetChild(0).childCount > 0) {
            DestroyImmediate(transform.GetChild(0).GetChild(0).gameObject);
        }
    }
}