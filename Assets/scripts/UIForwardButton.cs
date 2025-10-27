using UnityEngine;
using UnityEngine.EventSystems;

public class UIForwardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private PlayerController playerController;

    public void OnPointerDown(PointerEventData eventData)
    {
        playerController.SetMoveForward(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        playerController.SetMoveForward(false);
    }
}
