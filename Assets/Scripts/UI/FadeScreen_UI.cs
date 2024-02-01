using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen_UI : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FadeIn() => animator.SetTrigger("FadeIn");

    public void FadeOut() => animator.SetTrigger("FadeOut");
}
