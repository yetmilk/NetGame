using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCharacterCtrl : MonoBehaviour
{
    public Button applyBtn;

    public CharacterClacify curClacify;

    private void Start()
    {
        applyBtn.onClick.AddListener(OnApplyClick);
    }

    public void OnApplyClick()
    {
        PlayerManager.Instance.ChangePlayerClacify(PlayerManager.Instance.selfId, curClacify);
        gameObject.SetActive(false);
    }

    public void SetClacify(string curClacify)
    {
        this.curClacify = Enum.Parse<CharacterClacify>(curClacify)
            ;
    }
}
