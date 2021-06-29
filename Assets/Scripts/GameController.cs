using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public List<Character> Characters;
    public int CharIndex;
    public bool GameStart;

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

        }
        else if (CharIndex == Characters.Count - 1)
        {
            Characters = Characters.OrderByDescending(c => c.Roll).ToList();
            GameStart = true;
            CharIndex = 0;
        }
        else
        {
            CharIndex++;
            ChooseCharacter();
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
}
