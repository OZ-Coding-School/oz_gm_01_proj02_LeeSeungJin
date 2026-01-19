public class RankingModel
{
    // 마지막 등수의 기록보다 높은지 비교
    public bool IsHighRecord()
    {
        int score = GameManager.Instance.GetScore();
        RankingData data = RankingSystem.Instance.GetRanking()[RankingSystem.Instance.GetMAXRank() - 1];
        if (score > data.score) return true;
        else return false;
    }

    // 랭킹리스트 정리
    public string GetRankingList()
    {
        string tmp = "Rank     Score       Round   Name\n";
        foreach (var r in RankingSystem.Instance.GetRanking())
        {
            tmp += $"{r.rank}        " + r.score.ToString("D8") + "      " + r.round.ToString("D2") + "        " + r.playerName+"\n";
        }
        return tmp;
    }
}
