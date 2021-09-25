using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCard : Card, ICard
{
    //do I want to combine PlayType and TerrainType into one enum? not sure
    public enum PlayType { Militia, TradingPost, Farm, CityWall, Bandits, Raiders };

    private PlayType _type;
    public PlayType Type
    {
        get
        {
            return _type;
        }
    }

    public PlayCard(PlayType Type)
    {
        _type = Type;
        _revealed = false;
    }
    public override string ToString()
    {
        return _type.ToString();
    }
}
