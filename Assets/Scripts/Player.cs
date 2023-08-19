using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public float health;
    public Slider slider;
    public AudioSource spider_sound;
    public ScoreScriptNew scoreScript;
    public GameManager gameManager;
    public GameObject GameOverScreen;
    public Text lastScore;
    public Text highScore;
    private GameObject angel;

    void Start()
    {
            //PlayerPrefs.SetInt("HighScore", (int)0);
    }

    void Update()
    {
        slider.value = health;
    }
    

    void FixedUpdate()
    {
        /*if ((int)scoreScript.finalScore >= PlayerPrefs.GetInt("HighScore", 0))
        {
        }*/
    }

    public void IncreaseHealth(float health)
    {
        float tempHealth = this.health + health;
        if(tempHealth <= 0)
        {
            this.health = 0;
            gameManager.pauseGame();
            GameOverScreen.SetActive(true);
            if ((int)scoreScript.finalScore >= PlayerPrefs.GetInt("HighScore", 0))
            {
                PlayerPrefs.SetInt("HighScore", (int)scoreScript.finalScore);
                highScore.text = scoreScript.finalScore.ToString();
            }
            else
            {
                highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
            }
            if (scoreScript.finalScore >= 0)
            {
                lastScore.text = scoreScript.finalScore.ToString();
            }
            else
            {
                lastScore.text = "0";
            }
            scoreScript.WriteScore();
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
            //Cancelled feature.
            //scoreScript.IncreaseScore(-100);
            
            spider_sound.Play();
            
            Vector3 direction = (transform.position - collision.transform.position).normalized;
            gameObject.GetComponent<Rigidbody2D>().AddForce(direction * -100f );
        }
        else if (collision.gameObject.CompareTag("angel"))
        {
            IncreaseHealth(25f);
            Destroy(collision.gameObject, 0.1f);
        }

        else if (collision.gameObject.CompareTag("boss_spider"))
        {
            //Cancelled feature.
            //IncreaseHealth(-20f);
            scoreScript.IncreaseScore(-100);

            spider_sound.Play();

            Vector3 direction = (transform.position - collision.transform.position).normalized;
            gameObject.GetComponent<Rigidbody2D>().AddForce(direction * -100f);
        }
    }
}
