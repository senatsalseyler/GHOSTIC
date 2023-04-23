using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCollision : MonoBehaviour
{
    public Animator animator;
    public AudioSource bird_sound;
    public ScoreScript scoreScript;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ghost"))
        {
            animator.SetBool("bird", true);
            scoreScript.IncreaseScore(125);
            bird_sound.Play();
        }
    }
}
