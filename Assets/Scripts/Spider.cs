using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public GameObject spider ;

    void Update()
    {
        spider = GameObject.FindGameObjectWithTag("spider");
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(spider);
        }
    }
}
