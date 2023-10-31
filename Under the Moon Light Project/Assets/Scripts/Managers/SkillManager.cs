using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    //Dash Skill
    public Dash_Skill dashSkill { get; private set; }

    //Clone Skill
    public Clone_Skill cloneSkill { get; private set; }

    //Throw Sword Skill
    public Sword_Skill swordThrowSkill { get; private set; }

    //Black Hole Skill
    public BlackHole_Skill blackHoleSkill { get; private set; }

    //Crystal Skill
    public Crystal_Skill crystalSkill { get; private set; }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        //Dash Skill
        dashSkill = GetComponent<Dash_Skill>();
        //Clone Skill
        cloneSkill = GetComponent<Clone_Skill>();
        //Throw Sword Skill
        swordThrowSkill = GetComponent<Sword_Skill>();
        //Black Hole Skill
        blackHoleSkill = GetComponent<BlackHole_Skill>();
        //Crystal Skill
        crystalSkill = GetComponent<Crystal_Skill>();
    }
}
