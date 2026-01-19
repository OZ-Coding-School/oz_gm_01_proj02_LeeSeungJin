using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;

[System.Serializable]
public class RankingWrapper
{
    public List<RankingData> rankings = new List<RankingData>();

    public List<RankingData> GetDefault()
    {
        int tmp = RankingSystem.Instance.GetMAXRank();
        for (int i = 0; i < tmp; i++)
        {
            rankings.Add(new RankingData { rank = i+1, playerName = "AAA", round = 0, score = 0 });
        }
        return rankings;
    }
}

