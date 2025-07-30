using UnityEngine;

public interface ITimeService
{
    float DeltaTime { get; }
    float TimeScale { get; set; }
}

public class TimeService : ITimeService
{
    public float DeltaTime => Time.deltaTime;
    public float TimeScale { get; set; } = 1;
}