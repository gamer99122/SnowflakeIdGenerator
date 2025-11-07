# Snowflake ID Generator

一個簡單的雪花演算法 ID 生成器，適用於 C# Windows 主控台應用程式。

## 功能特點

- 生成 64 位元唯一 ID
- ID 遞增排序
- 線程安全
- 每毫秒最多可生成 4096 個 ID
- 無需外部套件依賴

## 環境需求

- .NET 5.0 或更高版本
- C# 9.0 或更高版本（支援 Top-level statements）

## 使用方法
```csharp
// 建立 ID 生成器（workerId: 1, datacenterId: 1）
var idGenerator = new SnowflakeIdGenerator(1, 1);

// 生成 ID
long id = idGenerator.NextId();
Console.WriteLine($"生成的 ID: {id}");
```

## 參數說明

- `workerId`: 機器 ID，範圍 0-31
- `datacenterId`: 資料中心 ID，範圍 0-31

單一應用程式使用時，兩個參數設為 1 即可。

## ID 結構

64 位元 ID 組成：
- 1 位元：保留（固定為 0）
- 41 位元：時間戳
- 5 位元：資料中心 ID
- 5 位元：機器 ID
- 12 位元：序列號

## 執行範例
```
生成的 ID: 1234567890123456789
生成的 ID: 1234567890123456790
生成的 ID: 1234567890123456791
```

---

*本專案由 Claude AI 產生*