using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public GameObject interactPrompt, enemyParent, cantInteractPromp;

    private Camera playerCamera;
    public bool canInteract, isSpace, canEnterPlanet;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (!isSpace)
        {
            if (enemyParent.transform.childCount > 0)
            {
                CheckForInteractable(cantInteractPromp, canInteract);
            }
            else if (enemyParent.transform.childCount == 0)
            {
                canInteract = true;
                CheckForInteractable(interactPrompt, canInteract);
            }
        }
        else if (isSpace)
        {
            if (!canEnterPlanet)
            {
                CheckForInteractable(cantInteractPromp,canInteract);
            }
            else if (canEnterPlanet)
            {
                canInteract = true;
                CheckForInteractable(interactPrompt, canInteract);
            }
        }
        
    }

    void CheckForInteractable(GameObject prompt, bool canInteract)
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable == null)
                interactable = hit.transform.GetComponentInParent<IInteractable>();

            if (interactable != null && interactable.IsInteractable())
            {
                prompt.SetActive(true);
                if (canInteract)
                {
                    if (!isSpace)
                    {
                        if (Input.GetKeyDown(KeyCode.E))
                            interactable.Interact();
                    }
                    else if (isSpace)
                    {
                        Debug.Log("Space, seeing if key pressed");
                        if (Input.GetKeyDown(KeyCode.T))
                            Debug.Log("Key pressed");
                            interactable.Interact();
                    }
                    
                }
                

                return;
            }
        }

        prompt.SetActive(false);
    }
}