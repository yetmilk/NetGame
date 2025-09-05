using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FireLiquid : MonoBehaviour
{
    // 检测目标层（玩家）
    public LayerMask targetLayers;
    // 障碍物层（地面等会阻挡灼烧的物体）
    public LayerMask obstacleLayers;
    // 射线检测的最大距离
    public float maxRayDistance = 3f;
    // 记录当前在触发范围内的目标
    private HashSet<Collider> _targetsInRange = new HashSet<Collider>();

    private void Awake()
    {
        // 确保碰撞体是触发器
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
        // 确保不会与物理系统产生碰撞
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
    }

    private void Update()
    {
        // 检查所有在范围内的目标
        foreach (var targetCollider in new List<Collider>(_targetsInRange))
        {
            if (targetCollider == null) continue;

            // 从目标脚底向岩浆发射射线检测
            if (IsTargetTouchingLava(targetCollider))
            {
                ApplyBurnBuff(targetCollider);
            }
        }
    }

    /// <summary>
    /// 检测目标是否直接接触岩浆（无障碍物）
    /// </summary>
    private bool IsTargetTouchingLava(Collider targetCollider)
    {
        // 获取目标脚底位置（假设胶囊碰撞体）
        Vector3 footPosition = targetCollider.bounds.center;
        footPosition.y = targetCollider.bounds.min.y; // 脚底位置

        // 向正下方发射射线检测岩浆
        if (Physics.Raycast(footPosition, Vector3.down, out RaycastHit hit, maxRayDistance))
        {
            // 射线击中了当前岩浆物体且没有被其他障碍物阻挡
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }

    /// <summary>
    /// 为目标应用灼烧Buff
    /// </summary>
    private void ApplyBurnBuff(Collider targetCollider)
    {
        CharacterController target = targetCollider.GetComponent<CharacterController>();
        if (target == null) return;

        AddBuffInfo addBuffInfo = new AddBuffInfo(
            BuffName.灼烧.ToString(),
            target,
            target
        );

        if (target.IsLocal&&target.selfActionCtrl.curActionObj.curActionInfo.tag!=ActionTag.Parry)
        {
            target.curCharaData.buffController.AddBuff(addBuffInfo);
        }
    }

    // 当目标进入触发器范围
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            _targetsInRange.Add(other);
        }
    }

    // 当目标离开触发器范围
    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            _targetsInRange.Remove(other);
        }
    }

    // 绘制射线调试
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // 绘制触发器范围
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }

        // 绘制射线示例
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
