using UnityEngine;

public class DoorSlider : MonoBehaviour, IInteractable
{
    public enum SlideAxis { X, Y, Z }

    [Header("Slide Settings")]
    public SlideAxis axis = SlideAxis.X;
    public float targetPosition = 2.5f;
    public float speed = 2f;

    [Header("Interaction")]
    public bool isObjInteractable;

    private Vector3 openPosition;
    private Vector3 closedPosition;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        closedPosition = transform.localPosition;

        openPosition = closedPosition;
        switch (axis)
        {
            case SlideAxis.X: openPosition.x = targetPosition; break;
            case SlideAxis.Y: openPosition.y = targetPosition; break;
            case SlideAxis.Z: openPosition.z = targetPosition; break;
        }
    }

    public bool IsInteractable()
    {
        return isObjInteractable;
    }

    public void Interact()
    {
        if (!isObjInteractable) return;
        if (isMoving) return;
        StartCoroutine(Slide(isOpen ? closedPosition : openPosition));
        isOpen = !isOpen;
    }

    public void Open()
    {
        if (isMoving) return;
        StartCoroutine(Slide(openPosition));
        isOpen = true;
    }

    System.Collections.IEnumerator Slide(Vector3 target)
    {
        isMoving = true;
        Vector3 start = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < speed)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / speed);
            transform.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localPosition = target;
        isMoving = false;
    }
}
