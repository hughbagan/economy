using Godot;
using System;

public class Building : Node2D
{
    [Export]
    public Tile TileOwner;

    public override void _Ready()
    {
        base._Ready();
    }
}
