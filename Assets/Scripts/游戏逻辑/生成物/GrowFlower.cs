using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowFlower : NetMonobehavior
{

    public float growTime = 5f;
    public float curLifeTimer = 0f;

    private CharacterBehaviourController ownerCBCtrl;

    private bool active;

    [SerializeField] private DamageFormulaType damageFormula;

    public DamageFormulaType Type => damageFormula;
    public void Init(object owner)
    {

        active = false;
        ownerCBCtrl = owner as CharacterBehaviourController;

    }


    private void Update()
    {
        if (!IsLocal) return;

        if (curLifeTimer <= growTime)
        {
            curLifeTimer += Time.deltaTime;



            if (curLifeTimer >= growTime)
            {
                active = true;
            }
        }
        else
        {
            if (transform.position.y < 1f)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * 1f, (1f / 2f) * Time.deltaTime);
            }
        }

    }

    protected void OnTargetDetected(Collider colInfo)
    {
        if (colInfo == null) return;

        GameObject colTarget = colInfo.gameObject;

        ICanInteract interactObj = colTarget.GetComponent<ICanInteract>();
        if (interactObj == null) return;

        bool isFriendly = ownerCBCtrl.GetComponent<CampFlag>().CheckIsFriendly(colTarget.GetComponent<CampFlag>().CampType);

        if (active)
        {
            if (isFriendly)
            {
                if (interactObj.CanBeHealth(colTarget.GetComponent<CampFlag>()))
                {
                    interactObj.GetHealth(DamageCaculateCollection.CaculateDamage(Type, ownerCBCtrl.GetComponent<CharacterDataController>().data
                        , colTarget.GetComponent<CharacterDataController>().data));
                    NetDestroy(NetID,gameObject);
                }

            }
            else
            {


                if (interactObj.CanBeAttack(colTarget.GetComponent<CampFlag>()))
                {
                    interactObj.GetDamage(Vector3.zero, DamageCaculateCollection.CaculateDamage(Type, ownerCBCtrl.GetComponent<CharacterDataController>().data
                   , colTarget.GetComponent<CharacterDataController>().data));
                    NetDestroy(NetID, gameObject);
                }

            }
        }
        else
        {
            if (!isFriendly)
            {
                NetDestroy(NetID, gameObject);
            }
        }


    }


}
