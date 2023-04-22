using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionController : MonoBehaviour
{
    public Animator animator;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("web"))
        {
            animator.SetBool("fire0", true);
            ScoreScript.scoreValue -= 75;

        }
        else if(collision.gameObject.CompareTag("mirror"))
        {
            animator.SetBool("reverse", true);
        }
        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("web"))
        {
            animator.SetBool("fire0", false);

        }
        else if(collision.gameObject.CompareTag("mirror"))
        {
            animator.SetBool("reverse", false);
            ScoreScript.scoreValue += 10;
        }
    }

}
