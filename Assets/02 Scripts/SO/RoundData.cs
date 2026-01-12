using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Round Data")]
public class RoundData : ScriptableObject
{
    public Vector2Int[] positions;
    public Bubble.BubbleColor[] colors;
}
