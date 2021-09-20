using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCard
{
    public enum PlayType { Militia, TradingPost, Farm, CityWall, Bandits, Raiders };

    private PlayType _type;
    public PlayType Type
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

    public PlayCard(PlayType Type)
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
