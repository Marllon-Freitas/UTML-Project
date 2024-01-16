using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Sword Skill Settings")]
    [SerializeField]
    private GameObject swordPrefab;

    [SerializeField]
    private Vector2 launchForce;

    [SerializeField]
    private float swordGravity;

    [SerializeField]
    private float freezeTimeDuration;

    [SerializeField]
    private float returnSpeed;

    [Header("Bounce Info")]
    [SerializeField]
    private int bounceAmount;

    [SerializeField]
    private float bounceGravity;

    [SerializeField]
    private float bounceSpeed;

    [Header("Pierce Info")]
    [SerializeField]
    private int pierceAmount;

    [SerializeField]
    private float pierceGravity;

    [Header("Spin Info")]
    [SerializeField]
    private float hitCooldown = 0.35f;

    [SerializeField]
    private float spinDuration = 2f;

    [SerializeField]
    private float maxTravelDistance = 7f;

    [SerializeField]
    private float spinGravity;

    private Vector2 finalDirection;

    [Header("Aim Dots")]
    [SerializeField]
    private GameObject dotPrefab;

    [SerializeField]
    private int numberOfDots;

    [SerializeField]
    private float spaceBetweenDots;

    [SerializeField]
    private Transform dotsParent;
    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        GenerateDots();

        SetupGravity();
    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.Mouse1))
            finalDirection = new Vector2(
                AimDirection().normalized.x * launchForce.x,
                AimDirection().normalized.y * launchForce.y
            );

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(
            swordPrefab,
            player.transform.position,
            transform.rotation
        );
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
            newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);

        newSwordScript.SetupSword(
            finalDirection,
            swordGravity,
            player,
            freezeTimeDuration,
            returnSpeed
        );

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim Region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    public void DotsActive(bool isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(
                dotPrefab,
                player.transform.position,
                Quaternion.identity,
                dotsParent
            );
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position =
            (Vector2)player.transform.position
            + new Vector2(
                AimDirection().normalized.x * launchForce.x,
                AimDirection().normalized.y * launchForce.y
            ) * t
            + (t * t) * 0.5f * (Physics2D.gravity * swordGravity);
        return position;
    }
    #endregion
}
