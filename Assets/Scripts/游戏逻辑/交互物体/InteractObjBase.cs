using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InteractObjBase : MonoBehaviour
{


    bool canInteract;
    private void Start()
    {
        PlayerInputManager.Instance.action.GamePlay.Interact.started += Interact_started;
    }

    private void Interact_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (canInteract)
            OnInteract();

        canInteract = false;
    }

    protected virtual void OnInteract()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDealActionCommand>() != null)
        {
            canInteract = true;

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            if (other.GetComponent<CharacterController>().IsLocal)
                canInteract = true;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            if (other.GetComponent<CharacterController>().IsLocal)
                canInteract = false;

        }
    }
    private void OnDestroy()
    {
        PlayerInputManager.Instance.action.GamePlay.Interact.started -= Interact_started;
    }
}
