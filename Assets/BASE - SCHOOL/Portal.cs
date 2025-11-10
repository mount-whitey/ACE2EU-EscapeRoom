using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal: MonoBehaviour {

    [Header("Room of Country")]
    [SerializeField]
    private string _scene = "";

    public void EnterScene() {

        if (SceneManager.GetSceneByName(_scene) == null) {
            Debug.LogError("Scene not available - " + _scene);
            return;
        }

        SceneManager.LoadSceneAsync(_scene, LoadSceneMode.Single);
    }
}
