using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoneycombController : MonoBehaviour
{
    public List<HoneycombCharacter> Characters;
    public SpecialText Text;
    public ParticleSystem Fireworks;
    public FadePanel FadePanel;
    public CharacterType Winner;

    public GameObject FruitsParent;
    public YellowDice YellowDice;
    public List<Fruit> Fruits;
    public List<Fruit> ShownFruits;

    public AudioSource MiniGameAS;
    public AudioSource MusicAS;
    public AudioSource SuccessAS;
    public AudioSource BeesAS;

    public List<AudioClip> MiniGameSounds;

    private SaveData _saveData;
    private int _charIndex;
    private int _fruitAmount;

    void Start()
    {
        _saveData = SaveController.Load();
        SetMainPlayer();
        LoadFruit();
        Text.Show("Ready...", 1.5f);
        Invoke("ShowGoText", 1.5f);
    }

    public void MoveFruit()
    {
        int amount = YellowDice.Amount;
        MoveFruit(amount);
    }

    public void ChangeCharacterTurn(bool isEliminated)
    {
        if (_fruitAmount < YellowDice.Amount - 1 && !isEliminated)
        {
            SuccessAS.Play();
            _fruitAmount++;
            return;
        }

        var curChar = Characters[_charIndex];

        if (isEliminated)
        {
            BeesAS.Play();
            Characters.RemoveAt(_charIndex);
            curChar.Lose();
            curChar.WalkTowards(curChar.transform.position + Vector3.left * 10, null);
            Destroy(curChar, 5);

            if (_charIndex == Characters.Count)
            {
                _charIndex = 0;
            }
        }
        else
        {
            SuccessAS.Play();
            _charIndex = (_charIndex + 1) % Characters.Count;
            curChar.WalkTowards(transform.position + Vector3.right * (Characters.Count * 1.5f), null);
        }

        if (Characters.Count == 1)
        {
            Winner = Characters.First().Type;

            MusicAS.Stop();
            MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Win];
            MiniGameAS.Play();

            Characters.First().Win();
            Text.Show($"{Winner} Wins!", 2f);
            Fireworks.Play();
            Invoke("FadeOut", 2f);
        }
        else
        {
            var newChar = Characters[_charIndex];

            foreach (HoneycombCharacter character in Characters)
            {
                if (character.Type == curChar.Type)
                {
                    continue;
                }

                character.MoveUp(character.Type == newChar.Type);
            }
        }
    }

    public void StartNewTurn()
    {
        var character = Characters[_charIndex];

        character.transform.LookAt(new Vector3(character.transform.position.x, character.transform.position.y, Camera.main.transform.position.z));
        character.CanJump = true;
        YellowDice.StartRotating();
        _fruitAmount = 0;
    }

    public int FruitsToHoneycomb()
    {
        return ShownFruits.FindIndex(s => s.Type == FruitType.Honeycomb) + 1;
    }

    private void ShowGoText()
    {
        MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Go];
        MiniGameAS.Play();
        Text.Show("Go!", 0.5f);

        Characters[_charIndex].CanJump = true;
    }

    private void LoadFruit()
    {
        var fruits = Fruits.FindAll(f => f.Type != FruitType.Honeycomb);
        var honeycomb = Fruits.Find(f => f.Type == FruitType.Honeycomb);

        int honeycombSpot = Random.Range(6, 9);

        for (int i = 0; i < 9; i++)
        {
            var chosenFruit = i == honeycombSpot ? honeycomb : fruits[Random.Range(0, fruits.Count)];
            var fruit = Instantiate(chosenFruit, new Vector3(), chosenFruit.transform.rotation);
            fruit.transform.parent = FruitsParent.transform;
            fruit.transform.localPosition = new Vector3(-2.75f + 0.8f * i, chosenFruit.transform.position.y, -0.15f);
            ShownFruits.Add(fruit);
        }
    }

    private IEnumerator AddNewFruit(int amount)
    {
        yield return new WaitForSeconds(0.5f);

        var fruits = Fruits.FindAll(f => f.Type != FruitType.Honeycomb);
        var honeycomb = Fruits.Find(f => f.Type == FruitType.Honeycomb);

        for (int i = 0; i < ShownFruits.Count; i++)
        {
            var shownFruit = ShownFruits[i];
            shownFruit.transform.localPosition = new Vector3(shownFruit.transform.localPosition.x - 0.8f, shownFruit.transform.localPosition.y, shownFruit.transform.localPosition.z);
        }

        if (Characters.Count > 2 || (Winner == CharacterType.Unknown && !ShownFruits.Any(s => s.Type == FruitType.Honeycomb)))
        {
            yield return new WaitForSeconds(0.1f);

            var chosenFruit = ShownFruits.Any(s => s.Type == FruitType.Honeycomb) ? fruits[Random.Range(0, fruits.Count)] : honeycomb;
            var fruit = Instantiate(chosenFruit, new Vector3(), chosenFruit.transform.rotation);
            fruit.transform.parent = FruitsParent.transform;
            fruit.transform.localPosition = new Vector3(-2.75f + 0.8f * 8, chosenFruit.transform.position.y, -0.15f);
            ShownFruits.Add(fruit);
        }

        if (amount > 0)
        {
            MoveFruit(amount);
        }
    }

    private void MoveFruit(int amount)
    {
        ShownFruits[0].Roll();
        ShownFruits.RemoveAt(0);
        StartCoroutine(AddNewFruit(amount - 1));
    }

    private void FadeOut()
    {
        FadePanel.FadeOut();
        Invoke("ReturnGame", 1f);
    }

    private void ReturnGame()
    {
        if (_saveData != null)
        {
            _saveData.LastWinningCharacters = new List<CharacterType> { Winner };
            _saveData.LastMiniGame = "Honeycomb";
            var winningCharacter = _saveData.Characters.Find(c => c.Type == Winner);
            winningCharacter.Coins += 10;
            winningCharacter.MiniGamesWon++;

            SaveController.Save(_saveData);
        }

        SceneManager.LoadScene("Game");
    }

    private void SetMainPlayer()
    {
        if (_saveData != null)
        {
            foreach (var character in Characters)
            {
                var matchingCharacter = _saveData.Characters.Find(c => c.Type == character.Type);
                character.IsPlayer = matchingCharacter.IsPlayer;
            }
        }
    }
}
