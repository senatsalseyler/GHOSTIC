using System;

public class LeaderboardUser
{
    public LeaderboardUser(int score, string userId, DateTime timestamp)
    {
        this.user_id = userId;
        this.score = score;
        this.timestamp = timestamp;
    }

    public string user_id { get; set; }

    public int score { get; set; }

    public DateTime timestamp { get; set; }
}