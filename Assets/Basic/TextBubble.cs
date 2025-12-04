using TMPro;

using UnityEngine;

public enum Style {
    Header,
    Message
}

public class TextBubble : MonoBehaviour
{
    [Header("Segments")]
    [SerializeField]
    private TMP_Text _prePart;
    [SerializeField]
    private TMP_Text _mainLeft;
    [SerializeField]
    private TMP_Text _mainMid;
    [SerializeField]
    private TMP_Text _mainRight;
    [SerializeField]
    private TMP_Text _postPart;

    [Header("Styles")]
    [SerializeField]
    private GameObject _header;
    [SerializeField]
    private GameObject _message;

    public Style Style {
        set {
            _header.SetActive(value == Style.Header);
            _message.SetActive(value == Style.Message);
        }
    }

    /// <summary>
    /// The left side of the Main part
    /// </summary>
    public string MainLeft {
        set {
            _mainLeft.text = value;    
        }
    }

    /// <summary>
    /// The right side of the Main part
    /// </summary>
    public string MainMid{
        set {
            _mainMid.text = value;
        }
    }

    /// <summary>
    /// The right side of the Main part
    /// </summary>
    public string MainRight {
        set {
            _mainRight.text = value;
        }
    }

    /// <summary>
    /// The right side of the Main part
    /// </summary>
    public string PrePart {
        set {
            _prePart.text = value;
        }
    }

    /// <summary>
    /// The right side of the Main part
    /// </summary>
    public string PostPart {
        set {
            _postPart.text = value;
        }
    }
}
