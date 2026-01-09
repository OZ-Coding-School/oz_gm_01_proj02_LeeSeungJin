using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bubble"))
        {
            Debug.Log("버블이 바닥에 닿음 → Game Over!");
            // 게임 오버 처리 필요
        }
    }
}
