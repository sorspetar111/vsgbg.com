using System.Runtime.Serialization;

public enum TimePeriod
{
    [EnumMember(Value = "1w")]
    OneWeek,
    [EnumMember(Value = "1d")]
    OneDay,
    [EnumMember(Value = "30m")]
    ThirtyMinutes,
    [EnumMember(Value = "5m")]
    FiveMinutes,
    [EnumMember(Value = "1m")]
    OneMinute
}
