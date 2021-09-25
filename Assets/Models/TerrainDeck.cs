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
}
