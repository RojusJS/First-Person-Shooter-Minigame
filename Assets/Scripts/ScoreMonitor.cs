using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreMonitor : MonoBehaviour
{
    public TextMeshPro scoreText3D;
    private List<int> scoreHistory = new List<int>();

    private void Start()
    {
        UpdateMonitor();
    }

    public void UpdateScoreHistory(int finalScore)
    {
        scoreHistory.Add(finalScore);

        if (scoreHistory.Count > 5)
        {
            scoreHistory = new List<int>(scoreHistory.GetRange(scoreHistory.Count - 5, 5));
        }

        UpdateMonitor();
    }

    private void UpdateMonitor()
    {
        scoreText3D.text = "Last Scores:\n";

        if (scoreHistory.Count == 0)
        {
            scoreText3D.text += "No scores yet!\n";
            scoreText3D.text += "Best Score: 0";
        }
        else
        {
            int bestScore = 0;

            foreach (int score in scoreHistory)
            {
                scoreText3D.text += score + "\n";
                if (score > bestScore)
                {
                    bestScore = score; 
                }
            }

            scoreText3D.text += $"\nBest Score: {bestScore}";
        }
    }
}
