using Godot;
using System;

public class Consumer : Node2D
{
    [Export]
    public Tile TileOwner;
    public const float Demand = 2.0f;
    private float ConsumeTimer = 0.0f;
    private float Consumed = 0.0f;
    private Label ConsumeLabel;

    public override void _Ready()
    {
        base._Ready();
        ConsumeLabel = GetNode<Label>("Sprite/Label");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        ConsumeTimer += delta;
        if (ConsumeTimer > Clock.ConsumerTick)
        {
            if (TileOwner.Supply >= Demand)
            {
                TileOwner.Supply -= Demand;
                Consumed += Demand;
                GD.Print("Consume");
                ConsumeLabel.Text = Consumed.ToString();
            }
            ConsumeTimer = 0.0f;
        }
    }
}
