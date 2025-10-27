using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Method to load the next scene
    public void PlayGame()
    {
        // Assuming the next scene is indexed after the current one
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Method to quit the game
    public void QuitGame()
    {
        Debug.Log("Quit Game"); // Log for debugging (only visible in the editor)
        Application.Quit();
    }
}
