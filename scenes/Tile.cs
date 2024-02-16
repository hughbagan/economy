using Godot;
using System;
using System.Collections.Generic;

public class Tile : Node2D
{
    [Export]
    public Dictionary<Global.Cardinal, Tile> Neighbours = new Dictionary<Global.Cardinal, Tile>();

    // TODO: dict w enums for every good
    [Export]
    public float Supply = 0.0f; // amount of loads here
    public float NextSupply = 0.0f; // supply of the next tick
    [Export]
    public float Price = 0.0f;
    [Export]
    public float Demand = 0.0f;

    private float TickTimer = 0.0f;
    private Label TestLabel;
    private Dictionary<ulong, bool> SeenRipples = new Dictionary<ulong, bool>();
    private Sprite SupplySprite;

    public override void _Ready()
    {
        base._Ready();

        TestLabel = GetNode<Label>("Label");
        SupplySprite = GetNode<Sprite>("Supply");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        TickTimer += delta;
        if (TickTimer > Clock.TileTick)
        {
            // Adjust price to demand
            float diffMagnitude;
            if (Supply == 0.0f)
                diffMagnitude = Demand / 0.4f; // FIXME
            else
                diffMagnitude = Demand / Supply;

            if (Demand > Supply)
            {
                Price += (float) Math.Round(diffMagnitude, 2);
                foreach (Tile tile in Neighbours.Values)
                {
                    ulong rippleStamp = Time.GetTicksMsec();
                    SeenRipples[rippleStamp] = true;
                    tile.AdjustPriceNeighbours(this, rippleStamp);
                }
            }
            else if (Demand < Supply)
            {
                if (Price > 0.0f)
                {
                    Price -= (float) Math.Round(diffMagnitude, 2);
                    if (Price < 0.0f)
                        Price = 0.0f;
                }
            }

            // Move Supply to a neighbour based on price
            // (only works assuming tiles' Process is called in order)
            if (NextSupply < Supply)
                NextSupply = Supply;
            if (Supply > 0.0f)
            {
                Tile maxTile = this;
                foreach (Tile tile in Neighbours.Values)
                {
                    if (tile.Price > maxTile.Price)
                        maxTile = tile;
                }
                if (maxTile.Price > 0.0f && maxTile != this)
                {
                    Supply -= 0.1f;
                    maxTile.NextSupply = (float) Math.Round(maxTile.Supply + 0.1f, 1);
                }
            }

            Supply = NextSupply;
            TickTimer = 0.0f;
        }

        // test graphics
        if (Price > 0.0f)
            Modulate = new Color(0.0f, Price, 0.0f);
        else
            Modulate = new Color(1.0f, 1.0f, 1.0f);
        if (Supply > 0.0f)
            SupplySprite.Show();
        else
            SupplySprite.Hide();

        TestLabel.Text = "S "+Supply.ToString()+"\nD "+Demand.ToString()+"\nP "+Price.ToString();
    }

    public void AdjustPriceNeighbours(Tile from, ulong rippleStamp)
    {
        // Adjust price to be the average of the Tile's Neighbours
        if (SeenRipples.ContainsKey(rippleStamp))
            return;
        SeenRipples[rippleStamp] = true;
        float total = 0.0f;
        int neighbours = 0;
        foreach (Tile tile in Neighbours.Values)
        {
            total += tile.Price;
            ++neighbours;
        }
        Price = Mathf.Round(total / (float) neighbours);
        if (Price == 0.0f)
            return;
        // Keep propogating to the neighbours
        foreach (Tile tile in Neighbours.Values)
        {
            if (tile == from)
                continue;
            tile.AdjustPriceNeighbours(this, rippleStamp);
        }
    }
}
