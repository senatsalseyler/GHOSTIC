using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public GameObject spider;
    public AudioSource hit;
    public GameObject boss_spider;
    private const float DOUBLE_CLICK_TIME = .5f;
    private float lastClickTime;

    //    public Animation smashAnimation;
    //    bool isDestroyStarted = false;
    void OnMouseDown()
    {
        spider = GameObject.FindGameObjectWithTag("spider");
        boss_spider = GameObject.FindGameObjectWithTag("boss_spider");

        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
        {
            Destroy(boss_spider, 0.15f);
        }

        else
        {
            hit.Play();
            Destroy(spider, 0.1f);
        }

        lastClickTime = Time.time;
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