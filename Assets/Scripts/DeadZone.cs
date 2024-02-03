using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterStats>() != null)
            collision.GetComponent<CharacterStats>().KillEntity();
        else
            Destroy(collision.gameObject);
    }
}
