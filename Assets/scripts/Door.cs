using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : OpenableObject
{

    [SerializeField]
    private bool _isOpenable = false;

    protected override bool Check() {
        return _isOpenable;
    }
}
