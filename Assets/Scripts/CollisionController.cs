using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionController : MonoBehaviour
{
    public Animator animator;
    public AudioSource web_sound;
    public AudioSource mirror_sound;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("web"))
        {
            animator.SetBool("fire0", true);
            web_sound.Play();

        }
        else if(collision.gameObject.CompareTag("mirror"))
        {
            animator.SetBool("reverse", true);
            mirror_sound.Play();
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
