using Godot;
using System;

public class Clock : Node
{
    public const float Tick = 0.1f;
    public static float TileTick = Tick*5.0f; // ratio to Tick
    public static float ProducerTick = Tick*10.0f;
    public static float ConsumerTick = Tick*10.0f;

    public override void _Ready()
    {
        base._Ready();
    }

}
