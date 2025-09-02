using UnityEngine;
using System;
using System.Collections.Generic;

// 生成范围形状
public enum SpawnShape
{
    Circle,    // 圆形(2D)
    Rectangle, // 矩形(2D)
    Sphere     // 球形(3D)
}

[Serializable]
public class SpawnRangeSettings
{
    public SpawnShape shape;              // 范围形状
    public Vector3 center;                // 范围中心点
    public Vector3 size;                  // 范围大小(不同形状含义不同)
    public float rotationAngle;           // 范围旋转角度(度)，主要用于2D形状
    public bool useLocalPosition = true;  // 是否使用本地位置
}

public class MonsterSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public List<CharacterClacify> monsterPrefabs;  // 怪物预制体列表
    public SpawnRangeSettings spawnRange;    // 生成范围设置
    public int maxMonsters = 10;             // 最大怪物数量

    [Header("调试")]
    public bool drawGizmos = true;           // 是否显示范围gizmos
    public Color gizmosColor = Color.green;  // Gizmos颜色



    /// <summary>
    /// 在指定范围内生成指定数量的怪物
    /// </summary>
    public List<GameObject> SpawnMonsters(int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning("生成数量不能小于等于0");
            return null;
        }

        if (monsterPrefabs == null || monsterPrefabs.Count == 0)
        {
            Debug.LogWarning("没有设置怪物预制体");
            return null;
        }

        int actualCount = count;
        if (actualCount <= 0)
        {
            Debug.LogWarning("已达到最大怪物数量");
            return null;
        }

        Vector3 worldCenter = spawnRange.useLocalPosition
            ? transform.TransformPoint(spawnRange.center)
            : spawnRange.center;
        List<GameObject> spawnedMonsters = new List<GameObject>();

        for (int i = 0; i < actualCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInRange(worldCenter);
            string selectedName = monsterPrefabs[UnityEngine.Random.Range(0, monsterPrefabs.Count)].ToString();
            GameObject monster = LoadManager.Instance.NetInstantiate(selectedName);
            monster.transform.position = spawnPosition;
            monster.transform.rotation = Quaternion.identity;
            spawnedMonsters.Add(monster);
        }

        return spawnedMonsters;
    }

    /// <summary>
    /// 在指定范围内获取随机位置
    /// </summary>
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
    /// 在圆形范围内获取随机点(2D)
    /// </summary>
    private Vector3 GetRandomPointInCircle(Vector3 center)
    {
        float radius = spawnRange.size.x;
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * radius;

        // 如果有旋转角度，应用旋转
        if (spawnRange.rotationAngle != 0)
        {
            float rad = spawnRange.rotationAngle * Mathf.Deg2Rad;
            float rotatedX = randomPoint.x * Mathf.Cos(rad) - randomPoint.y * Mathf.Sin(rad);
            float rotatedY = randomPoint.x * Mathf.Sin(rad) + randomPoint.y * Mathf.Cos(rad);
            randomPoint = new Vector2(rotatedX, rotatedY);
        }

        return new Vector3(center.x + randomPoint.x, center.y, center.z + randomPoint.y);
    }

    /// <summary>
    /// 在矩形范围内获取随机点(2D)
    /// </summary>
    private Vector3 GetRandomPointInRectangle(Vector3 center)
    {
        float halfWidth = spawnRange.size.x / 2;
        float halfDepth = spawnRange.size.z / 2;

        float randomX = UnityEngine.Random.Range(-halfWidth, halfWidth);
        float randomZ = UnityEngine.Random.Range(-halfDepth, halfDepth);

        // 如果有旋转角度，应用旋转
        if (spawnRange.rotationAngle != 0)
        {
            float rad = spawnRange.rotationAngle * Mathf.Deg2Rad;
            float rotatedX = randomX * Mathf.Cos(rad) - randomZ * Mathf.Sin(rad);
            float rotatedZ = randomX * Mathf.Sin(rad) + randomZ * Mathf.Cos(rad);
            randomX = rotatedX;
            randomZ = rotatedZ;
        }

        return new Vector3(center.x + randomX, center.y, center.z + randomZ);
    }

    /// <summary>
    /// 在球形范围内获取随机点(3D)
    /// </summary>
    private Vector3 GetRandomPointInSphere(Vector3 center)
    {
        float radius = spawnRange.size.x;
        Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * radius;
        return center + randomPoint;
    }



    // 绘制Gizmos以可视化生成范围
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = gizmosColor;
        Vector3 worldCenter = spawnRange.useLocalPosition
            ? transform.TransformPoint(spawnRange.center)
            : spawnRange.center;

        // 保存当前矩阵状态
        Matrix4x4 originalMatrix = Gizmos.matrix;

        // 如果有旋转角度，应用旋转矩阵
        if (spawnRange.rotationAngle != 0 && spawnRange.shape != SpawnShape.Sphere)
        {
            Gizmos.matrix = Matrix4x4.TRS(worldCenter, Quaternion.Euler(0, spawnRange.rotationAngle, 0), Vector3.one);
            worldCenter = Vector3.zero; // 旋转后使用本地坐标
        }

        switch (spawnRange.shape)
        {
            case SpawnShape.Circle:
                Gizmos.DrawWireSphere(worldCenter, spawnRange.size.x);
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

        // 恢复原始矩阵
        Gizmos.matrix = originalMatrix;
    }
}