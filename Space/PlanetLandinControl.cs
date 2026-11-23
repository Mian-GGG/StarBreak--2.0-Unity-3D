using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetLandinControl : MonoBehaviour, IInteractable
{

    [SerializeField] private InteractionManager interactionManager;

    public void Interact()
    {
        if(interactionManager != null)
        {
            if (interactionManager.canEnterPlanet)
            {
                SceneManager.LoadScene("LandingScene");
            }
        }
    }

    public bool IsInteractable()
    {
        return true;
    }
}
