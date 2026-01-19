using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class NameInputAssistant : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] letterSlots;
    private char[] letters = new char[3] { 'A', 'A', 'A' };
    private int currentIndex = 0;
    private void Start()
    {
        UpdateUI();
    }
    private void Update()
    {
        // 좌우 이동
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = (currentIndex + 2) % 3; 
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = (currentIndex + 1) % 3; 
            UpdateUI();
        }

        // 위아래로 알파벳 변경
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            letters[currentIndex]++;
            if (letters[currentIndex] > 'Z') letters[currentIndex] = 'A';
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            letters[currentIndex]--;
            if (letters[currentIndex] < 'A') letters[currentIndex] = 'Z';
            UpdateUI();
        }

        // Enter로 확정
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string playerName = new string(letters);
            // 랭킹 등록 호출
            FindObjectOfType<RankingSystem>().TryAddScore(playerName, RoundManager.Instance.CurrentRound, GameManager.Instance.GetScore());
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < 3; i++)
        {
            letterSlots[i].text = letters[i].ToString();
            letterSlots[i].color = (i == currentIndex) ? Color.yellow : Color.white;
        }
    }
}