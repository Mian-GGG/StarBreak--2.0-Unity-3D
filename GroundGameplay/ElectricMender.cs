using System.Collections;
using UnityEngine;

public class ElectricMender : MonoBehaviour, IInteractable
{
    public GameObject sparks, pipe, mendedPrompt, cantMendPrompt;
    public bool canMend = false, isObjInteractable;


    void Update()
    {
        if(sparks.activeSelf == false)
        {
            canMend = true;
        }
        else
        {
            canMend = false; 
        }
    }

    public bool IsInteractable()
    {
        return isObjInteractable;
    }

    public void Interact()
    {
        if (canMend)
        {
            pipe.SetActive(true);
            Destroy(sparks);
            StartCoroutine(ShowPrompt(mendedPrompt));
        }
        else
        {
            StartCoroutine(ShowPrompt(cantMendPrompt));
        }
    }

    public IEnumerator ShowPrompt(GameObject prompt)
    {
        prompt.SetActive(true);
        yield return new WaitForSeconds(3f);
        prompt.SetActive(false);
    }
}
