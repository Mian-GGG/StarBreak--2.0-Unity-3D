using System.Collections;
using UnityEngine;

public class KeypadWareHouseDoor : MonoBehaviour, IInteractable
{
    public GameObject warehouseDoor, fixingPipe, noElectricPrompt, lever;
    private bool isUnlocked = false, isObjInteractable = true, iselectricOn = false;
    public LevelManager levelManager;

    public void Update()
    {
        if(lever.transform.rotation.x == 38.058)
        {
            iselectricOn = true;
        }
        else if(lever.transform.rotation.x == -90)
        {
            iselectricOn = false;
        }
    }

    public void Interact()
    {
        if (fixingPipe.activeSelf == true && !isUnlocked && iselectricOn)
        {
            SequenceGame.Instance.StartGame(
        () => KeypadDone(),
        () => Debug.Log("Failed!")
    );
        }
        else if(fixingPipe.activeSelf != true || !iselectricOn)
        {
            StartCoroutine(ShowPrompt(noElectricPrompt));
        }
        else if (isUnlocked)
        {
            return;
        }
        
    }

    public void KeypadDone()
    {
        isUnlocked = true;
        isObjInteractable = false;
        warehouseDoor.GetComponent<DoorSlider>().Open();
        levelManager.isInWareHouse = true;
    }

    public bool IsInteractable()
    {
        return isObjInteractable;
    }

    public IEnumerator ShowPrompt(GameObject prompt)
    {
        prompt.SetActive(true);
        yield return new WaitForSeconds(3f);
        prompt.SetActive(false);
    }
}

