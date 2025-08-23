using System;
using System.Reflection;

public class StringConversionHelper
{
    // ��stringת��ΪĿ�����ͣ���boxedTypeʾ������ָ����
    public static object ConvertStringToType(string input, object boxedTypeExample)
    {
        if (input == null)
            return null;

        // ��ȡĿ������
        Type targetType = boxedTypeExample.GetType();

        // ����ɿ����ͣ���int?��
        if (targetType.IsGenericType &&
            targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            if (string.IsNullOrEmpty(input))
                return null; // ���ַ���ת��Ϊnull

            // ��ȡ�ɿ����͵Ļ������ͣ���int?�Ļ���������int��
            targetType = Nullable.GetUnderlyingType(targetType);
        }

        try
        {
            // ʹ��Convert.ChangeType����ת���������ڴ�����������ͣ�
            return Convert.ChangeType(input, targetType);
        }
        catch (Exception ex)
        {
            // ת��ʧ�ܣ���¼����
            Console.WriteLine($"ת��ʧ��: {ex.Message}");
            return null;
        }
    }

    // ���ط�����ֱ�Ӵ���Ŀ�����ͣ�������ʾ������
    public static object ConvertStringToType(string input, Type targetType)
    {
        if (input == null)
            return null;

        // ����ɿ�����
        if (targetType.IsGenericType &&
            targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            if (string.IsNullOrEmpty(input))
                return null;

            targetType = Nullable.GetUnderlyingType(targetType);
        }

        try
        {
            // ʹ��Convert.ChangeType
            return Convert.ChangeType(input, targetType);
        }
        catch
        {
            // �����������������Զ������͵Ĺ��캯����
            if (TryCreateCustomType(input, targetType, out object result))
                return result;

            return null;
        }
    }

    // ���Դ����Զ������ͣ�ͨ��������ù��캯����
    private static bool TryCreateCustomType(string input, Type targetType, out object result)
    {
        result = null;

        // ����Ƿ��д�string�����Ĺ��캯��
        ConstructorInfo constructor = targetType.GetConstructor(new[] { typeof(string) });
        if (constructor != null)
        {
            try
            {
                result = constructor.Invoke(new object[] { input });
                return true;
            }
            catch
            {
                return false;
            }
        }

        // �����Զ��崴���߼�...
        return false;
    }
}
