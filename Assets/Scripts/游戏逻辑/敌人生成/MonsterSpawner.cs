using UnityEngine;
using System;
using System.Collections.Generic;

// 生成范围形状类型
public enum SpawnShape
{
    Circle,    // 圆形（2D）
    Rectangle, // 矩形（2D）
    Sphere     // 球形（3D）
}

[Serializable]
public class SpawnRangeSettings
{
    public SpawnShape shape;              // 范围形状
    public Vector3 center;                // 范围中心点
    public Vector3 size;                  // 范围大小（根据形状有不同含义）
    public bool useLocalPosition = true;  // 是否使用本地坐标
}

public class MonsterSpawner : NetMonobehavior
{
    [Header("生成设置")]
    public List<CharacterClacify> monsterPrefabs;  // 怪物预制体列表
    public SpawnRangeSettings spawnRange;    // 生成范围设置
    public int maxMonsters = 10;             // 最大怪物数量限制

    [Header("调试")]
    public bool drawGizmos = true;           // 是否绘制范围 gizmos
    public Color gizmosColor = Color.green;  // Gizmos 颜色

    private List<GameObject> spawnedMonsters = new List<GameObject>();  // 已生成的怪物

    /// <summary>
    /// 在指定范围内生成指定数量的怪物
    /// </summary>
    /// <param name="count">要生成的怪物数量</param>
    public void SpawnMonsters(int count)
    {
        // 检查参数合法性
        if (count <= 0)
        {
            Debug.LogWarning("生成数量必须大于0");
            return;
        }

        if (monsterPrefabs == null || monsterPrefabs.Count == 0)
        {
            Debug.LogWarning("没有设置怪物预制体");
            return;
        }

        // 计算实际能生成的数量（不超过最大限制）
        int actualCount = Mathf.Min(count, maxMonsters - spawnedMonsters.Count);
        if (actualCount <= 0)
        {
            Debug.LogWarning("已达到最大怪物数量限制");
            return;
        }

        // 获取世界空间中的范围中心点
        Vector3 worldCenter = spawnRange.useLocalPosition
            ? transform.TransformPoint(spawnRange.center)
            : spawnRange.center;

        // 生成怪物
        for (int i = 0; i < actualCount; i++)
        {
            // 获取随机生成位置
            Vector3 spawnPosition = GetRandomPositionInRange(worldCenter);

            // 随机选择一个怪物预制体
            string selectedName = monsterPrefabs[UnityEngine.Random.Range(0, monsterPrefabs.Count)].ToString();

            // 生成怪物
            GameObject monster = LoadManager.Instance.NetInstantiate(selectedName, transform,);
            spawnedMonsters.Add(monster);
        }
    }

    /// <summary>
    /// 在配置的范围内获取随机位置
    /// </summary>
    /// <param name="worldCenter">世界空间中的范围中心点</param>
    /// <returns>随机位置</returns>
    private Vector3 GetRandomPositionInRange(Vector3 worldCenter)
    {
        switch (spawnRange.shape)
        {
            case SpawnShape.Circle:
                return GetRandomPointInCircle(worldCenter);
            case SpawnShape.Rectangle:
                return GetRandomPointInRectangle(worldCenter);
            case SpawnShape.Sphere:
                return GetRandomPointInSphere(worldCenter);
            default:
                return worldCenter;
        }
    }

    /// <summary>
    /// 在圆形范围内获取随机点（2D）
    /// </summary>
    private Vector3 GetRandomPointInCircle(Vector3 center)
    {
        // size.x 作为半径
        float radius = spawnRange.size.x;
        // 在单位圆内生成随机点
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * radius;
        // 返回3D位置（Y轴保持不变）
        return new Vector3(center.x + randomPoint.x, center.y, center.z + randomPoint.y);
    }

    /// <summary>
    /// 在矩形范围内获取随机点（2D）
    /// </summary>
    private Vector3 GetRandomPointInRectangle(Vector3 center)
    {
        // 计算半尺寸
        float halfWidth = spawnRange.size.x / 2;
        float halfDepth = spawnRange.size.z / 2;

        // 在矩形内生成随机点
        float randomX = UnityEngine.Random.Range(-halfWidth, halfWidth);
        float randomZ = UnityEngine.Random.Range(-halfDepth, halfDepth);

        return new Vector3(center.x + randomX, center.y, center.z + randomZ);
    }

    /// <summary>
    /// 在球形范围内获取随机点（3D）
    /// </summary>
    private Vector3 GetRandomPointInSphere(Vector3 center)
    {
        // size.x 作为半径
        float radius = spawnRange.size.x;
        // 在单位球内生成随机点
        Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * radius;
        return center + randomPoint;
    }

    /// <summary>
    /// 清除所有已生成的怪物
    /// </summary>
    public void ClearAllMonsters()
    {
        foreach (var monster in spawnedMonsters)
        {
            if (monster != null)
            {
                Destroy(monster);
            }
        }
        spawnedMonsters.Clear();
    }

    /// <summary>
    /// 移除指定怪物
    /// </summary>
    public void RemoveMonster(GameObject monster)
    {
        if (monster != null && spawnedMonsters.Contains(monster))
        {
            spawnedMonsters.Remove(monster);
            Destroy(monster);
        }
    }

    // 绘制Gizmos以可视化生成范围
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = gizmosColor;

        // 计算世界空间中的范围中心点
        Vector3 worldCenter = spawnRange.useLocalPosition
            ? transform.TransformPoint(spawnRange.center)
            : spawnRange.center;

        // 根据形状绘制不同的Gizmos
        switch (spawnRange.shape)
        {
            case SpawnShape.Circle:
                Gizmos.DrawWireSphere(worldCenter, spawnRange.size.x);
                // 绘制2D平面指示器
                Gizmos.DrawLine(worldCenter, new Vector3(worldCenter.x + spawnRange.size.x, worldCenter.y, worldCenter.z));
                Gizmos.DrawLine(worldCenter, new Vector3(worldCenter.x, worldCenter.y, worldCenter.z + spawnRange.size.x));
                break;
            case SpawnShape.Rectangle:
                Gizmos.DrawWireCube(worldCenter, spawnRange.size);
                break;
            case SpawnShape.Sphere:
                Gizmos.DrawWireSphere(worldCenter, spawnRange.size.x);
                break;
        }
    }
}
