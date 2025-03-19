using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootingTarget : BaseTarget, IFormattable
{
    public bool isTargetPractice;
    public float health = 10f;
    public float defaultHealth;
    public int score = 0;
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI timeDisplay;

    private float timeRemaining = 10f;
    private bool timerRunning = false;
    private bool gameStarted = false;

    private List<int> scoreHistory = new List<int>();

    //Teisingai atlikote implementacija IFormattable - 1 t.
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return format switch
        {
            "H" => $"Health: {health}",
            "S" => $"Score: {score}",
            _ => $"Target: {health} HP, {score} points"
        };
    }

    private void Start()
    {
        defaultHealth = health;
        timeDisplay.text = "Time: 10s";
        scoreDisplay.text = "Score: 0";
    }

    private void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;
            timeDisplay.text = $"Time: {Mathf.Max(0, Mathf.Ceil(timeRemaining))}s";

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerRunning = false;
                //gameStarted = false;
                Debug.Log("Time is up! " + this.ToString("S", null));
                //PrintScoreHistory();
            }
        }

        scoreDisplay.text = $"Score: {score}";
    }

    


    public override void TakeDamage(float amount = 5f) // Naudojami numatyti ir vardiniai argumentai - 0.5 t
    {
        if (!gameStarted || !timerRunning) 
        { 
            return;
        }
        
        health -= amount;

        if (health <= 0)
        {


            scoreHistory.Add(score);
            if (scoreHistory.Count > 5)
            {
                scoreHistory = new List<int>(scoreHistory.GetRange(scoreHistory.Count - 5, 5)); //Naudojate 'Range' tipa 0.5 t
            }

            score += 10;

            Die();
        }
    }

    void Die()
    {
        if (isTargetPractice)
        {
            health = defaultHealth;
            transform.position = new Vector3(UnityEngine.Random.Range(27, 42), 2.5f, UnityEngine.Random.Range(12.5f, 92.5f));

        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("Target Broken");
    }

    public void StartGame()
    {
        if (!gameStarted)
        {
            //Debug.Log("Game Started!");
            gameStarted = true;
            timerRunning = true;
            timeRemaining = 10f;
            score = 0;
            scoreDisplay.text = "Score: 0";
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");

        if (score > 0)
        {
            ScoreMonitor monitor = FindObjectOfType<ScoreMonitor>();
            monitor?.UpdateScoreHistory(score);
        }

        gameStarted = false;
        timerRunning = false;
        timeRemaining = 10f;
        score = 0;

        timeDisplay.text = "Time: 10s";
        scoreDisplay.text = "Score: 0";
    }

    //Naudojamas dekonstruktorius - 0.5t
    public void Deconstruct(out float health, out bool isTargetPractice)
    {
        health = this.health;
        isTargetPractice = this.isTargetPractice;
    }

    public void PrintScoreHistory()
    {
        Debug.Log($"Last 5 Scores: {string.Join(", ", scoreHistory)}");
    }

}
