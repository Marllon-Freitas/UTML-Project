using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider_UI : MonoBehaviour
{
    public Slider slider;
    public string parametr;

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private float multiplayer;

    public void LoadSlider(float _value)
    {
        if (_value >= 0.001f)
            slider.value = _value;
    }

    public void SliderValue(float _value)
    {
        audioMixer.SetFloat(parametr, Mathf.Log10(_value) * multiplayer);
    }
}
