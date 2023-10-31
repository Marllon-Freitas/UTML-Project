using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_UI : MonoBehaviour
{
    private CharacterStats myStats;

    private Entity entity;
    private RectTransform myRectTransform;
    private Slider healthBarSlider;

    private void Start()
    {
        myRectTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        healthBarSlider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();

        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthBarSlider.maxValue = myStats.GetMaxHealthValue();
        healthBarSlider.value = myStats.currentHealth;
    }

    private void FlipUI() => myRectTransform.Rotate(0, 180, 0);
    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        myStats.onHealthChanged -= UpdateHealthUI;
    }
}
