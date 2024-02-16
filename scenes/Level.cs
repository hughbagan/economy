using Godot;
using System;

public class Level : Node2D
{
    private PackedScene TileScene;
    private TileMap TileMap;
    private Node2D TilesParent;

    private Producer TestProducer;
    private Consumer TestConsumer;

    public override void _Ready()
    {
        base._Ready();

        TileScene = GD.Load<PackedScene>("res://scenes/Tile.tscn");
        TileMap = GetNode<TileMap>("TileMap");
        TilesParent = GetNode<Node2D>("Tiles");

        TestProducer = GetNode<Producer>("Producer");
        TestConsumer = GetNode<Consumer>("Consumer");

        // convert tilemap blueprint to tile objects (a real level)
        foreach (Vector2 pos in TileMap.GetUsedCells())
        {
            Tile newTile = (Tile) TileScene.Instance();
            newTile.GlobalPosition = TileMap.ToGlobal(TileMap.MapToWorld(pos));
            TilesParent.AddChild(newTile);
        }

        Godot.Collections.Array tiles = TilesParent.GetChildren();
        float tileSize = TileMap.CellSize.x;
        foreach (Tile tile in tiles)
        {
            // set tile neighbours
            foreach (Tile anotherTile in tiles)
            {
                if (tile == anotherTile)
                    continue;
                if (tile.GlobalPosition.y == anotherTile.GlobalPosition.y)
                {
                    float xdiff = tile.GlobalPosition.x - anotherTile.GlobalPosition.x;
                    if (xdiff == -tileSize)
                        tile.Neighbours[Global.Cardinal.East] = anotherTile;
                    else if (xdiff == tileSize)
                        tile.Neighbours[Global.Cardinal.West] = anotherTile;
                }
                else if (tile.GlobalPosition.x == anotherTile.GlobalPosition.x)
                {
                    float ydiff = tile.GlobalPosition.y - anotherTile.GlobalPosition.y;
                    if (ydiff == -tileSize)
                        tile.Neighbours[Global.Cardinal.South] = anotherTile;
                    else if (ydiff == tileSize)
                        tile.Neighbours[Global.Cardinal.North] = anotherTile;
                }
            }

            if (tile.GlobalPosition == TestProducer.GlobalPosition)
                TestProducer.TileOwner = tile;
            if (tile.GlobalPosition == TestConsumer.GlobalPosition)
            {
                TestConsumer.TileOwner = tile;
                tile.Demand = Consumer.Demand;
            }
        }

        TileMap.Hide();
    }
}
