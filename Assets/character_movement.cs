using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character_movement : MonoBehaviour
{
    public GameObject player;
    public CharacterController controller;
    public float speed;


    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        player.transform.position = new Vector2(x * speed , y * speed);


        controller.Move(player.transform.position * speed * Time.deltaTime);
    }
}
