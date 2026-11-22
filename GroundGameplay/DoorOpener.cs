using UnityEngine;

public class DoorOpener : MonoBehaviour, IInteractable
{

    [Header("Rotation Settings")]
    public float openAngle = 0f;
    public float closedAngle = -90f;
    public float speed = 2f;

    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public GameObject interactPrompt, sparks;
    public bool isLever = false, isObjInteractable = true;



    private bool isOpen = false;

    public void Update()
    {
        if (isLever)
        {
            if(transform.localEulerAngles.x == 270)
            {
            sparks?.SetActive(true);
            }
            else
            {
                sparks?.SetActive(false);
            }
        }

    }

    public bool IsInteractable()
    {
        return isObjInteractable;
    }
    public void Interact()
    {
        StartCoroutine(Rotate(isOpen ? closedAngle : openAngle));
        isOpen = !isOpen;
    }

    System.Collections.IEnumerator Rotate(float targetAngle)
    {
        Quaternion startRot = transform.localRotation;
        Quaternion targetRot = Quaternion.Euler(targetAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);

        float elapsed = 0f;

        while (elapsed < speed)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / speed);
            transform.localRotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        transform.localRotation = targetRot;
    }
}
