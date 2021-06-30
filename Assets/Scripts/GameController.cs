using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public List<Character> Characters;
    public int CharIndex;
    public int ReloadIndex;
    public int Turn = -1;
    public bool GameStart;

    public List<CharacterStat> CharacterStats;
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

    public void NextCharacter()
    {
        if (GameStart)
        {
            DoTurn();
        }
        else if (CharIndex == Characters.Count - 1)
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

    public void LoadAllCharacterStats(bool reloadDelay, bool moveGame = true)
    {
        if (ReloadIndex == Characters.Count)
        {
            ReloadIndex = 0;
            GameStart = true;
            if (moveGame)
            {
                NextCharacter();
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
        if (Characters[CharIndex].IsPlayer)
        {
            character.CanJump = true;
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
            foreach (GameObject die in Dice)
            {
                die.SetActive(false);
            }
            foreach (Character character in Characters)
            {
                Instantiate(Coin, character.gameObject.transform.position + Vector3.up * 3, Quaternion.identity);
            }
            Turn++;
            DoTurn();
        }
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
}
