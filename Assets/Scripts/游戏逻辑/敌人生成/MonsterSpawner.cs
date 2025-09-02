using UnityEngine;
using System;
using System.Collections.Generic;

// ���ɷ�Χ��״����
public enum SpawnShape
{
    Circle,    // Բ�Σ�2D��
    Rectangle, // ���Σ�2D��
    Sphere     // ���Σ�3D��
}

[Serializable]
public class SpawnRangeSettings
{
    public SpawnShape shape;              // ��Χ��״
    public Vector3 center;                // ��Χ���ĵ�
    public Vector3 size;                  // ��Χ��С��������״�в�ͬ���壩
    public bool useLocalPosition = true;  // �Ƿ�ʹ�ñ�������
}

public class MonsterSpawner : NetMonobehavior
{
    [Header("��������")]
    public List<CharacterClacify> monsterPrefabs;  // ����Ԥ�����б�
    public SpawnRangeSettings spawnRange;    // ���ɷ�Χ����
    public int maxMonsters = 10;             // ��������������

    [Header("����")]
    public bool drawGizmos = true;           // �Ƿ���Ʒ�Χ gizmos
    public Color gizmosColor = Color.green;  // Gizmos ��ɫ

    private List<GameObject> spawnedMonsters = new List<GameObject>();  // �����ɵĹ���

    /// <summary>
    /// ��ָ����Χ������ָ�������Ĺ���
    /// </summary>
    /// <param name="count">Ҫ���ɵĹ�������</param>
    public void SpawnMonsters(int count)
    {
        // �������Ϸ���
        if (count <= 0)
        {
            Debug.LogWarning("���������������0");
            return;
        }

        if (monsterPrefabs == null || monsterPrefabs.Count == 0)
        {
            Debug.LogWarning("û�����ù���Ԥ����");
            return;
        }

        // ����ʵ�������ɵ�������������������ƣ�
        int actualCount = Mathf.Min(count, maxMonsters - spawnedMonsters.Count);
        if (actualCount <= 0)
        {
            Debug.LogWarning("�Ѵﵽ��������������");
            return;
        }

        // ��ȡ����ռ��еķ�Χ���ĵ�
        Vector3 worldCenter = spawnRange.useLocalPosition
            ? transform.TransformPoint(spawnRange.center)
            : spawnRange.center;

        // ���ɹ���
        for (int i = 0; i < actualCount; i++)
        {
            // ��ȡ�������λ��
            Vector3 spawnPosition = GetRandomPositionInRange(worldCenter);

            // ���ѡ��һ������Ԥ����
            string selectedName = monsterPrefabs[UnityEngine.Random.Range(0, monsterPrefabs.Count)].ToString();

            // ���ɹ���
            GameObject monster = LoadManager.Instance.NetInstantiate(selectedName, transform,);
            spawnedMonsters.Add(monster);
        }
    }

    /// <summary>
    /// �����õķ�Χ�ڻ�ȡ���λ��
    /// </summary>
    /// <param name="worldCenter">����ռ��еķ�Χ���ĵ�</param>
    /// <returns>���λ��</returns>
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
    /// ��Բ�η�Χ�ڻ�ȡ����㣨2D��
    /// </summary>
    private Vector3 GetRandomPointInCircle(Vector3 center)
    {
        // size.x ��Ϊ�뾶
        float radius = spawnRange.size.x;
        // �ڵ�λԲ�����������
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * radius;
        // ����3Dλ�ã�Y�ᱣ�ֲ��䣩
        return new Vector3(center.x + randomPoint.x, center.y, center.z + randomPoint.y);
    }

    /// <summary>
    /// �ھ��η�Χ�ڻ�ȡ����㣨2D��
    /// </summary>
    private Vector3 GetRandomPointInRectangle(Vector3 center)
    {
        // �����ߴ�
        float halfWidth = spawnRange.size.x / 2;
        float halfDepth = spawnRange.size.z / 2;

        // �ھ��������������
        float randomX = UnityEngine.Random.Range(-halfWidth, halfWidth);
        float randomZ = UnityEngine.Random.Range(-halfDepth, halfDepth);

        return new Vector3(center.x + randomX, center.y, center.z + randomZ);
    }

    /// <summary>
    /// �����η�Χ�ڻ�ȡ����㣨3D��
    /// </summary>
    private Vector3 GetRandomPointInSphere(Vector3 center)
    {
        // size.x ��Ϊ�뾶
        float radius = spawnRange.size.x;
        // �ڵ�λ�������������
        Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * radius;
        return center + randomPoint;
    }

    /// <summary>
    /// ������������ɵĹ���
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
    /// �Ƴ�ָ������
    /// </summary>
    public void RemoveMonster(GameObject monster)
    {
        if (monster != null && spawnedMonsters.Contains(monster))
        {
            spawnedMonsters.Remove(monster);
            Destroy(monster);
        }
    }

    // ����Gizmos�Կ��ӻ����ɷ�Χ
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = gizmosColor;

        // ��������ռ��еķ�Χ���ĵ�
        Vector3 worldCenter = spawnRange.useLocalPosition
            ? transform.TransformPoint(spawnRange.center)
            : spawnRange.center;

        // ������״���Ʋ�ͬ��Gizmos
        switch (spawnRange.shape)
        {
            case SpawnShape.Circle:
                Gizmos.DrawWireSphere(worldCenter, spawnRange.size.x);
                // ����2Dƽ��ָʾ��
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
