using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackHole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private KeyCode myHotkey;
    private TextMeshProUGUI myText;

    private Transform enemiesTransform;
    private BlackHole_Skill_Controller blackHole;

    public void SetupHotKey(KeyCode myNewHotKey, Transform myEnemy, BlackHole_Skill_Controller myBlackHole)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        enemiesTransform = myEnemy;
        blackHole = myBlackHole;

        myHotkey = myNewHotKey;
        myText.text = myNewHotKey.ToString();

    }

    private void Update()
    {
        if (Input.GetKeyDown(myHotkey))
        {
            blackHole.AddEnemyToList(enemiesTransform);
            myText.color = Color.clear;
            spriteRenderer.color = Color.clear;
        }
    }
}
