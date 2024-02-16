using Godot;
using System;

public class Producer : Node2D
{
    [Export]
    public Tile TileOwner;
    private float ProduceTimer = 0.0f;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        ProduceTimer += delta;
        if (ProduceTimer > Clock.ProducerTick)
        {
            TileOwner.Supply += 1.0f;
            ProduceTimer = 0.0f;
        }
    }
}
