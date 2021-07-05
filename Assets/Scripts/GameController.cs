using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public List<Character> Characters;
    public List<Circle> Spaces;
    public int CharIndex;
    public int ReloadIndex;
    public int Turn = -1;

    public List<CharacterStat> CharacterStats;
    public Dialog Dialog;
    public TurnText TurnText;
    public List<Dice> Dice;
    public GameObject Coin;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        ChooseCharacter();
    }

    public void CharacterLanded(Character character)
    {
        if (Turn == -1)
        {
            NextCharacter();
        }
        else
        {
            Move(character);
        }
    }

    public void NextCharacter()
    {
        if (Turn >= 0)
        {
            return;
        }
        if (CharIndex == Characters.Count - 1)
        {
            Characters = Characters.OrderByDescending(c => c.Roll).ToList();
            CharIndex = 0;
            LoadAllCharacterStats(true);
        }
        else
        {
            CharIndex++;
            ChooseCharacter();
        }
    }

    public void LoadAllCharacterStats(bool reloadDelay)
    {
        if (ReloadIndex == Characters.Count)
        {
            ReloadIndex = 0;
            if (Turn == -1)
            {
                DoTurn();
            }
            return;
        }

        var charStat = CharacterStats[ReloadIndex];
        var character = Characters[ReloadIndex];
        int place = GetPlace(character.Type);
        charStat.Reload(character, place);

        if (!charStat.gameObject.activeSelf)
        {
            charStat.gameObject.SetActive(true);
        }

        ReloadIndex++;
        if (reloadDelay)
        {
            Invoke("LoadAllCharacterStatsDelay", 0.5f);
        }
        else
        {
            LoadAllCharacterStats(false);
        }
    }

    public void ReachedSpace(Character character)
    {
        //hide die
        Dice[(int)character.Type - 1].gameObject.SetActive(false);

        //make space do its action
        var space = Spaces[character.Position];
        if (space.Type == CircleType.Positive || space.Type == CircleType.Negative)
        {
            character.ChangeCoins(space.Type == CircleType.Positive ? 3 : -3);
            Invoke("NextTurn", 1f);
        }
    }

    public Character GetCurrentCharacter()
    {
        if (Turn == -1)
        {
            return null;
        }

        return Characters[CharIndex];
    }

    private void ChooseCharacter()
    {
        Character character = Characters[CharIndex];
        if (Characters[CharIndex].IsPlayer)
        {
            Dialog.ShowText("Press space to hit the dice block", CharacterJump);
        }
        else
        {
            character.CanJump = true;
            character.Jump();
        }
    }

    private void LoadAllCharacterStatsDelay()
    {
        LoadAllCharacterStats(true);
    }

    private void DoTurn()
    {
        if (Turn == -1)
        {
            Dialog.ShowText($"{Characters[CharIndex].Type} goes first!", AddCoinsText);
            return;
        }

        if (CharIndex == 0)
        {
            TurnText.DisplayTurn(Turn + 1);
        }

        Dialog.ShowText($"{Characters[CharIndex].Type}, start!", SetUpDice);
    }

    private void SetUpDice()
    {
        Character character = Characters[CharIndex];
        Vector3 characterPosition = character.transform.position;
        GameObject die = Dice[(int)character.Type - 1].gameObject;

        die.transform.position = new Vector3(characterPosition.x, die.transform.position.y, characterPosition.z);
        die.SetActive(true);
        ChooseCharacter();
    }

    private void Move(Character character)
    {
        int roll = character.Roll;
        int originalPos = character.Position;
        character.Position = (originalPos + roll) % Spaces.Count;
        if (Turn == 0)
        {
            character.Position--;
        }

        Queue<Vector3> spacePositions = new Queue<Vector3>();
        for (int i = originalPos; i <= (character.Position < originalPos ? Spaces.Count - 1 : character.Position); i++)
        {
            Vector3 spacePosition = Spaces[i].transform.position;
            var pos = new Vector3(spacePosition.x, character.transform.position.y, spacePosition.z);
            spacePositions.Enqueue(pos);
        }
        if (character.Position < originalPos)
        {
            for (int i = 0; i <= character.Position; i++)
            {
                Vector3 spacePosition = Spaces[i].transform.position;
                var pos = new Vector3(spacePosition.x, character.transform.position.y, spacePosition.z);
                spacePositions.Enqueue(pos);
            }
        }

        //walk towards this space
        character.WalkTowards(spacePositions);
    }

    private int GetPlace(CharacterType type)
    {
        var orderedCharacters = Characters.OrderByDescending(c => c.Stars).ThenByDescending(c => c.Coins).ToList();
        int rank = 1;
        int currentStars = orderedCharacters[0].Stars;
        int currentCoins = orderedCharacters[0].Coins;
        int amountSinceLastIncrement = 0;
        foreach (Character character in orderedCharacters)
        {
            if (character.Stars != currentStars)
            {
                currentStars = character.Stars;
                rank += amountSinceLastIncrement;
                amountSinceLastIncrement = 1;
            }
            else if  (character.Coins != currentCoins)
            {
                currentCoins = character.Coins;
                rank += amountSinceLastIncrement;
                amountSinceLastIncrement = 1;
            }
            else
            {
                amountSinceLastIncrement++;
            }

            if (character.Type == type)
            {
                return rank;
            }
        }

        return 0;
    }

    private void AddCoinsText()
    {
        Dialog.ShowText("Each player starts with 5 coins", AddStartingCoins);
    }

    private void AddStartingCoins()
    {
        Turn++;
        foreach (Dice die in Dice)
        {
            die.gameObject.SetActive(false);
        }
        foreach (Character character in Characters)
        {
            Instantiate(Coin, character.gameObject.transform.position + Vector3.up * 3, Quaternion.identity);
        }
        Invoke("DoTurn", 1.5f);
    }

    private void NextTurn()
    {
        //update char index and turn
        CharIndex++;
        if (CharIndex >= Characters.Count)
        {
            CharIndex = 0;
            Turn++;
        }

        DoTurn();
    }

    private void CharacterJump()
    {
        Character character = Characters[CharIndex];
        character.CanJump = true;
        character.Jump();
    }
}
