using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions.Must;

public class InstantiateObj_AttackDetector : InstantiateObjBase, IAttackDetector
{
    [Header("�������")]
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

    // ��ײ�����
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

        // ע���߼������¼�
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
    /// ���ص���
    /// </summary>
    /// <param name="colInfo"></param>
    protected virtual void OnTargetDetected(CollisionInfo colInfo)
    {
        if (colInfo == null || ownerTransform == null) return;

        ICanInteract attackTarget = colInfo.target.GetComponent<ICanInteract>();
        if (attackTarget != null && attackTarget.CanBeAttack(fromChara.GetComponent<CampFlag>()))
        {
            // �����Ŀ��ָ��owner�ķ���
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


    #region Trigger�¼�����
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
        //// �����
        //if (!((1 << other.gameObject.layer) & targetLayers.value))
        //    return;

        // �����б����
        if (blockList.Contains(other))
            return;

        // ȥ�ز��������
        if (!processedColliders.Contains(other))
        {
            processedColliders.Add(other);

            Vector3 pos, normal;
            //��ýӴ���
            GetImpactInfo(other, out pos, out normal);
            detectedColliders.Enqueue(new CollisionInfo(other, pos, normal));
        }
    }
    private void GetImpactInfo(Collider other, out Vector3 impactPos, out Vector3 impactNormal)
    {

        // ����1��ʹ�����߼���ȡ����ȷ����ײλ�úͷ���
        (impactPos, impactNormal) = GetImpactPositionByRaycast(other);

    }
    // ʹ�����߼���ȡ��ȷ��ײλ�úͷ���
    private (Vector3 position, Vector3 normal) GetImpactPositionByRaycast(Collider other)
    {
        // �Ӵ�������������Ŀ�����巢������
        Vector3 direction = (other.transform.position - transform.position).normalized;
        Vector3 origin = transform.position;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, .5f, targetLayers))
        {

            return (hit.point, hit.normal);
        }


        return (GetImpactPositionByBounds(other), direction);
    }
    // ʹ����ײ��߽������ײλ��
    private Vector3 GetImpactPositionByBounds(Collider other)
    {
        // ��ȡ������ײ��ı߽�
        Bounds triggerBounds = GetComponent<Collider>().bounds;
        Bounds otherBounds = other.bounds;

        // ���������߽�������
        Vector3 closestPoint = otherBounds.ClosestPoint(triggerBounds.center);


        //Debug.DrawRay(closestPoint, Vector3.up * 0.2f, Color.yellow, 2f);


        return closestPoint;
    }
    #endregion
}



