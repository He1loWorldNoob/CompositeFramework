using System;

public class LogicTimer
{
    private const int MaxGameSequence = 60;
    private const float FixedDeltaTime = 1.0f / FPS;
    private const int FPS = 60;
    
    private readonly ITimeService _timeService;
    
    public ushort Tick;
    public float LerpAlpha;
    

    private double _accumulator;

    public LogicTimer(ITimeService timeService)
    {
        _timeService = timeService;
    }
        
    public void ResetTick(ushort tick = 0)
    {
        Tick = tick;
        _accumulator = 0.0;
    }

    public void TimerTick(Action<float> onLogicUpdate)
    {
        _accumulator += _timeService.DeltaTime * _timeService.TimeScale;
        
        while (_accumulator >= FixedDeltaTime)
        {
            Tick = (ushort)((Tick + 1) % MaxGameSequence);
            LerpAlpha = (float)_accumulator / FixedDeltaTime;
            _accumulator -= FixedDeltaTime;
            onLogicUpdate?.Invoke(FixedDeltaTime);
        }
    }

}

