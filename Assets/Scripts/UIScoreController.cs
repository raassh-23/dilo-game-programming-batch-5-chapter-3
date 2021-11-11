using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreController : MonoBehaviour
{
    [Header("UI")]
    public Text currentScoreText;
    public Text highScoreText;

    [Header("Score")]
    public ScoreController scoreController;

    // Update is called once per frame
    void Update()
    {
        currentScoreText.text = scoreController.GetCurrentScore().ToString();
        highScoreText.text = ScoreData.highScore.ToString();
    }
}
