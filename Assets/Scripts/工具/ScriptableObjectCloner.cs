using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utility
{
    public static class ScriptableObjectCloner
    {
        // ���� ScriptableObject ʵ��
        public static T Clone<T>(T original) where T : ScriptableObject
        {
            if (original == null)
            {
                Debug.LogError("Original ScriptableObject is null!");
                return null;
            }

            // ������ʵ��
            T clone = ScriptableObject.CreateInstance<T>();

            // ��ȡ�����ֶΣ�����˽���ֶΣ�
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                // ������̬�ֶκͳ���
                if (field.IsStatic || field.IsLiteral)
                    continue;

                // �����ֶ�ֵ
                object value = field.GetValue(original);

                // ������������
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
