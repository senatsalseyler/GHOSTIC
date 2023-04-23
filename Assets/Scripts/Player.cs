using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public float health;
    public Slider slider;
    public AudioSource spider_sound;
    public ScoreScript scoreScript;

    void Update()
    {
        slider.value = health;
    }

    public void IncreaseHealth(float health)
    {
        float tempHealth = this.health + health;
        if(tempHealth <= 0)
        {
            this.health = 0;
            //death
        }
        else if(tempHealth >= 100)
        {
            this.health = 100;
        }
        else
        {
            this.health = tempHealth;
        }
    } 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("spider"))
        {
            IncreaseHealth(-10f);
            scoreScript.IncreaseScore(-100);
            spider_sound.Play();
        }
        else if (collision.gameObject.CompareTag("angel"))
        {
            IncreaseHealth(25f);
        }
    }
}
