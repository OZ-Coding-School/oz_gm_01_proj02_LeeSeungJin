using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    public GameManager Game { get; private set; }
    public UIManager UI { get; private set; }
    public BubbleManager Bubble { get; private set; }
    public AudioManager Audio { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitManagers()
    {
        Game = new GameManager();
        UI = new UIManager();
        Bubble = new BubbleManager();
        Audio = new AudioManager();

        Game.Init();
        UI.Init();
        Bubble.Init();
        Audio.Init();
    }
}
