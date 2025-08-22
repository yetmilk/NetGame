using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

[CustomPropertyDrawer(typeof(FrameIndexEvent), true)]
public class FrameIndexEventDrawer : PropertyDrawer
{
    private string[] eventNames;
    private bool initialized = false;
    private const float VERTICAL_SPACING = 20f;

    // ��ʼ���¼������б�
    private void Initialize()
    {
        if (initialized) return;

        // ��EventCenter��ȡ�����¼�ID
        Type functionIdType = typeof(FunctionId);
        if (functionIdType == null)
        {
            Debug.LogError("EventCenter type not found.");
            return;
        }


        // ���Ի�ȡEventId�ֶ�
        FieldInfo[] eventIdFields = functionIdType.GetFields(BindingFlags.Public | BindingFlags.Static);
        if (eventIdFields.Length > 0)
        {
            eventNames = new string[eventIdFields.Length];
            for (int i = 0; i < eventIdFields.Length; i++)
            {
                eventNames[i] = (string)eventIdFields[i].GetValue(null);
            }
        }
        else
        {
            Debug.LogError("EventId fields not found in EventCenter.");
            return;
        }


        Debug.Log($"Initialized event names: {string.Join(", ", eventNames)}");
        initialized = true;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Initialize();
        if (eventNames == null || eventNames.Length == 0)
        {
            EditorGUI.LabelField(position, "Event names not initialized");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = position;

        // �����۵�����
        contentPosition.height = EditorGUIUtility.singleLineHeight;
        property.isExpanded = EditorGUI.Foldout(contentPosition, property.isExpanded, label);
        contentPosition.y += contentPosition.height + VERTICAL_SPACING;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            // �����¼��������ڷ�Χ
            DrawPropertyField(ref contentPosition, property, "eventLifeRange", "�¼��������ڷ�Χ");

            // �����¼����������˵�
            DrawEventNameDropdown(ref contentPosition, property);

            // ���¼����Ƹı�ʱ��������Ӧ��EventParam
            if (GUI.changed)
            {
                SerializedProperty eventNameProp = property.FindPropertyRelative("eventName");
                if (eventNameProp != null)
                {
                    CreateEventParam(property, eventNameProp.stringValue);
                }
            }

            // �����¼�������ֱ��ʹ����������Ϊ�ֶ�������ȷ���ֶ�����������һ�£�
            string paramTypeName = GetEventParamTypeName(property);
            if (!string.IsNullOrEmpty(paramTypeName))
            {
                SerializedProperty paramProp = property.FindPropertyRelative(paramTypeName);
                if (paramProp != null)
                {
                    contentPosition.height = GetEventParamHeight(paramProp);
                    EditorGUI.PropertyField(contentPosition, paramProp, new GUIContent("�¼�����"), true);
                    contentPosition.y += contentPosition.height + VERTICAL_SPACING;
                }
                else
                {
                    EditorGUI.LabelField(contentPosition, $"δ�ҵ���������: {paramTypeName}");
                    contentPosition.y += EditorGUIUtility.singleLineHeight + VERTICAL_SPACING;
                }
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    private void DrawPropertyField(ref Rect position, SerializedProperty property, string propName, string label)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        SerializedProperty prop = property.FindPropertyRelative(propName);
        if (prop != null)
        {
            EditorGUI.PropertyField(position, prop, new GUIContent(label));
        }
        else
        {
            EditorGUI.LabelField(position, $"δ�ҵ�����: {propName}");
        }
        position.y += position.height + VERTICAL_SPACING;
    }

    private void DrawEventNameDropdown(ref Rect position, SerializedProperty property)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        SerializedProperty eventNameProp = property.FindPropertyRelative("actionName");
        if (eventNameProp != null)
        {
            int selectedIndex = Array.IndexOf(eventNames, eventNameProp.stringValue);
            if (selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUI.Popup(position, "�¼�����", selectedIndex, eventNames);
            eventNameProp.stringValue = eventNames[selectedIndex];
        }
        else
        {
            EditorGUI.LabelField(position, "δ�ҵ��¼���������");
        }
        position.y += position.height + VERTICAL_SPACING;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!initialized) Initialize();
        if (eventNames == null || eventNames.Length == 0) return EditorGUIUtility.singleLineHeight;

        float height = EditorGUIUtility.singleLineHeight + VERTICAL_SPACING; // �۵�����

        if (property.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight + VERTICAL_SPACING; // �������ڷ�Χ
            height += EditorGUIUtility.singleLineHeight + VERTICAL_SPACING; // �¼����������˵�

            string paramTypeName = GetEventParamTypeName(property);
            if (!string.IsNullOrEmpty(paramTypeName))
            {
                SerializedProperty paramProp = property.FindPropertyRelative(paramTypeName);
                if (paramProp != null)
                {
                    height += GetEventParamHeight(paramProp) + VERTICAL_SPACING;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight + VERTICAL_SPACING;
                }
            }
        }

        return height;
    }

    private float GetEventParamHeight(SerializedProperty paramProp)
    {
        return EditorGUI.GetPropertyHeight(paramProp, true);
    }

    private void CreateEventParam(SerializedProperty property, string eventName)
    {
        Type paramType = FindEventParamType(eventName);
        if (paramType == null)
        {
            Debug.LogWarning($"δ�ҵ��¼� '{eventName}' ��Ӧ��EventParam����");
            return;
        }

        // ֱ��ʹ����������Ϊ�ֶ�������ȷ��FrameIndexEvent�д��ڸ����Ƶ��ֶΣ�
        string paramTypeName = paramType.Name;
        SerializedProperty paramProp = property.FindPropertyRelative(paramTypeName);
        if (paramProp != null)
        {
            // ������ʵ�������л�
            FunctionParam newParam = (FunctionParam)Activator.CreateInstance(paramType);
            string json = JsonUtility.ToJson(newParam);
            JsonUtility.FromJsonOverwrite(json, paramProp.serializedObject.targetObject);
            paramProp.serializedObject.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning($"δ�ҵ���������: {paramTypeName}����ȷ��FrameIndexEvent�д��ڸ��ֶ�");
        }
    }

    private Type FindEventParamType(string eventName)
    {
        Assembly assembly = Assembly.GetAssembly(typeof(FunctionParam));
        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
            if (type.IsSubclassOf(typeof(FunctionParam)) && !type.IsAbstract)
            {
                FunctionParamAttribute attribute = type.GetCustomAttribute<FunctionParamAttribute>();
                if (attribute != null && attribute.FunctionName == eventName)
                {
                    return type;
                }
            }
        }
        return null;
    }

    private string GetEventParamTypeName(SerializedProperty property)
    {
        SerializedProperty eventNameProp = property.FindPropertyRelative("actionName");
        if (eventNameProp != null)
        {
            Type paramType = FindEventParamType(eventNameProp.stringValue);
            if (paramType != null)
            {
                return paramType.Name; // ֱ��ʹ����������Ϊ�ֶ���
            }
        }
        return "";
    }
}