using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    TerrainArea playerTerrains;
    TerrainArea aiTerrains;
    PlayHand playerHand;
    PlayHand aiHand;

    PlayArea playerArea;
    PlayArea aiArea;

    ScorePile playerScorePile;
    ScorePile aiScorePile;

    AIStrat aiStrat;

    const int firstRoundTurns = 5;
    const int drawFirstRound = 8;

    const int secondRoundTurns = 3;
    const int drawSecondRound = 2;

    public Sprite CliffSmallSprite;
    public Sprite RiverSmallSprite;
    public Sprite FieldSmallSprite;

    public Sprite MilitiaSmallSprite;
    public Sprite TradingPostSmallSprite;
    public Sprite FarmSmallSprite;

    public Sprite CityWallsSmallSprite;
    public Sprite BanditsSmallSprite;
    public Sprite RaidersSmallSprite;

    public MainCard OriginalCard;

    private int turnCounter;
    private int roundCounter;
    private int turnsInCurrentRound;

    private List<MainCard> playerTerrainsGraphics;
    private List<MainCard> aiTerrainsGraphics;
    private List<MainCard> playerHandGraphics;
    private List<MainCard> aiHandGraphics;
    private List<MainCard> playerAreaGraphics;
    private List<MainCard> aiAreaGraphics;

    Action<MainCard> cbCardPlayedFromHand;

    private PlayDeck pDeck;
    private TerrainDeck tDeck;

    GameObject scoreTextAI;
    GameObject scoreTextPlayer;

    GameObject cardTitleDisplay;
    GameObject cardDescriptionDisplay;

    GameObject commandDisplay;
    GameObject turnDisplay;
    GameObject roundDisplay;

    GameObject nextButton;
    ButtonScript nextButtonScript;

    // Start is called before the first frame update
    void Start()
    {
        scoreTextAI = GameObject.Find("ScoreTextAI");
        scoreTextAI.SetActive(false);
        scoreTextPlayer = GameObject.Find("ScoreTextPlayer");
        scoreTextPlayer.SetActive(false);

        cardTitleDisplay = GameObject.Find("CardTitleDisplay");
        cardTitleDisplay.SetActive(false);
        cardDescriptionDisplay = GameObject.Find("CardDescriptionDisplay");
        cardDescriptionDisplay.SetActive(false);

        commandDisplay = GameObject.Find("CommandDisplay");
        turnDisplay = GameObject.Find("TurnDisplay");
        roundDisplay = GameObject.Find("RoundDisplay");

        nextButton = GameObject.Find("NextButton");
        nextButtonScript = nextButton.GetComponent<ButtonScript>();
        nextButton.SetActive(false);

        SetUpGameStart();
    }

    void SetUpGameStart()
    {
        playerTerrainsGraphics = new List<MainCard>();
        aiTerrainsGraphics = new List<MainCard>();
        playerHandGraphics = new List<MainCard>();
        aiHandGraphics = new List<MainCard>();
        playerAreaGraphics = new List<MainCard>();
        aiAreaGraphics = new List<MainCard>();

        turnCounter = 0;
        roundCounter = 0;
        turnsInCurrentRound = firstRoundTurns;

        var turnDisplayText = turnDisplay.GetComponent<TextMesh>();
        turnDisplayText.text = "Turn " + "1/" + turnsInCurrentRound;

        var roundDisplayText = roundDisplay.GetComponent<TextMesh>();
        roundDisplayText.text = "Round 1/2";

        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Play card from your hand";

        aiStrat = new AIStrat();

        tDeck = new TerrainDeck(5);

        playerTerrains = new TerrainArea(true);

        playerTerrains.Cards.AddRange(tDeck.DrawCards(3));

        InitTerrain(playerTerrains, true);

        aiTerrains = new TerrainArea(false);

        aiTerrains.Cards.AddRange(tDeck.DrawCards(3));

        InitTerrain(aiTerrains, false);

        pDeck = new PlayDeck(5);

        playerHand = new PlayHand(true);

        playerHand.Cards.AddRange(pDeck.DrawCards(drawFirstRound));

        InitHand(playerHand, true);

        aiHand = new PlayHand(false);

        aiHand.Cards.AddRange(pDeck.DrawCards(drawFirstRound));

        InitHand(aiHand, false);

        playerArea = new PlayArea(true);
        aiArea = new PlayArea(false);

        playerScorePile = new ScorePile();
        aiScorePile = new ScorePile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitHand(PlayHand hand, bool player)
    {
        for (int i = 0; i < hand.Cards.Count; i++)
        {
            MainCard card = Instantiate(OriginalCard) as MainCard;
            if (player)
            {
                card.name = "PlayerHand" + (i + 1);
                card.transform.position = new Vector3(-6 + (1.75f * i), -4f, 0);
                card.FaceDown = false;
                card.VisibleToPlayer = true;
                card.Selectable = true;
                card.CardSelected = PlayCardFromHand;
                playerHandGraphics.Add(card);
            }
            else
            {
                card.name = "AiHand" + (i + 1);
                card.transform.position = new Vector3(-6 + (1.75f * i), 4f, 0);
                card.FaceDown = true;
                card.VisibleToPlayer = false;
                card.Selectable = false;
                aiHandGraphics.Add(card);
            }

            SpriteRenderer hand_sr = card.GetComponent<SpriteRenderer>();
            card.CardTitleDisplay = cardTitleDisplay;
            card.CardDescriptionDisplay = cardDescriptionDisplay;

            if (hand.Cards[i].Type == PlayCard.PlayType.Militia)
            {
                hand_sr.sprite = MilitiaSmallSprite;
                card.CardTitle = "Militia";
                card.CardDescription = "1 point + 1 for each cliff";
            }
            else if (hand.Cards[i].Type == PlayCard.PlayType.TradingPost)
            {
                hand_sr.sprite = TradingPostSmallSprite;
                card.CardTitle = "Trading Post";
                card.CardDescription = "1 point + 1 for each river";
            }
            else if (hand.Cards[i].Type == PlayCard.PlayType.Farm)
            {
                hand_sr.sprite = FarmSmallSprite;
                card.CardTitle = "Farm";
                card.CardDescription = "1 point + 1 for each field";
            }
            else if (hand.Cards[i].Type == PlayCard.PlayType.CityWall)
            {
                hand_sr.sprite = CityWallsSmallSprite;
                card.CardTitle = "City Wall";
                card.CardDescription = "1 point and counters 1 militia";
            }
            else if (hand.Cards[i].Type == PlayCard.PlayType.Bandits)
            {
                hand_sr.sprite = BanditsSmallSprite;
                card.CardTitle = "Bandits";
                card.CardDescription = "1 point and counters 1 trading post";
            }
            else
            {
                hand_sr.sprite = RaidersSmallSprite;
                card.CardTitle = "Raiders";
                card.CardDescription = "1 point and counters 1 farm";
            }
        }
    }

    void InitTerrain(TerrainArea terrain, bool player)
    {
        for (int i = 0; i < terrain.Cards.Count; i++)
        {
            MainCard card = Instantiate(OriginalCard) as MainCard;
            if (player)
            {
                card.name = "PlayerTerrain" + (i + 1);
                card.transform.position = new Vector3(-2 + 2 * i, -2.5f, 0);
                card.FaceDown = true;
                card.VisibleToPlayer = true;
                playerTerrainsGraphics.Add(card);
            }
            else
            {
                card.name = "AITerrain" + (i + 1);
                card.transform.position = new Vector3(-2 + 2 * i, 2.5f, 0);
                card.FaceDown = true;
                card.VisibleToPlayer = false;
                card.CardSelected = RevealACard;
                aiTerrainsGraphics.Add(card);
            }

            card.CardTitleDisplay = cardTitleDisplay;

            SpriteRenderer terrain_sr = card.GetComponent<SpriteRenderer>();

            if (terrain.Cards[i].Type == TerrainCard.TerrainType.Cliff)
            {
                terrain_sr.sprite = CliffSmallSprite;
                card.CardTitle = "Cliff";
            }
            else if (terrain.Cards[i].Type == TerrainCard.TerrainType.River)
            {
                terrain_sr.sprite = RiverSmallSprite;
                card.CardTitle = "River";
            }
            else
            {
                terrain_sr.sprite = FieldSmallSprite;
                card.CardTitle = "Field";
            }
        }
    }

    public void PlayCardFromHand(MainCard card)
    {
        //get index of selected card
        int index = playerHandGraphics.IndexOf(card);

        // move card to play area
        card.transform.position = new Vector3(-6 + (1.75f * turnCounter), -1f, 0);
        card.FaceDown = true;
        card.Selectable = false;

        //move reference in list
        playerHandGraphics.Remove(card);
        playerAreaGraphics.Add(card);

        //move card in data
        playerArea.Cards.Add(playerHand.SelectCard(index));

        //ai plays card
        var aiIndex = aiStrat.GetNextPlayPick(aiHand, aiTerrains, aiArea, playerArea, playerTerrains, turnCounter - (firstRoundTurns - 1));

        //get card reference
        var aiCard = aiHandGraphics[aiIndex];

        //move card to play area
        aiCard.transform.position = new Vector3(-6 + (1.75f * turnCounter), 1f, 0);

        //move reference in list
        aiHandGraphics.Remove(aiCard);
        aiAreaGraphics.Add(aiCard);
        aiCard.CardSelected = RevealACard;

        //move card in data
        aiArea.Cards.Add(aiHand.SelectCard(aiIndex));

        if (turnCounter < turnsInCurrentRound - 1)
        {
            //make player hand not selectable
            foreach (var cardGraphic in playerHandGraphics)
                cardGraphic.Selectable = false;

            //make aiArea and aiTerrain selectable
            foreach (var cardGraphic in aiAreaGraphics)
                cardGraphic.Selectable = true;

            foreach (var cardGraphic in aiTerrainsGraphics)
                cardGraphic.Selectable = true;

            var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
            commandDisplayText.text = "Select opponent card to reveal";
        }
        else
        {
            //reveal all play cards
            for (int j = 0; j < playerArea.Cards.Count; j++)
            {
                playerArea.Cards[j].Reveal();
                playerAreaGraphics[j].FaceDown = false;
                aiArea.Cards[j].Reveal();
                aiAreaGraphics[j].FaceDown = false;
            }

            if (roundCounter == 1)
            {
                //reveal all terrains
                for (int j = 0; j < playerTerrains.Cards.Count; j++)
                {
                    playerTerrains.Cards[j].Reveal();
                    playerTerrainsGraphics[j].FaceDown = false;
                    aiTerrains.Cards[j].Reveal();
                    aiTerrainsGraphics[j].FaceDown = false;
                }
            }

            nextButtonScript.ButtonClicked = Proceed;
            nextButton.SetActive(true);
            var nextButtonText = nextButton.GetComponentInChildren(typeof(TextMesh));
            ((TextMesh)nextButtonText).text = "Next";

            var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
            commandDisplayText.text = "Click next to apply counters";
        }
    }

    public void RevealACard(MainCard card)
    {
        //get index of selected card
        if(aiTerrainsGraphics.Contains(card))
        {
            //get index of selected card
            int index = aiTerrainsGraphics.IndexOf(card);

            // reveal the card in the data
            aiTerrains.Cards[index].Reveal();
        }
        else //it was from the play area
        {
            //get index of selected card
            int index = aiAreaGraphics.IndexOf(card);

            //reveal the card in the data
            aiArea.Cards[index].Reveal();
        }

        //reveal the graphics
        card.FaceDown = false;

        //ai reveals a card
        var revealIndex = aiStrat.GetNextRevealPick(playerArea, playerTerrains);

        //reveal the card in data and in graphics
        if (revealIndex < playerArea.Cards.Count)
        {
            playerArea.Cards[revealIndex].Reveal();
            playerAreaGraphics[revealIndex].FaceDown = false;
        }
        else
        {
            playerTerrains.Cards[revealIndex - playerArea.Cards.Count].Reveal();
            playerTerrainsGraphics[revealIndex - playerArea.Cards.Count].FaceDown = false;
        }

        //make player hand not selectable
        foreach (var cardGraphic in playerHandGraphics)
            cardGraphic.Selectable = true;

        //make aiArea and aiTerrain selectable
        foreach (var cardGraphic in aiAreaGraphics)
            cardGraphic.Selectable = false;

        foreach (var cardGraphic in aiTerrainsGraphics)
            cardGraphic.Selectable = false;

        turnCounter++;
        var turnDisplayText = turnDisplay.GetComponent<TextMesh>();
        turnDisplayText.text = "Turn " + (turnCounter + 1) + "/" + turnsInCurrentRound;

        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Play card from your hand";
    }

    public void applyCounters(PlayArea area1, PlayArea area2, List<MainCard> area2Graphics)
    {
        //loop player area and find any counters
        for (int i = 0; i < area1.Cards.Count; i++)
        {
            if ((area1.Cards[i].Type == PlayCard.PlayType.CityWall) ||
                (area1.Cards[i].Type == PlayCard.PlayType.Bandits) ||
                (area1.Cards[i].Type == PlayCard.PlayType.Raiders))
            {
                //when counter found then loop ai area for the card it counters
                for (int j = 0; j < area2.Cards.Count; j++)
                {
                    //if you find it remove it
                    if ((area1.Cards[i].Type == PlayCard.PlayType.CityWall) && (area2.Cards[j].Type == PlayCard.PlayType.Militia))
                    {
                        area2.Cards.RemoveAt(j);
                        var card = area2Graphics[j];
                        area2Graphics.RemoveAt(j);
                        Destroy(card.gameObject);
                        break;
                    }
                    if ((area1.Cards[i].Type == PlayCard.PlayType.Bandits) && (area2.Cards[j].Type == PlayCard.PlayType.TradingPost))
                    {
                        area2.Cards.RemoveAt(j);
                        var card = area2Graphics[j];
                        area2Graphics.RemoveAt(j);
                        Destroy(card.gameObject);
                        break;
                    }
                    if ((area1.Cards[i].Type == PlayCard.PlayType.Raiders) && (area2.Cards[j].Type == PlayCard.PlayType.Farm))
                    {
                        area2.Cards.RemoveAt(j);
                        var card = area2Graphics[j];
                        area2Graphics.RemoveAt(j);
                        Destroy(card.gameObject);
                        break;
                    }
                }
            }
        }
    }

    public void Proceed(ButtonScript button)
    {
        applyCounters(playerArea, aiArea, aiAreaGraphics);
        applyCounters(aiArea, playerArea, playerAreaGraphics);

        if (roundCounter == 0)
        {
            nextButtonScript.ButtonClicked = SetUpNextTurn;
            var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
            commandDisplayText.text = "Click next to start next round";
        }
        else
        {
            nextButtonScript.ButtonClicked = FinishGame;
            var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
            commandDisplayText.text = "Click next to calculate score";
        }
    }

    public void SetUpNextTurn(ButtonScript button)
    {
        turnCounter = 0;
        roundCounter = 1;
        turnsInCurrentRound = secondRoundTurns;

        var turnDisplayText = turnDisplay.GetComponent<TextMesh>();
        turnDisplayText.text = "Turn " + "1/" + turnsInCurrentRound;

        var roundDisplayText = roundDisplay.GetComponent<TextMesh>();
        roundDisplayText.text = "Round 2/2";

        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Play card from your hand";

        nextButton.SetActive(false);

        //move card data to score pile
        aiScorePile.Cards.AddRange(aiArea.Cards);
        aiArea.Cards.Clear();

        playerScorePile.Cards.AddRange(playerArea.Cards);
        playerArea.Cards.Clear();

        //clear play area graphics
        foreach (var graphic in aiAreaGraphics)
            Destroy(graphic.gameObject);
        aiAreaGraphics = new List<MainCard>();

        foreach (var graphic in playerAreaGraphics)
            Destroy(graphic.gameObject);
        playerAreaGraphics = new List<MainCard>();

        //draw cards for second round
        playerHand.Cards.AddRange(pDeck.DrawCards(drawSecondRound));
        aiHand.Cards.AddRange(pDeck.DrawCards(drawSecondRound));

        //refresh hands
        foreach (var graphic in aiHandGraphics)
            Destroy(graphic.gameObject);
        aiHandGraphics = new List<MainCard>();

        InitHand(aiHand, false);

        foreach (var graphic in playerHandGraphics)
            Destroy(graphic.gameObject);
        playerHandGraphics = new List<MainCard>();

        InitHand(playerHand, true);
    }

    public void FinishGame(ButtonScript button)
    {
        turnCounter = 0;
        roundCounter = 2;

        //move card data to score pile
        aiScorePile.Cards.AddRange(aiArea.Cards);
        aiArea.Cards.Clear();

        playerScorePile.Cards.AddRange(playerArea.Cards);
        playerArea.Cards.Clear();

        //clear play area graphics
        foreach (var graphic in aiAreaGraphics)
            Destroy(graphic.gameObject);
        aiAreaGraphics = new List<MainCard>();

        foreach (var graphic in playerAreaGraphics)
            Destroy(graphic.gameObject);
        playerAreaGraphics = new List<MainCard>();

        //show score pile and scores
        ShowScorePile(aiScorePile, false);
        ShowScorePile(playerScorePile, true);

        //var scoreTextAI = GameObject.Find("ScoreTextAI");
        //var scoreTextPlayer = GameObject.Find("ScoreTextPlayer");

        var scoreTextAIObject = scoreTextAI.GetComponent<TextMesh>();
        scoreTextAIObject.text = "Points: " + aiScorePile.CountScore(aiTerrains).ToString();
        scoreTextAI.SetActive(true);

        var scoreTextPlayerObject = scoreTextPlayer.GetComponent<TextMesh>();
        scoreTextPlayerObject.text = "Points: " + playerScorePile.CountScore(playerTerrains).ToString();
        scoreTextPlayer.SetActive(true);

        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Click restart to play again";

        var nextButtonText = nextButton.GetComponentInChildren(typeof(TextMesh));
        ((TextMesh)nextButtonText).text = "Restart";
        nextButtonScript.ButtonClicked = RestartGame;
    }

    public void RestartGame(ButtonScript button)
    {
        nextButton.SetActive(false);
        scoreTextAI.SetActive(false);
        scoreTextPlayer.SetActive(false);

        aiScorePile.Cards.Clear();
        playerScorePile.Cards.Clear();

        //clear play area graphics
        foreach (var graphic in aiAreaGraphics)
            Destroy(graphic.gameObject);
        aiAreaGraphics = new List<MainCard>();

        foreach (var graphic in playerAreaGraphics)
            Destroy(graphic.gameObject);
        playerAreaGraphics = new List<MainCard>();

        //clear terrain area graphics
        foreach (var graphic in aiTerrainsGraphics)
            Destroy(graphic.gameObject);
        aiTerrainsGraphics = new List<MainCard>();

        foreach (var graphic in playerTerrainsGraphics)
            Destroy(graphic.gameObject);
        playerTerrainsGraphics = new List<MainCard>();

        //clear hand area graphics
        foreach (var graphic in aiHandGraphics)
            Destroy(graphic.gameObject);
        aiHandGraphics = new List<MainCard>();

        foreach (var graphic in playerHandGraphics)
            Destroy(graphic.gameObject);
        playerHandGraphics = new List<MainCard>();

        SetUpGameStart();
    }

    public void ShowScorePile(ScorePile pile, bool player)
    {
        for (int i = 0; i < pile.Cards.Count; i++)
        {
            MainCard card = Instantiate(OriginalCard) as MainCard;
            if (player)
            {
                card.name = "PlayerScorePile" + (i + 1);
                card.transform.position = new Vector3(-6 + (1.75f * i), -1f, 0);
                card.FaceDown = false;
                card.VisibleToPlayer = true;
                playerAreaGraphics.Add(card);
            }
            else
            {
                card.name = "AIScorePile" + (i + 1);
                card.transform.position = new Vector3(-6 + (1.75f * i), 1f, 0);
                card.FaceDown = false;
                card.VisibleToPlayer = true;
                aiAreaGraphics.Add(card);
            }

            SpriteRenderer pile_sr = card.GetComponent<SpriteRenderer>();
            card.CardTitleDisplay = cardTitleDisplay;
            card.CardDescriptionDisplay = cardDescriptionDisplay;

            if (pile.Cards[i].Type == PlayCard.PlayType.Militia)
            {
                pile_sr.sprite = MilitiaSmallSprite;
                card.CardTitle = "Militia";
                card.CardDescription = "1 point + 1 for each cliff";
            }
            else if (pile.Cards[i].Type == PlayCard.PlayType.TradingPost)
            {
                pile_sr.sprite = TradingPostSmallSprite;
                card.CardTitle = "Trading Post";
                card.CardDescription = "1 point + 1 for each river";
            }
            else if (pile.Cards[i].Type == PlayCard.PlayType.Farm)
            {
                pile_sr.sprite = FarmSmallSprite;
                card.CardTitle = "Farm";
                card.CardDescription = "1 point + 1 for each field";
            }
            else if (pile.Cards[i].Type == PlayCard.PlayType.CityWall)
            {
                pile_sr.sprite = CityWallsSmallSprite;
                card.CardTitle = "City Wall";
                card.CardDescription = "1 point and counters 1 militia";
            }
            else if (pile.Cards[i].Type == PlayCard.PlayType.Bandits)
            {
                pile_sr.sprite = BanditsSmallSprite;
                card.CardTitle = "Bandits";
                card.CardDescription = "1 point and counters 1 trading post";
            }
            else
            {
                pile_sr.sprite = RaidersSmallSprite;
                card.CardTitle = "Raiders";
                card.CardDescription = "1 point and counters 1 farm";
            }
        }
    }
}
