using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Scripts.Conversation;

public class MinuteHandMovement : MonoBehaviour
{
    Conversation conversation;
    public float durationMinutes;

    private float rotationSpeed;
    private float durationSeconds;
    private float elapsedTime = 0f;

    public Transform pivotPoint;

    private bool start = false;

    private void Start()
    {
        durationSeconds = durationMinutes * 60;
        rotationSpeed = 360f / durationSeconds;
    }

    private void Update()
    {
        if (start && elapsedTime < durationSeconds)
        {
            transform.RotateAround(pivotPoint.position, Vector3.left, rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
        }
    }

    public void StartTime()
    {
       start = true;

    }


}