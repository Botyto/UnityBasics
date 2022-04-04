using UnityEngine;

[CreateAssetMenu(fileName = "NewDreamloLeaderboard", menuName = "ScriptableObject/Dreamlo Leaderboard")]
public class DreamloLeaderboard : ScriptableObject
{
    public enum ESortOrder
    {
        HighestScore,
        LowestScore,
        HighestTime,
        LowestTime,
        MostRecent,
        Oldest,
    }

    [Range(1, 25)]
    public int ScoresCount = 25;
    public ESortOrder SortOrder;
    public string PrivateCode;
    public string PublicCode;
    public bool UseSSL = false;
}
