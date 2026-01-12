using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPressureSystem : MonoBehaviour
{
    [SerializeField] private Transform ceiling;
    [SerializeField] private float moveDistance = 0.5f; // 내려오는 거리
    [SerializeField] private int attachPerPressure = 7; // x번 버블 붙으면 압박
    [SerializeField] private float attachTimeLimit = 10f;  // 발사 제한시간

    private int attachCount = 0;
    private float attachTimer = 0f;
    private bool waitingForAttach = true;

    private void Update()
    {
        if (waitingForAttach)
        {
            attachTimer += Time.deltaTime;
            if (attachTimer >= attachTimeLimit)
            {
                ApplyPressure();
                ResetAttachTimer();
            }
        }
    }

    public void OnBubbleAttached()
    {
        attachCount++;
        ApplyAttachPressureCheck();
        ResetAttachTimer();
    }

    private void ApplyAttachPressureCheck()
    {
        if (attachCount >= attachPerPressure)
        {
            ApplyPressure();
            attachCount = 0;
        }
    }
    private void ApplyPressure()
    {
        ceiling.position += Vector3.down * moveDistance;
        BubbleManager.Instance.ApplyPressure();

        Debug.Log("벽 압박 발생!");
    }
    private void ResetAttachTimer()
    {
        attachTimer = 0f;
        waitingForAttach = true;
    }

}

