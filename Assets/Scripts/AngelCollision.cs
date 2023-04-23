using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelCollision : MonoBehaviour
{
    public Animator animator;
    public AudioSource angel_sound;
    public ScoreScript scoreScript;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ghost"))
        {
            animator.SetBool("glow", true);
            angel_sound.Play();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ghost"))
        {
            animator.SetBool("glow", false);
            scoreScript.IncreaseScore(100);
            
        }
    }
}
