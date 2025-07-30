using System;
using UnityEngine;

[Serializable]
public class MoveSpeedTag : ICmsComponentDefinition
{
    public float speed;
}

public class MoveComponent : IObjectComponent
{
    public Vector3 direction;
    public bool IsMoving;
}


public class MovementSystem : BaseInteraction, ILogicUpdateObjectModule
{
    public void OnLogicUpdateObject(ObjectState state, float dt)
    {
        if (!state.Is<MoveComponent>(out var move) || !move.IsMoving) return;

        var transform = state.Get<TransformComponent>();
        var speed = state.Model.Get<MoveSpeedTag>().speed;

        var dir = move.direction.normalized;
        float step = speed * dt;

        transform.position += dir * step;
    }
}
