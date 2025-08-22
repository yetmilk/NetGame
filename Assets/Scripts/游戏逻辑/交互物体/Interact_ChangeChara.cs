using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_ChangeChara : InteractObjBase
{
    public GameObject selectPlayerPanel;
    protected override void OnInteract()
    {
        base.OnInteract();
        selectPlayerPanel.SetActive(true);
    }
}
