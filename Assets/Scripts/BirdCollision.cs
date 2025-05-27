using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCollision : MonoBehaviour
{
    public Animator animator;
    public AudioSource bird_sound;
//    public ScoreScriptNew scoreScript;

    //A boolean for denifying have we ever interacted with cage.
    private bool isBirdFree;

    private void Start()
    {
        isBirdFree = false;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ghost") && !isBirdFree)
        {
            isBirdFree = true;
            animator.SetBool("bird", true);
//            scoreScript.IncreaseScore(125);
            bird_sound.Play();
        }
    }
}
