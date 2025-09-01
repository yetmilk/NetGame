using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public static class FunctionId
{
    public const string Move = "Move";
    public const string SubmitActionChangeQuest = "SubmitActionChangeQuest";
    public const string Rotate = "Rotate";
    public const string InstantiateObj = "InstantiateObj";
    public const string SetRigidBody = "SetRigidBody";
}
public static class ActionLogicCollection
{


    private static Dictionary<string, Action<object, FunctionParam>> functionMap = new Dictionary<string, Action<object, FunctionParam>>();


    static ActionLogicCollection()
    {

        functionMap[FunctionId.Move] = (owner, param) => Move(owner, param as MoveFunctionParam);
        functionMap[FunctionId.InstantiateObj] = (owner, param) => InstantiateObj(owner, param as OnInstantiateObjFunctionParam);
        functionMap[FunctionId.Rotate] = (owner, param) => RotateToDirection(owner, param as RotateFuncParam);
        functionMap[FunctionId.SubmitActionChangeQuest] = (owner, param) => UpdateSubmitDic(owner, param as SubmitFuncParam);
        functionMap[FunctionId.SetRigidBody] = (owner, param) => SetRigidBody(owner, param as RigidbodyParam);
    }

    public static void Move(object questOwner, MoveFunctionParam param)
    {
        CharacterController fromCBCtrl = questOwner as CharacterController;
        var rig = fromCBCtrl.GetComponent<Rigidbody>();
       var transform = fromCBCtrl.transform;

        Vector3 dir = Vector3.zero;



        if (param.useActionDir)
            dir = fromCBCtrl.selfActionCtrl.curActionObj.direction;
        else
            dir = param.moveDir;



        CharacterDataObj data = fromCBCtrl.curCharaData.GetDataObj();
        float speed = param.useDataSpeed ? (data != null ? data.GetDataByEnum(param.dataName) : param.speed) : param.speed;

        Ray ray = new Ray(fromCBCtrl.transform.position + fromCBCtrl.transform.up * 1f + fromCBCtrl.transform.forward * .3f, fromCBCtrl.transform.forward);
        Debug.DrawRay(fromCBCtrl.transform.position + fromCBCtrl.transform.up * 1f + fromCBCtrl.transform.forward * 1f, fromCBCtrl.transform.forward, Color.red);
        RaycastHit hit;
        bool isCol = Physics.Raycast(ray, out hit, .5f);
        Vector3 raydir = Vector3.zero;
        //Debug.Log(isCol);
        if (isCol)
        {
            Debug.Log(hit.collider.name);
            raydir = Vector3.Cross(ray.direction, Vector3.up) * 1.2f;
            //Debug.DrawRay(fromCBCtrl.transform.position + Vector3.up * 2f, raydir, Color.green);
        }


        switch (param.useRig)
        {
            case true:
                // 
                float verticalVelocity = rig.velocity.y;


                Vector3 horizontalMovement;
                if (param.useLocalSpace)
                {

                    horizontalMovement = ((rig.transform.right * dir.x) + rig.transform.forward * dir.z) + raydir;
                    horizontalMovement *= speed;

                }
                else
                {

                    horizontalMovement = ((Vector3.right * dir.x) + Vector3.forward * dir.z) + raydir;
                    horizontalMovement *= speed;
                }

                rig.velocity = new Vector3(horizontalMovement.x, verticalVelocity, horizontalMovement.z);
                break;
            case false:

                if (param.useLocalSpace)
                {

                    horizontalMovement = ((rig.transform.right * dir.x) + rig.transform.forward * dir.z) + raydir;


                }
                else
                {

                    horizontalMovement = ((Vector3.right * dir.x) + Vector3.forward * dir.z) + raydir;

                }
                transform.position += (FrameManager.LogicFrameInterval * speed) * horizontalMovement;
                break;
        }
    }
    public static void InstantiateObj(object questOwner, OnInstantiateObjFunctionParam vfxParam)
    {


        CharacterController fromObj = questOwner as CharacterController;
        if (!fromObj.IsLocal) return;

        var vfxObj = LoadManager.Instance.NetInstantiate(vfxParam.vfxName, fromObj.transform,fromObj.NetID);

        if (vfxObj.GetComponent<IInstantiateObj>() != null)
            vfxObj.GetComponent<IInstantiateObj>().Init(fromObj, vfxParam.VfxLifeTime);

        if (vfxParam.selfAsparent)
        {
            vfxObj.transform.parent = fromObj.transform;
            vfxObj.transform.localPosition = vfxParam.startDirection * vfxParam.distance;
            //vfxObj.transform.forward = fromObj.transform.forward;
        }
        else
        {
            vfxObj.transform.parent = null;

            Vector3 dir = (fromObj.transform.right * vfxParam.startDirection.x + fromObj.transform.up * vfxParam.startDirection.y + fromObj.transform.forward * vfxParam.startDirection.z).normalized;
            vfxObj.transform.position = fromObj.transform.position + dir * vfxParam.distance;
            //vfxObj.transform.forward = fromObj.transform.forward;
        }

    }

    public static void RotateToDirection(object questOwner, RotateFuncParam param)
    {
        CharacterController fromObj = questOwner as CharacterController;
        if (fromObj == null) return;

        RotateFuncParam rotParam = param as RotateFuncParam;
        Vector3 targetDirection = Vector3.zero;

        //Debug.Log(fromObj.playerID.flag);

        if (param.useActionDir)
        {
            targetDirection = fromObj.selfActionCtrl.curActionObj.direction;
        }
        else
        {
            targetDirection = param.rotateDir;
        }



        targetDirection.y = 0;
        targetDirection = Vector3.Normalize(targetDirection);

        if (targetDirection.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);


        if (Quaternion.Angle(fromObj.transform.rotation, targetRotation) < 0.5f)
            return;

        if (param.useSmooth)
        {
            float step = param.rotationSpeed * Time.deltaTime;
            fromObj.transform.rotation = Quaternion.RotateTowards(
                fromObj.transform.rotation,
                targetRotation,
                step
            );
        }
        else
        {

            fromObj.transform.rotation = targetRotation;
        }
    }

    private static void UpdateSubmitDic(object questOwner, SubmitFuncParam param)
    {
        CharacterController fromObj = questOwner as CharacterController;
        ActionController acCtrl = fromObj.GetComponent<ActionController>();
        acCtrl.AddSubmitAction(param);
    }

    [System.Serializable]
    [FunctionParam(FunctionId.SetRigidBody)]
    public class RigidbodyParam : FunctionParam
    {
        [Header("isKenamic状态")]
        public bool isKenamicActive;
        [Header("Collider状态")]
        public bool colliderActive;

        public RigidbodyParam()
        {

        }
        public RigidbodyParam(RigidbodyParam RigidbodyParam)
        {
            isKenamicActive = RigidbodyParam.isKenamicActive;
        }
    }

    private static void SetRigidBody(object questOwner, RigidbodyParam param)
    {
        CharacterController fromObj = questOwner as CharacterController;

        fromObj.GetComponent<Rigidbody>().isKinematic = param.isKenamicActive;
        fromObj.GetComponent<Collider>().enabled = param.colliderActive;
    }

    #region-------------------------������-------------------------------
    // ִ�к�����ͳһ�ӿ�
    public static bool Execute(string functionId, object questOwner, FunctionParam param)
    {
        if (functionMap.TryGetValue(functionId, out var action))
        {
            action(questOwner, param);
            return true;
        }

        Debug.LogError($"δ�ҵ�����ID: {functionId}");
        return false;
    }
    #endregion
}



