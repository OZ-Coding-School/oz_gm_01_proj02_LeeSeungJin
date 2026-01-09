using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float minAngle = -60f;
    [SerializeField] private float maxAngle = 60f;
    [SerializeField] private float bubbleSpeed = 10f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] bubblePrefab;
    [SerializeField] private Image previewImage;
    [SerializeField] private BubbleTrajectory trajectory;

    private GameObject currentBubblePrefab;

    private void Start()
    {
        PrepareCurrentBubble();
    }

    private void Update()
    {
        HandleInput();
    }
    private void FixedUpdate()
    {
        // 발사 궤적은 물리 주기에서 갱신
        trajectory.ShowTrajectory(firePoint.position, transform.up, bubbleSpeed);
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward * -horizontal * rotationSpeed * Time.deltaTime);

        float z = transform.eulerAngles.z;
        if (z > 180) z -= 360;

        z = Mathf.Clamp(z, minAngle, maxAngle);  // 각도 제한

        transform.eulerAngles = new Vector3(0, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireBubble();
        }
    }
    private void FireBubble()
    {
        Bubble bubble = Bubble.CreateFromPool(currentBubblePrefab, firePoint.position, Quaternion.identity);
        bubble.Fire(transform.up, bubbleSpeed);

        PrepareCurrentBubble();
    }
    private void PrepareCurrentBubble()
    {
        currentBubblePrefab = bubblePrefab[Random.Range(0, bubblePrefab.Length)];

        // UI Image에 Sprite만 교체
        SpriteRenderer sr = currentBubblePrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            previewImage.sprite = sr.sprite;
        }
    }
}
