﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    Vector3 originalPos;

    [Range(0, 5)]
    public float ShakeDuration = 1;
    float shakeCountDown = 0;

    [Range(0,5)]
    public float shakeAmount = 0.7f;


    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(shakeCountDown <= 0)
        {
            shakeCountDown = 0;
            transform.localPosition = originalPos;
        }
        else if (Time.timeScale == 1.0f)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeCountDown -= Time.deltaTime;
        }

    }

    public void enableCamShake()
    {
        originalPos = transform.localPosition;
        shakeCountDown = ShakeDuration;
    }
}
