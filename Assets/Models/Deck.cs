using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    protected List<ICard> _cards;

    protected List<ICard> shuffleDeck(List<ICard> cards)
    {
        System.Random rand = new System.Random();

        List<ICard> shuffledCards = new List<ICard>();

        while (cards.Count > 0)
        {
            int randomIndex = (int)(rand.NextDouble() * cards.Count);

            shuffledCards.Add(cards[randomIndex]);

            cards.RemoveAt(randomIndex);
        }

        return shuffledCards;
    }

    public ICard DrawCard()
    {
        var card = _cards[0];

        _cards.RemoveAt(0);

        return card;
    }

    public List<ICard> DrawCards(int num)
    {
        var cards = _cards.GetRange(0, num);

        _cards.RemoveRange(0, num);

        return cards;
    }

    public override string ToString()
    {
        string output = "";

        foreach (var card in _cards)
        {
            output += card.ToString() + ", ";
        }

        return output;
    }
}
