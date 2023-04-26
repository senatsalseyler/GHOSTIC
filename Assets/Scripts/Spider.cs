using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public AudioSource hit;
    public Animator animator;
    public BoxCollider2D collider;
    private float lastClickTime;

    //    public Animation smashAnimation;
    //    bool isDestroyStarted = false;
    private void Start()
    {
        collider = gameObject.GetComponent<BoxCollider2D>();
    }

    
    void OnMouseDown()
    {
        hit.Play();
        animator.Play("smash");
        collider.enabled = false;
        Destroy(gameObject, 0.3f);
    }


    //        if(!smashAnimation.isPlaying && isDestroyStarted)
    //        {
    //            Destroy(spider);
    //        }
    //        if (Input.GetMouseButtonDown(1) && !isDestroyStarted)
    //        {
    //            smashAnimation.Play();
    //            isDestroyStarted = true;
    //        }
}