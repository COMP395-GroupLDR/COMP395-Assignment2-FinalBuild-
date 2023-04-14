using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrafficConeController : MonoBehaviour
{
    //private GameController gameController;
    private GameController gameController;
    private UnityEngine.SceneManagement.Scene scene;

    // Start is called before the first frame update
    void Start()
    {
        //gameController = GameController._instance;
        scene = SceneManager.GetActiveScene();
        if (scene.name != "MainMenu" && scene.name != "GameOver")
        {
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (scene.name != "MainMenu" && scene.name != "GameOver")
        {
            if (other.gameObject.CompareTag("Player"))
            {
                gameController.TrafficConeHit();
            }
        }
    }
}
