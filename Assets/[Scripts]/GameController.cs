using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int scorePenalty = 5;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject starsPanel;
    [SerializeField] private int minimumForThreeStars = 75;
    [SerializeField] private int minimumForTwoStars = 50;
    [SerializeField] private int minimumForOneStars = 25;

    private int score;
    private int stars;

    // Start is called before the first frame update
    void Start()
    {
        //instance= this;
        score = 100;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        CarState();

        if(score == 0)
        {
            GameOver();
        }
    }

    // hazardType is a gameobject tag provided by HazardController script (Must add tags to hazard objects)
    public void HazardHit(string hazardType)
    {
        switch(hazardType)
        {
            case "TrafficCone":
                Debug.Log("Traffic Cone Hit!");
                score -= scorePenalty;
                break;
        }
    }

    public void CarState()
    {
        if (CarController.instance.mode == CarController.Mode.DRIVE && CarController.instance.lightMode == CarController.DirectionalLightMode.RIGHT_ON)
        {
            score -= scorePenalty;
        }
        if (CarController.instance.mode == CarController.Mode.REVERSE && CarController.instance.lightMode == CarController.DirectionalLightMode.LEFT_ON)
        {
            score -= scorePenalty;
        }
        if (!CarController.instance.seatBeltTightened && CarController.instance.mode == CarController.Mode.DRIVE)
        {
            score -= scorePenalty;
        }
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();

        if(score >= minimumForThreeStars) 
        { 
            stars = 3; 
            starsPanel.transform.GetChild(0).gameObject.SetActive(true);
            starsPanel.transform.GetChild(1).gameObject.SetActive(true);
            starsPanel.transform.GetChild(2).gameObject.SetActive(true);
        }
        else if(score >= minimumForTwoStars) 
        { 
            stars = 2;
            starsPanel.transform.GetChild(0).gameObject.SetActive(true);
            starsPanel.transform.GetChild(1).gameObject.SetActive(true);
            starsPanel.transform.GetChild(2).gameObject.SetActive(false);
        }
        else if(score >= minimumForOneStars) 
        { 
            stars = 1;
            starsPanel.transform.GetChild(0).gameObject.SetActive(true);
            starsPanel.transform.GetChild(1).gameObject.SetActive(false);
            starsPanel.transform.GetChild(2).gameObject.SetActive(false);
        }
        else 
        { 
            stars = 0;
            starsPanel.transform.GetChild(0).gameObject.SetActive(false);
            starsPanel.transform.GetChild(1).gameObject.SetActive(false);
            starsPanel.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
