using System;

public class ByteArray
{
    // 默认大小
    const int DEFAULT_SIZE = 4096;

    // 初始大小
    int initSize = 0;

    // 缓冲区
    public byte[] bytes;

    // 读写位置
    public int readIdx = 0;
    public int writeIdx = 0;

    // 容量
    private int capacity = 0;

    // 剩余空间（修复：实际可用空间 = 总容量 - 已写入位置）
    public int remain { get { return capacity - writeIdx; } }

    // 数据长度
    public int length { get { return writeIdx - readIdx; } }

    // 构造函数
    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIdx = 0;
        writeIdx = 0;
    }

    // 构造函数
    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }

    // 调整缓冲区大小）
    public void ReSize(int needSize)
    {
        // 关键修复：计算需要的总容量 = 最大(当前写入位置 + 需要的空间, 原有有效数据长度 + 需要的空间)
        int required = Math.Max(writeIdx + needSize, length + needSize);

        // 如果当前容量足够，无需扩容
        if (capacity >= required) return;

        // 计算新容量（取大于等于 required 的最小 2 的幂）
        int newCapacity = capacity;
        while (newCapacity < required)
        {
            newCapacity *= 2;
            if (newCapacity > 1024 * 1024) // 最大1MB
            {
                newCapacity = 1024 * 1024;
                break;
            }
        }

        // 执行扩容：复制所有已写入的数据（从0到writeIdx，包括已读取的部分）
        byte[] newBytes = new byte[newCapacity];
        Array.Copy(bytes, 0, newBytes, 0, writeIdx); // 复制0到writeIdx的所有数据
        bytes = newBytes;
        capacity = newCapacity;

        // 不需要重置readIdx和writeIdx，保持原有位置
        // （整理缓冲区的工作交给CheckAndMoveBytes方法）
    }

    // 检查并移动数据（放宽触发条件，减少碎片）
    public void CheckAndMoveBytes()
    {
        // 当已读取数据超过一半时，整理缓冲区（更合理的触发条件）
        if (readIdx > capacity / 2)
        {
            MoveBytes();
        }
    }

    // 移动数据（将有效数据移到缓冲区头部）
    public void MoveBytes()
    {
        if (length > 0)
        {
            Array.Copy(bytes, readIdx, bytes, 0, length);
        }
        writeIdx = length;
        readIdx = 0;
    }

    // 写入数据
    public int Write(byte[] bs, int offset, int count)
    {
        if (bs == null || count <= 0) return 0;

        // 确保有足够空间
        if (remain < count)
        {
            ReSize(count); // 传入需要的新空间，内部会计算总需求
        }

        Array.Copy(bs, offset, bytes, writeIdx, count);
        writeIdx += count;
        return count;
    }

    // 读取数据
    public int Read(byte[] bs, int offset, int count)
    {
        if (bs == null || count <= 0) return 0;

        count = Math.Min(count, length);
        Array.Copy(bytes, readIdx, bs, offset, count);
        readIdx += count;
        CheckAndMoveBytes();
        return count;
    }

    // 读取Int16（2字节）
    public Int16 ReadInt16()
    {
        if (length < 2) return 0;

        Int16 ret = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        readIdx += 2;
        CheckAndMoveBytes();
        return ret;
    }

    // 读取Int32（4字节，修复位运算错误）
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