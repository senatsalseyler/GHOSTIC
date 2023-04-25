using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public GameObject spider ;
    public AudioSource hit;
//    public Animation smashAnimation;
//    bool isDestroyStarted = false;
    void Update()
    {
        spider = GameObject.FindGameObjectWithTag("spider");

        if (Input.GetMouseButtonDown(1))
        {
            hit.Play();
            Destroy(spider, 0.2f);
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
}
