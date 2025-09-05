using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FireLiquid : MonoBehaviour
{
    // ���Ŀ��㣨��ң�
    public LayerMask targetLayers;
    // �ϰ���㣨����Ȼ��赲���յ����壩
    public LayerMask obstacleLayers;
    // ���߼���������
    public float maxRayDistance = 3f;
    // ��¼��ǰ�ڴ�����Χ�ڵ�Ŀ��
    private HashSet<Collider> _targetsInRange = new HashSet<Collider>();

    private void Awake()
    {
        // ȷ����ײ���Ǵ�����
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
        // ȷ������������ϵͳ������ײ
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
    }

    private void Update()
    {
        // ��������ڷ�Χ�ڵ�Ŀ��
        foreach (var targetCollider in new List<Collider>(_targetsInRange))
        {
            if (targetCollider == null) continue;

            // ��Ŀ��ŵ����ҽ��������߼��
            if (IsTargetTouchingLava(targetCollider))
            {
                ApplyBurnBuff(targetCollider);
            }
        }
    }

    /// <summary>
    /// ���Ŀ���Ƿ�ֱ�ӽӴ��ҽ������ϰ��
    /// </summary>
    private bool IsTargetTouchingLava(Collider targetCollider)
    {
        // ��ȡĿ��ŵ�λ�ã����轺����ײ�壩
        Vector3 footPosition = targetCollider.bounds.center;
        footPosition.y = targetCollider.bounds.min.y; // �ŵ�λ��

        // �����·��������߼���ҽ�
        if (Physics.Raycast(footPosition, Vector3.down, out RaycastHit hit, maxRayDistance))
        {
            // ���߻����˵�ǰ�ҽ�������û�б������ϰ����赲
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }

    /// <summary>
    /// ΪĿ��Ӧ������Buff
    /// </summary>
    private void ApplyBurnBuff(Collider targetCollider)
    {
        CharacterController target = targetCollider.GetComponent<CharacterController>();
        if (target == null) return;

        AddBuffInfo addBuffInfo = new AddBuffInfo(
            BuffName.����.ToString(),
            target,
            target
        );

        if (target.IsLocal&&target.selfActionCtrl.curActionObj.curActionInfo.tag!=ActionTag.Parry)
        {
            target.curCharaData.buffController.AddBuff(addBuffInfo);
        }
    }

    // ��Ŀ����봥������Χ
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            _targetsInRange.Add(other);
        }
    }

    // ��Ŀ���뿪��������Χ
    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            _targetsInRange.Remove(other);
        }
    }

    // �������ߵ���
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // ���ƴ�������Χ
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }

        // ��������ʾ��
        foreach (var target in _targetsInRange)
        {
            if (target != null)
            {
                Vector3 footPosition = target.bounds.center;
                footPosition.y = target.bounds.min.y;
                Gizmos.DrawLine(footPosition, footPosition + Vector3.down * maxRayDistance);
            }
        }
    }
}
