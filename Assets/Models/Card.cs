using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard
{
    public bool IsRevealed
    {
        get;
    }

    public void Reveal();
    
}

public class Card : ICard
{
    protected bool _revealed;
    public bool IsRevealed
    {
        get
        {
            return _revealed;
        }
    }
    public void Reveal()
    {
        _revealed = true;
    }
}
