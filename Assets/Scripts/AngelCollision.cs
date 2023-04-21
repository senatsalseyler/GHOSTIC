using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelCollision : MonoBehaviour
{
    public Animator animator;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ghost"))
        {
            Debug.Log("ghost");
            animator.SetBool("glow", true);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ghost"))
        {
            animator.SetBool("glow", false);
        }
    }
}
