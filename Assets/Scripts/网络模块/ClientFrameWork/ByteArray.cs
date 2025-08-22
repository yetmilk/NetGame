using System;

public class ByteArray
{
    // Ĭ�ϴ�С
    const int DEFAULT_SIZE = 4096;

    // ��ʼ��С
    int initSize = 0;

    // ������
    public byte[] bytes;

    // ��дλ��
    public int readIdx = 0;
    public int writeIdx = 0;

    // ����
    private int capacity = 0;

    // ʣ��ռ䣨�޸���ʵ�ʿ��ÿռ� = ������ - ��д��λ�ã�
    public int remain { get { return capacity - writeIdx; } }

    // ���ݳ���
    public int length { get { return writeIdx - readIdx; } }

    // ���캯��
    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIdx = 0;
        writeIdx = 0;
    }

    // ���캯��
    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }

    // ������������С��
    public void ReSize(int needSize)
    {
        // �ؼ��޸���������Ҫ�������� = ���(��ǰд��λ�� + ��Ҫ�Ŀռ�, ԭ����Ч���ݳ��� + ��Ҫ�Ŀռ�)
        int required = Math.Max(writeIdx + needSize, length + needSize);

        // �����ǰ�����㹻����������
        if (capacity >= required) return;

        // ������������ȡ���ڵ��� required ����С 2 ���ݣ�
        int newCapacity = capacity;
        while (newCapacity < required)
        {
            newCapacity *= 2;
            if (newCapacity > 1024 * 1024) // ���1MB
            {
                newCapacity = 1024 * 1024;
                break;
            }
        }

        // ִ�����ݣ�����������д������ݣ���0��writeIdx�������Ѷ�ȡ�Ĳ��֣�
        byte[] newBytes = new byte[newCapacity];
        Array.Copy(bytes, 0, newBytes, 0, writeIdx); // ����0��writeIdx����������
        bytes = newBytes;
        capacity = newCapacity;

        // ����Ҫ����readIdx��writeIdx������ԭ��λ��
        // �����������Ĺ�������CheckAndMoveBytes������
    }

    // ��鲢�ƶ����ݣ��ſ���������������Ƭ��
    public void CheckAndMoveBytes()
    {
        // ���Ѷ�ȡ���ݳ���һ��ʱ������������������Ĵ���������
        if (readIdx > capacity / 2)
        {
            MoveBytes();
        }
    }

    // �ƶ����ݣ�����Ч�����Ƶ�������ͷ����
    public void MoveBytes()
    {
        if (length > 0)
        {
            Array.Copy(bytes, readIdx, bytes, 0, length);
        }
        writeIdx = length;
        readIdx = 0;
    }

    // д������
    public int Write(byte[] bs, int offset, int count)
    {
        if (bs == null || count <= 0) return 0;

        // ȷ�����㹻�ռ�
        if (remain < count)
        {
            ReSize(count); // ������Ҫ���¿ռ䣬�ڲ������������
        }

        Array.Copy(bs, offset, bytes, writeIdx, count);
        writeIdx += count;
        return count;
    }

    // ��ȡ����
    public int Read(byte[] bs, int offset, int count)
    {
        if (bs == null || count <= 0) return 0;

        count = Math.Min(count, length);
        Array.Copy(bytes, readIdx, bs, offset, count);
        readIdx += count;
        CheckAndMoveBytes();
        return count;
    }

    // ��ȡInt16��2�ֽڣ�
    public Int16 ReadInt16()
    {
        if (length < 2) return 0;

        Int16 ret = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        readIdx += 2;
        CheckAndMoveBytes();
        return ret;
    }

    // ��ȡInt32��4�ֽڣ��޸�λ�������
    public Int32 ReadInt32()
    {
        if (length < 4) return 0;

        Int32 ret = (Int32)(
            (bytes[readIdx + 3] << 24) |
            (bytes[readIdx + 2] << 16) |
            (bytes[readIdx + 1] << 8) |
            bytes[readIdx]
        );
        readIdx += 4;
        CheckAndMoveBytes();
        return ret;
    }
}