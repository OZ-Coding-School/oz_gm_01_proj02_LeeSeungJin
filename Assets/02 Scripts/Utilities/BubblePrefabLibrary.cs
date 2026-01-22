using UnityEngine;

public class BubblePrefabLibrary : Singleton<BubblePrefabLibrary>
{
    [System.Serializable]
    public struct BubbleEntry
    {
        public Bubble.BubbleColor color;  // 색상 enum
        public GameObject prefab;  // 해당 색상 프리팹
    }

    [SerializeField] private BubbleEntry[] entries;

    public GameObject GetPrefab(Bubble.BubbleColor color)
    {
        foreach (var entry in entries)
        {
            if (entry.color == color)
                return entry.prefab;
        }
        return null;
    }
}