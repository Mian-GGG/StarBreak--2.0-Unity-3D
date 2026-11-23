using UnityEngine;
using UnityEngine.UI;

public class LandingInitiater : MonoBehaviour, IInteractable
{
    [SerializeField] private string _landingScene = "LandingScene";
    private InteractionManager interactionManager = GameObject.Find("Player").GetComponent<InteractionManager>();

    public void Interact()
    {
        if (SpaceUIManager.Instance != null)
            SpaceUIManager.Instance.TriggerLanding();
    }

    public bool IsInteractable()
    {
        if (interactionManager.canEnterPlanet == true)
        {
            return true;
        }
        return false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
