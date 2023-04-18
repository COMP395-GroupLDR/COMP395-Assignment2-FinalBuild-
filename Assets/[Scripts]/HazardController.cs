using UnityEngine;
using UnityEngine.SceneManagement;

public class HazardController : MonoBehaviour
{
    //private GameController gameController;
    private GameController gameController;
    private Scene scene;

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
                gameController.HazardHit(gameObject.tag);
            }
        }
    }
}
