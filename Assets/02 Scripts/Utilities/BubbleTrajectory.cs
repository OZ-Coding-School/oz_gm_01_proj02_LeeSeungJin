using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BubbleTrajectory : MonoBehaviour
{
    [SerializeField] private float stepSize = 0.3f;          // 시뮬레이션 이동 단위
    [SerializeField] private int maxSteps = 100;             // 최대 시뮬레이션 스텝 수
    [SerializeField] private int maxReflectionCount = 3;     // 최대 반사 횟수
    [SerializeField] private LayerMask collisionMask;        // 벽 + 버블 + 천장 포함

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    public void ShowTrajectory(Vector2 origin, Vector2 direction, float speed)
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, origin);

        Vector2 pos = origin;
        Vector2 vel = direction.normalized * speed;
        int reflectionCount = 0;

        for (int i = 0; i < maxSteps; i++)
        {
            Vector2 nextPos = pos + vel.normalized * stepSize;

            // 충돌 체크
            RaycastHit2D hit = Physics2D.Linecast(pos, nextPos, collisionMask);

            if (hit.collider != null)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                if (hit.collider.CompareTag("EdgeWall") && reflectionCount < maxReflectionCount)
                {
                    // 벽 반사 처리
                    Vector2 normal = hit.normal;

                    // 수평/수직 벽일 경우 스냅 보정
                    if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
                        normal = new Vector2(Mathf.Sign(normal.x), 0);
                    else
                        normal = new Vector2(0, Mathf.Sign(normal.y));

                    vel = Vector2.Reflect(vel, normal);
                    pos = hit.point + vel.normalized * 0.01f; // 살짝 앞으로 이동
                    reflectionCount++;
                    continue; // 시뮬레이션 계속
                }
                else
                {
                    // 버블이나 천장에 닿으면 궤적 종료
                    break;
                }
            }
            else
            {
                // 충돌 없으면 계속 직진
                pos = nextPos;
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
            }
        }
    }

    public void HideTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
}