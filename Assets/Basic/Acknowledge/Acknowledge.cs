using UnityEngine;

public class Acknowledge: MonoBehaviour {

    private GameObject _child;

    void Start() {
        _child = transform.GetChild(0).gameObject;
    }

    void Update() {
        _child.SetActive(Input.GetKey(KeyCode.T) && Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.S));
    }
}
