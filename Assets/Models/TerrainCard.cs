using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCard
{
    public enum TerrainType { Cliff, River, Field };

    private TerrainType _type;

    public TerrainType Type
    {
        get
        {
            return _type;
        }
    }

    private bool _revealed;

    public bool IsRevealed
    {
        get
        {
            return _revealed;
        }
    }

    public TerrainCard(TerrainType Type)
    {
        _type = Type;
        _revealed = false;
    }

    public override string ToString()
    {
        return _type.ToString();
    }

    public void Reveal()
    {
        _revealed = true;
    }
}
