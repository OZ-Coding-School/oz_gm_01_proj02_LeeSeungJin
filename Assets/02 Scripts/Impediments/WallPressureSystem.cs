using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPressureSystem : MonoBehaviour
{
    [SerializeField] private Transform ceiling;
    [SerializeField] private float moveDistance = 0.5f; // 내려오는 거리
    [SerializeField] private int shotsPerPressure = 7;   // x번 발사마다 압박
    [SerializeField] private float shotTimeLimit = 10f;  // 발사 제한시간

    private int shotCount = 0;
    private float shotTimer = 0f;
    private bool waitingForShot = true;

    private void Update()
    {
        if (waitingForShot)
        {
            shotTimer += Time.deltaTime;
            if (shotTimer >= shotTimeLimit)
            {
                ApplyPressure(); // 시간 초과 시 압박
                ResetShotTimer();
            }
        }
    }

    public void OnPlayerShot()
    {
        shotCount++;
        ApplyShotPressureCheck();
        ResetShotTimer();
        Debug.Log($"count : {shotCount}  timer : {shotTimer}");
    }

    private void ApplyShotPressureCheck()
    {
        if (shotCount >= shotsPerPressure)
        {
            ApplyPressure();
            shotCount = 0;
        }
    }

    private void ApplyPressure()
    {
        ceiling.position += Vector3.down * moveDistance;
        BubbleManager.Instance.ApplyPressure();

        Debug.Log("벽 압박 발생!");
    }


    private void ResetShotTimer()
    {
        shotTimer = 0f;
        waitingForShot = true;
    }

}

