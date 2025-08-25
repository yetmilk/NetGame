using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utility
{
    public static class ScriptableObjectCloner
    {
        // 复制 ScriptableObject 实例
        public static T Clone<T>(T original) where T : ScriptableObject
        {
            if (original == null)
            {
                Debug.LogError("Original ScriptableObject is null!");
                return null;
            }

            // 创建新实例
            T clone = ScriptableObject.CreateInstance<T>();

            // 获取所有字段（包括私有字段）
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                // 跳过静态字段和常量
                if (field.IsStatic || field.IsLiteral)
                    continue;

                // 复制字段值
                object value = field.GetValue(original);

                // 处理数组的深拷贝
                if (field.FieldType.IsArray && value != null)
                {
                    System.Array originalArray = (System.Array)value;
                    System.Array clonedArray = (System.Array)System.Activator.CreateInstance(field.FieldType, originalArray.Length);
                    originalArray.CopyTo(clonedArray, 0);
                    value = clonedArray;
                }

                field.SetValue(clone, value);
            }

            return clone;
        }
    }

}
