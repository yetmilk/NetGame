using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utility
{
    public static class ObjectCloner
    {
        // 克隆ScriptableObject（保留原功能）
        public static T Clone<T>(T original) where T : ScriptableObject
        {
            if (original == null)
            {
                Debug.LogError("Original ScriptableObject is null!");
                return null;
            }

            T clone = ScriptableObject.CreateInstance<T>();
            CopyFields(original, clone);
            return clone;
        }

        // 克隆普通类（新增通用深拷贝方法）
        public static T CloneObject<T>(T original) where T : class, new()
        {
            if (original == null)
            {
                Debug.LogError("Original object is null!");
                return null;
            }

            T clone = new T();
            CopyFields(original, clone);
            return clone;
        }

        // 核心字段复制逻辑（递归处理引用类型）
        private static void CopyFields(object source, object target)
        {
            if (source == null || target == null) return;

            Type type = source.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                // 跳过静态字段和常量
                if (field.IsStatic || field.IsLiteral)
                    continue;

                object value = field.GetValue(source);
                if (value == null)
                {
                    field.SetValue(target, null);
                    continue;
                }

                // 处理数组
                if (field.FieldType.IsArray)
                {
                    Array originalArray = (Array)value;
                    Array clonedArray = (Array)Activator.CreateInstance(field.FieldType, originalArray.Length);

                    // 递归复制数组元素
                    for (int i = 0; i < originalArray.Length; i++)
                    {
                        object element = originalArray.GetValue(i);
                        clonedArray.SetValue(CloneElement(element), i);
                    }

                    field.SetValue(target, clonedArray);
                }
                // 处理值类型（直接复制）
                else if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                {
                    field.SetValue(target, value);
                }
                // 处理引用类型（递归克隆）
                else
                {
                    object clonedValue = CloneElement(value);
                    field.SetValue(target, clonedValue);
                }
            }
        }

        // 克隆单个元素（区分值类型和引用类型）
        private static object CloneElement(object element)
        {
            if (element == null) return null;

            Type elementType = element.GetType();

            // 值类型或字符串直接返回（值类型会自动复制）
            if (elementType.IsValueType || elementType == typeof(string))
            {
                return element;
            }

            // 引用类型：如果有默认构造函数则创建新实例并复制字段
            ConstructorInfo ctor = elementType.GetConstructor(Type.EmptyTypes);
            if (ctor != null)
            {
                object cloned = Activator.CreateInstance(elementType);
                CopyFields(element, cloned);
                return cloned;
            }

            // 不支持无参构造函数的类型（如某些系统类）
            Debug.LogWarning($"不支持克隆类型 {elementType.Name}（缺少无参构造函数）");
            return element;
        }
    }
}