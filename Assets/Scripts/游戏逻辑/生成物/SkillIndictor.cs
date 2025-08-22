using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class SkillIndictor : InstantiateObjBase
{
    [Header("ÊÇ·ñ¸úËæÊó±êÒÆ¶¯")]
    public bool followMouse;

    [Header("¼¼ÄÜ·¶Î§°ë¾¶")]
    public float radius;

    private GameObject owner;

    private PlayerInputAction action;

    public string destroyInstantiateObjName;

    public override void Init(object owner, float lifeTime = -1)
    {
        base.Init(owner, lifeTime);
        this.owner = owner as GameObject;

    }

    protected override void Update()
    {
        base.Update();

        if (followMouse)
        {
            bool checkSucc;
            Vector3 mousePos = MouseUtility.GetClickedPosition(PlayerInputManager.Instance.action.GamePlay.MousePosition.ReadValue<Vector2>(), "Ground", out checkSucc);
            float distance = Vector3.Distance(mousePos, owner.transform.position);
            if (checkSucc && distance <= radius)
            {
                transform.position = new Vector3(mousePos.x, 1f, mousePos.z);
            }

        }
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(destroyInstantiateObjName))
        {
            GameObject vfxObj = LoadManager.Instance.GetResourceByName<GameObject>(destroyInstantiateObjName);

            var go = Instantiate(vfxObj);
            go.transform.position = new Vector3(transform.position.x, go.transform.position.y, transform.position.z);
            go.GetComponent<InstantiateObjBase>().Init(owner, -1);
        }
    }
}
