using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("PopUpText")]
    [SerializeField]
    private GameObject popUpTextPrefab;

    [Header("FX")]
    [SerializeField]
    private float flashDuration;

    [SerializeField]
    private Material hitMaterial;
    private Material originalMaterial;

    [Header("Effect colors")]
    [SerializeField]
    private Color[] frozenColor;

    [SerializeField]
    private Color[] igniteColor;

    [SerializeField]
    private Color[] shockColor;

    [Header("FX")]
    [SerializeField]
    private GameObject hitFx;

    [SerializeField]
    private GameObject criticalHitFx;

    [Space]
    [SerializeField]
    private ParticleSystem swordCatchFx;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public IEnumerator FlashFX()
    {
        spriteRenderer.material = hitMaterial;
        Color currentColor = spriteRenderer.color;

        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = currentColor;
        spriteRenderer.material = originalMaterial;
    }

    private void RedCollorBlink()
    {
        if (spriteRenderer.color != Color.white)
            spriteRenderer.color = Color.white;
        else
            spriteRenderer.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        spriteRenderer.color = Color.white;
    }

    public void IgniteFxFor(float _seconds)
    {
        InvokeRepeating("IgniteColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void FrozenFxFor(float _seconds)
    {
        InvokeRepeating("FrozenColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFxFor(float _seconds)
    {
        InvokeRepeating("ShockColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void IgniteColorFx()
    {
        if (spriteRenderer.color != igniteColor[0])
            spriteRenderer.color = igniteColor[0];
        else
            spriteRenderer.color = igniteColor[1];
    }

    private void ShockColorFx()
    {
        if (spriteRenderer.color != shockColor[0])
            spriteRenderer.color = shockColor[0];
        else
            spriteRenderer.color = shockColor[1];
    }

    private void FrozenColorFx()
    {
        if (spriteRenderer.color != frozenColor[0])
            spriteRenderer.color = frozenColor[0];
        else
            spriteRenderer.color = frozenColor[1];
    }

    public void MakeTransparent(bool transparent)
    {
        if (transparent)
            spriteRenderer.color = Color.clear;
        else
            spriteRenderer.color = Color.white;
    }

    public void CreateHitFx(Transform _target, bool _isCriticalHit = false)
    {
        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-0.5f, 0.5f);
        float yPosition = Random.Range(-0.5f, 0.5f);

        Vector3 hitFxRotation = new Vector3(0, 0, zRotation);

        GameObject hitPrefab = hitFx;

        if (_isCriticalHit)
        {
            hitPrefab = criticalHitFx;
            float yRotation = 0;
            zRotation = Random.Range(-45, 45);
            if (GetComponent<Entity>().facingDirection == -1)
                yRotation = 180;

            hitFxRotation = new Vector3(0, yRotation, zRotation);
        }
        GameObject newHitFx = Instantiate(
            hitPrefab,
            _target.position + new Vector3(xPosition, yPosition),
            Quaternion.identity
        );

        newHitFx.transform.Rotate(hitFxRotation);

        Destroy(newHitFx, 0.4f);
    }

    public void PlaySwordCatchFx()
    {
        if (swordCatchFx != null)
            swordCatchFx.Play();
    }

    public void CreatePopUpText(string _text)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(1, 5);

        Vector3 randomPosition = new Vector3(randomX, randomY, 0);

        GameObject newPopUpText = Instantiate(
            popUpTextPrefab,
            transform.position + randomPosition,
            Quaternion.identity
        );

        newPopUpText.GetComponent<TextMeshPro>().text = _text;
    }
}
