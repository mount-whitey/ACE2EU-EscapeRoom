using UnityEngine;

public class CloseGame : MonoBehaviour
{
    private void Update()
    {
        // Check if the "K" key is pressed
        if (Input.GetKeyDown(KeyCode.K))
        {
            CloseTheGame();
        }
    }

    private void CloseTheGame()
    {
        // Quit the game in a built application
#if UNITY_EDITOR
        // In Unity Editor, we cannot close the editor, so we can simulate quitting with a log
        Debug.Log("Game is closed in the editor. Press K in the built version.");
#else
            Application.Quit(); // Quit the game in a built application
            Debug.Log("Game is quitting...");
#endif
    }
}
