using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCard : Card, ICard
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

    public TerrainCard(TerrainType Type)
    {
        _type = Type;
        _revealed = false;
    }

    public override string ToString()
    {
        return _type.ToString();
    }
}
