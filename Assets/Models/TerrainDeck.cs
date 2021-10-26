using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainDeck : Deck
{    
    public TerrainDeck(int numEachTerrains)
    {
        var tempCards = new List<ICard>();

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.Cliff));
        }

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.Field));
        }

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.River));
        }

        _cards = shuffleDeck(tempCards);
    }

    public TerrainDeck(int numEachTerrains, int[] indexArray)
    {
        var tempCards = new List<ICard>();

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.Cliff));
        }

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.Field));
        }

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.River));
        }

        _cards = new List<ICard>();
        for (int i = 0; i < indexArray.Length; i++)
        {
            _cards.Add(tempCards[indexArray[i]]);
        }
    }

    public TerrainDeck(int numEachTerrains, string topCards)
    {
        var topCardsArray = topCards.Split(',');
        
        var tempCards = new List<ICard>();

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.Cliff));
        }

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.Field));
        }

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.River));
        }

        _cards = new List<ICard>();

        foreach(var topCard in topCardsArray)
        {
            bool revealed = false;

            if (topCard.EndsWith("+"))
            {
                revealed = true;
            }

            var searchCard = tempCards.FirstOrDefault(t => t.ToString() == topCard.TrimEnd(new char[] { '+' }));
            if (searchCard != null)
            {
                _cards.Add(searchCard);
                tempCards.Remove(searchCard);

                if (revealed)
                    searchCard.Reveal();
            }
            else
            {
                Debug.Log(topCard + " not found in deck");
            }

            
        }

        _cards.AddRange(shuffleDeck(tempCards));
    }
}
