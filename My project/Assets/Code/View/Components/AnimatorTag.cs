using UnityEngine;
using Zenject;

public class AnimatorTag : ViewComponent
{
    public Animator animator;
    private float _speed;
    private ITimeService _logicTimer;
    private static readonly int UpperBodySpeed = Animator.StringToHash("UpperBodySpeed");

    [Inject]
    public void Construct(ITimeService timer)
    {
        _logicTimer = timer;
    }


    public void SetAnimatorSpeed(float speed)
    {
        _speed = speed;
        ChangeAnimationSpeed();
    }

    private void ChangeAnimationSpeed()
    {
        animator.SetFloat(UpperBodySpeed, _speed * _logicTimer.TimeScale);
    }
}