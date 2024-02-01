using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    public string checkpointId;
    public bool isCheckpointActive;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    [ContextMenu("Generate Checkpoint Id")]
    private void GenerateCheckpointId()
    {
        checkpointId = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        isCheckpointActive = true;
        if (animator != null)
            animator.SetBool("Active", true);
    }
}
