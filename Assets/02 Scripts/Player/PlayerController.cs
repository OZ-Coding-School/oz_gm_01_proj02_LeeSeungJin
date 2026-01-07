using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float minAngle = -60f;
    [SerializeField] private float maxAngle = 60f;
    [SerializeField] private float bubbleSpeed = 10f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bubblePrefab;

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward * -horizontal * rotationSpeed * Time.deltaTime);

        float z = transform.eulerAngles.z;
        if (z > 180) z -= 360;

        z = Mathf.Clamp(z, minAngle, maxAngle);

        transform.eulerAngles = new Vector3(0, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireBubble();
        }
    }

    private void FireBubble()
    {
        GameObject bubble = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();

        Vector2 dir = transform.up;

        rb.velocity = dir.normalized * bubbleSpeed;
    }

}
