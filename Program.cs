using System;

// Top-level statements - 直接開始執行
var idGenerator = new SnowflakeIdGenerator(1, 1);

// 生成 10 個 ID 作為示範
for (int i = 0; i < 10; i++)
{
    long id = idGenerator.NextId();
    Console.WriteLine($"生成的 ID: {id}");
}

Console.ReadLine();

// 類別定義放在後面
public class SnowflakeIdGenerator
{
    private readonly long _workerId;
    private readonly long _datacenterId;
    private long _sequence = 0L;
    private long _lastTimestamp = -1L;

    // 時間戳佔 41 位
    private const int TimestampBits = 41;
    // 資料中心 ID 佔 5 位
    private const int DatacenterIdBits = 5;
    // 機器 ID 佔 5 位
    private const int WorkerIdBits = 5;
    // 序列號佔 12 位
    private const int SequenceBits = 12;

    // 最大值
    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
    private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
    private const long MaxSequence = -1L ^ (-1L << SequenceBits);

    // 位移量
    private const int WorkerIdShift = SequenceBits;
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

    // 起始時間戳 (2020-01-01)
    private const long Epoch = 1577836800000L;

    private readonly object _lock = new object();

    public SnowflakeIdGenerator(long workerId, long datacenterId)
    {
        if (workerId > MaxWorkerId || workerId < 0)
            throw new ArgumentException($"Worker ID 必須在 0 到 {MaxWorkerId} 之間");

        if (datacenterId > MaxDatacenterId || datacenterId < 0)
            throw new ArgumentException($"Datacenter ID 必須在 0 到 {MaxDatacenterId} 之間");

        _workerId = workerId;
        _datacenterId = datacenterId;
    }

    public long NextId()
    {
        lock (_lock)
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp < _lastTimestamp)
                throw new Exception("時鐘回撥，拒絕生成 ID");

            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & MaxSequence;
                if (_sequence == 0)
                {
                    timestamp = WaitNextMillis(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0L;
            }

            _lastTimestamp = timestamp;

            return ((timestamp - Epoch) << TimestampLeftShift)
                   | (_datacenterId << DatacenterIdShift)
                   | (_workerId << WorkerIdShift)
                   | _sequence;
        }
    }

    private long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private long WaitNextMillis(long lastTimestamp)
    {
        long timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }
}