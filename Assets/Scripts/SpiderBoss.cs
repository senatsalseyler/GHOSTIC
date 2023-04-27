using System;
using UnityEngine;

public class SpiderBoss : MonoBehaviour
{
    public Animator animator;
    private BoxCollider2D collider;
    private const float DOUBLE_CLICK_TIME = .5f;
    private float lastClickTime;

    //    public Animation smashAnimation;
    //    bool isDestroyStarted = false;
    private void Start()
    {
        collider = gameObject.GetComponent<BoxCollider2D>();
    }

    void OnMouseDown()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
        {
            animator.Play("boss_spider_smash");
            Destroy(gameObject, 0.3f);
        }
        lastClickTime = Time.time;
    }
}