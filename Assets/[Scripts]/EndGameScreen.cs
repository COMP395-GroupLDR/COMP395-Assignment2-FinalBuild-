using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameScreen : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public GameController gameController;
    public void Setup()
    {
        int score = gameController.score;
        gameObject.SetActive(true);
        pointsText.text = score.ToString() + " POINTS";
        Time.timeScale = 0;
    }
    public void RestarButton()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitButton()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
