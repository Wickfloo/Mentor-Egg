using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneResetter : MonoBehaviour
{
    private void Update()
    {
        // Press 'U' to restart the current active scene
        if (Input.GetKeyDown(KeyCode.U))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}