using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenShake/New Profile")]
public class ScreenShakeProfile : ScriptableObject
{
    [Header("Impulse Source Settings")]
    public float impulseTime = 0.2f;
    public float impulseForce = 0.5f;
    public Vector3 defaultVelocity = new Vector3(0f, -1f, 0f);
    public AnimationCurve impulseCurve;

    [Header("Impulse Listening Settings")]
    public float listenerAmplitude = 1f;
    public float listenerFrequency = 1f;
    public float listenerDuration = 1f;
}
