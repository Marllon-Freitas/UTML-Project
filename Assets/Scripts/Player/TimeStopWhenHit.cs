using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopWhenHit : MonoBehaviour
{
    [Header("Time Stop")]
    [SerializeField] private float speed;
    [SerializeField] private bool restoreTime;

    // Start is called before the first frame update
    void Start()
    {
        restoreTime = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1f)
                Time.timeScale += speed * Time.deltaTime;
            else
            {
                Time.timeScale = 1f;
                restoreTime = false;
            }
        }
    }

    public void StopTime(float changeTime, int restoreSpeed, float delay)
    {
        speed = restoreSpeed;

        if (delay > 0)
        {
            StopCoroutine(StartTimeAgain(delay));
            StartCoroutine(StartTimeAgain(delay));
        }

        Time.timeScale = changeTime;
    }

    private IEnumerator StartTimeAgain(float delay)
    {
        restoreTime = true;
        yield return new WaitForSecondsRealtime(delay);
    }
}
