using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions.Must;

public class InstantiateObj_AttackDetector : InstantiateObjBase, IAttackDetector
{
    [Header("检测配置")]
    public LayerMask targetLayers = ~0;

    private float lifeTimer = 0;
    private bool isDestroying = false;

    public string vfxName;

    private CharacterBehaviourController fromChara;

    public class CollisionInfo
    {
        public Collider target;
        public Vector3 collisionPoint;
        public Vector3 collisionNormal;

        public CollisionInfo(Collider target, Vector3 collisionPoint, Vector3 collisionNormal)
        {
            this.target = target;
            this.collisionPoint = collisionPoint;
            this.collisionNormal = collisionNormal;
        }
    }

    // 碰撞体管理
    protected Queue<CollisionInfo> detectedColliders = new Queue<CollisionInfo>();
    protected HashSet<Collider> processedColliders = new HashSet<Collider>();
    protected List<Collider> blockList = new List<Collider>();
    protected float checkTimer = 0f;
    protected Vector3 attackDirection = Vector3.zero;
    protected Transform ownerTransform;

    protected bool isDealAttack;
    protected bool isInitialized = false;

    [SerializeField] private DamageFormulaType damageFormulaType;
    public DamageFormulaType Type => damageFormulaType;

    public override void Init(object owner, float lifeTime = -1)
    {
        fromChara = owner as CharacterBehaviourController;
        if (fromChara != null)
        {
            blockList.Clear();
            Collider ownerCollider = fromChara.GetComponent<Collider>();
            if (ownerCollider != null)
                blockList.Add(ownerCollider);

            ownerTransform = fromChara.transform;
            isInitialized = true;
        }

        processedColliders.Clear();
        detectedColliders.Clear();
        isDealAttack = false;
        lifeTimer = Time.time;

        // 注册逻辑更新事件
        EventCenter.Subscribe(EventCenter.EventId.LogicFrameUpdate, LogicUpdate);
    }

    protected void LogicUpdate(object param = null)
    {
        if (!IsLocal || isDestroying) return;

        if (!isDealAttack && detectedColliders.Count > 0)
        {
            isDealAttack = true;
            while (detectedColliders.Count > 0)
            {
                CollisionInfo target = detectedColliders.Dequeue();
                if (target.target.GetComponent<NetMonobehavior>() && target.target.GetComponent<NetMonobehavior>().NetID != ownerTransform.GetComponent<NetMonobehavior>().NetID)
                    OnTargetDetected(target);
            }
            isDealAttack = false;
        }

    }

    /// <summary>
    /// 本地调用
    /// </summary>
    /// <param name="colInfo"></param>
    protected virtual void OnTargetDetected(CollisionInfo colInfo)
    {
        if (colInfo == null || ownerTransform == null) return;

        ICanInteract attackTarget = colInfo.target.GetComponent<ICanInteract>();
        if (attackTarget != null && attackTarget.CanBeAttack(fromChara.GetComponent<CampFlag>()))
        {
            // 计算从目标指向owner的方向
            attackDirection = (ownerTransform.position - colInfo.target.transform.position).normalized;

            if (string.IsNullOrEmpty(vfxName))
            {
                var go = LoadManager.Instance.NetInstantiate(vfxName);

                go.transform.position = colInfo.collisionPoint;
                go.transform.rotation = Quaternion.LookRotation(colInfo.collisionNormal);
            }


            //CameraManager.Instance.ShakeCamera();

            //ownerTransform.GetComponent<AudioSource>().clip = clipList[Random.Range(0, clipList.Count)];
            //ownerTransform.GetComponent<AudioSource>().Play();

            CharacterDataObj attacker = fromChara.GetComponent<CharacterDataController>().data;
            CharacterDataObj beAttacker = colInfo.target.GetComponent<CharacterDataController>().data;

            float damage = DamageCaculateCollection.CaculateDamage(damageFormulaType, attacker, beAttacker);
            attackTarget.GetDamage(attackDirection, damage);

            blockList.Remove(colInfo.target);
        }
    }


    #region Trigger事件处理
    void OnTriggerEnter(Collider other)
    {
        if (!isInitialized && !IsLocal) return;
        ProcessCollider(other);
    }

    void OnTriggerStay(Collider other)
    {
        if (!isInitialized && !IsLocal) return;
        ProcessCollider(other);
    }

    void ProcessCollider(Collider other)
    {
        //// 层过滤
        //if (!((1 << other.gameObject.layer) & targetLayers.value))
        //    return;

        // 屏蔽列表过滤
        if (blockList.Contains(other))
            return;

        // 去重并加入队列
        if (!processedColliders.Contains(other))
        {
            processedColliders.Add(other);

            Vector3 pos, normal;
            //获得接触点
            GetImpactInfo(other, out pos, out normal);
            detectedColliders.Enqueue(new CollisionInfo(other, pos, normal));
        }
    }
    private void GetImpactInfo(Collider other, out Vector3 impactPos, out Vector3 impactNormal)
    {

        // 方法1：使用射线检测获取更精确的碰撞位置和法线
        (impactPos, impactNormal) = GetImpactPositionByRaycast(other);

    }
    // 使用射线检测获取精确碰撞位置和法线
    private (Vector3 position, Vector3 normal) GetImpactPositionByRaycast(Collider other)
    {
        // 从触发物体中心向目标物体发射射线
        Vector3 direction = (other.transform.position - transform.position).normalized;
        Vector3 origin = transform.position;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, .5f, targetLayers))
        {

            return (hit.point, hit.normal);
        }


        return (GetImpactPositionByBounds(other), direction);
    }
    // 使用碰撞体边界估算碰撞位置
    private Vector3 GetImpactPositionByBounds(Collider other)
    {
        // 获取两个碰撞体的边界
        Bounds triggerBounds = GetComponent<Collider>().bounds;
        Bounds otherBounds = other.bounds;

        // 计算两个边界的最近点
        Vector3 closestPoint = otherBounds.ClosestPoint(triggerBounds.center);


        //Debug.DrawRay(closestPoint, Vector3.up * 0.2f, Color.yellow, 2f);


        return closestPoint;
    }
    #endregion
}



