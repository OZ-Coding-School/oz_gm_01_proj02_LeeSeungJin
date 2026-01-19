using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RankingSystem : Singleton<RankingSystem>
{
    protected override bool IsDDOL => false;

    private List<RankingData> rankingList = new List<RankingData>();
    private const int MAX_RANK = 5;

    protected override void Awake()
    {
        base.Awake();
        LoadRanking();
    }

    // 점수와 이름을 랭킹에 추가 시도
    public void TryAddScore(string name, int round, int score)
    {
        // 새로운 기록 추가
        rankingList.Add(new RankingData { rank = 0, playerName = name, round = round, score = score });

        // 점수 높은 순으로 정렬 후 상위 5개만 유지
        rankingList = rankingList.OrderByDescending(r => r.score)
                                 .Take(MAX_RANK)
                                 .ToList();
        // 랭크 매기기
        foreach( var rl in rankingList)
        {
            int setRank = 1;
            rl.rank = setRank;
            setRank++;
        }

        SaveRanking();
    }

    // 랭킹 저장
    private void SaveRanking()
    {
        RankingWrapper wrapper = new RankingWrapper { rankings = rankingList };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("RankingData", json);
        PlayerPrefs.Save();
    }

    // 랭킹 불러오기
    private void LoadRanking()
    {
        string json = PlayerPrefs.GetString("RankingData", "");
        if (!string.IsNullOrEmpty(json))
        {
            RankingWrapper wrapper = JsonUtility.FromJson<RankingWrapper>(json);
            rankingList = wrapper.rankings;
        }
    }

    // 현재 랭킹 리스트 반환
    public List<RankingData> GetRanking()
    {
        return rankingList;
    }
}