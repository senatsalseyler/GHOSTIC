using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public float health;
    public Slider slider;
    public AudioSource spider_sound;

    void Update()
    {
        slider.value = health;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("spider"))
        {
            health = health - 10f;
            ScoreScript.scoreValue -= 10;
            spider_sound.Play();
        }
        else if (collision.gameObject.CompareTag("angel"))
        {
            health = health + 25f;
        }
    }
}
