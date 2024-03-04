using Godot;
using System;

public class Level : Node2D
{
    private PackedScene TileScene;
    private TileMap TileMap;
    private Node2D TilesParent;
    private Node2D BuildingsParent;

    public override void _Ready()
    {
        base._Ready();

        TileScene = GD.Load<PackedScene>("res://scenes/Tile.tscn");
        TileMap = GetNode<TileMap>("TileMap");
        TilesParent = GetNode<Node2D>("Tiles");
        BuildingsParent = GetNode<Node2D>("Buildings");

        // convert tilemap blueprint to tile objects (a real level)
        foreach (Vector2 pos in TileMap.GetUsedCells())
        {
            Tile newTile = (Tile) TileScene.Instance();
            newTile.GlobalPosition = TileMap.ToGlobal(TileMap.MapToWorld(pos));
            TilesParent.AddChild(newTile);
        }

        Godot.Collections.Array tiles = TilesParent.GetChildren();
        Godot.Collections.Array buildings = BuildingsParent.GetChildren();
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

            foreach (Building building in buildings)
            {
                if (tile.GlobalPosition == building.GlobalPosition)
                {
                    building.TileOwner = tile;
                    if (building is Consumer)
                        tile.Demand = Consumer.Demand;
                }
            }
        }

        TileMap.Hide();
    }
}
