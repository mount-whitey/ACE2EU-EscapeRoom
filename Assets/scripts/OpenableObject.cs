using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableObject : MonoBehaviour, IInteractable
{
    [Header("Defaults")]
    [SerializeField]
    protected bool _isOpen = false;

    Animator[] _animators;

    private void Awake() {
        _animators = GetComponentsInChildren<Animator>();
    }

    private void Start() {

        if (_isOpen) {
            Trigger();
        }
    }

    public void Interact() {

        if (!Check()) {
            return;
        }

        Trigger();
        _isOpen = !_isOpen;
    }

    private void Trigger() {
        foreach (var anim in _animators) {
            anim.SetTrigger("Interact");
        }
    }

    protected virtual bool Check() {
        return true;   
    }
}
