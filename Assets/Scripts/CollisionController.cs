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
            Debug.Log("web");
            animator.SetBool("fire0", true);
        }
        else if(collision.gameObject.CompareTag("angel"))
        {
            Debug.Log("angel");
            animator.SetBool("reverse", true);
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("web"))
        {
            animator.SetBool("fire0", false);
        }
        else if(collision.gameObject.CompareTag("angel"))
        {
            animator.SetBool("reverse", false);
        }
    }

}
