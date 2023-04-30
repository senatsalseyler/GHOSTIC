using System;
using Firebase.Firestore;

[FirestoreData]
public class LeaderboardUser
{
    
    [FirestoreProperty]
    public string UserId { get; set; }

    [FirestoreProperty]
    public int Score { get; set; }

    [FirestoreProperty]
    public DateTime TimeStamp { get; set; }
}