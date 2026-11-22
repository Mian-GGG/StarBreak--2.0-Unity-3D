using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOut : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene("LevelComplete");
        gameObject.SetActive(false);
    }

    public bool IsInteractable()
    {
        return true;
    }

}
