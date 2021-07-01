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
    public List<GameObject> Dice;
    public GameObject Coin;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        ChooseCharacter();
    }

    public void CharacterLanded()
    {
        if (Turn == -1)
        {
            NextCharacter();
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

    private void ChooseCharacter()
    {
        Character character = Characters[CharIndex];
        character.CanJump = true;
        if (Characters[CharIndex].IsPlayer)
        {
            Dialog.ShowText("Press space to hit the dice block");
        }
        else
        {
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
            Dialog.ShowText("Each player starts with 5 coins", AddStartingCoins);
            return;
        }

        if (CharIndex == 0)
        {
            TurnText.DisplayTurn(Turn + 1);
        }

        Dialog.ShowText($"It's {Characters[CharIndex].Type}'s {(CharIndex == 0 && Turn == 0 ? "turn first" : "turn")}!");

        Turn++;
    }

    private int GetPlace(CharacterType type)
    {
        var orderedCharacters = Characters.OrderByDescending(c => c.Stars).ThenByDescending(c => c.Coins).ToList();
        int rank = 1;
        int currentStars = orderedCharacters[0].Stars;
        int currentCoins = orderedCharacters[0].Coins;
        foreach (Character character in orderedCharacters)
        {
            if (character.Type == type)
            {
                return rank;
            }
            if (character.Stars != currentStars)
            {
                currentStars = character.Stars;
                rank++;
            }
            else if  (character.Coins != currentCoins)
            {
                currentCoins = character.Coins;
                rank++;
            }
        }

        return 0;
    }

    private void AddStartingCoins()
    {
        Turn++;
        foreach (GameObject die in Dice)
        {
            die.SetActive(false);
        }
        foreach (Character character in Characters)
        {
            Instantiate(Coin, character.gameObject.transform.position + Vector3.up * 3, Quaternion.identity);
        }
        Invoke("DoTurn", 1.5f);
    }
}
