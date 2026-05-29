using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Ball : MonoBehaviour
{
    public float minY = -5.5f;
    public float maxVelocity = 15f;

    Rigidbody2D rb;
    int score = 0;
    int lives = 5;

    public TextMeshProUGUI scoreTxt;
    public GameObject[] livesImage;

    public GameObject gameOverPanel;
    public GameObject youWonPanel;
    int brickCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
       brickCount = FindAnyObjectByType<LevelGenerator>().transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < minY)
        {
            if(lives <= 0)
            {
                GameOver();
            }
            else
            {
                transform.position = Vector3.zero;
                rb.linearVelocity = Vector3.zero;
                lives--;
                livesImage[lives].SetActive(false);
            }
        }

        if(rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxVelocity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
            score += 10;
            scoreTxt.text = score.ToString("00000");
            brickCount--;
            if(brickCount <= 0)
            {
                youWonPanel.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        Destroy(gameObject);
    }
}
