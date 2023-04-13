using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneHelper : MonoBehaviour
{
    [SerializeField] private bool useCurrentScene;

    [DisableIf("useCurrentScene")]
    [SerializeField] private int sceneIndex;

    public void LoadScene()
    {
        if (useCurrentScene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
