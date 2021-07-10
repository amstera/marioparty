using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterChooserController : MonoBehaviour
{
    public SpriteRenderer LeftTriangle;
    public SpriteRenderer RightTriangle;
    public List<CharacterChoose> Characters;
    public CharacterChoose ChosenCharacter;
    public ParticleSystem Fireworks;

    private int _charIndex;

    void Start()
    {
        PlayerPrefs.DeleteAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) && _charIndex > 0)
        {
            _charIndex--;
            SetActiveChar();
            RightTriangle.enabled = true;
            if (_charIndex == 0)
            {
                LeftTriangle.enabled = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) && _charIndex < 3)
        {
            _charIndex++;
            SetActiveChar();
            LeftTriangle.enabled = true;
            if (_charIndex == 3)
            {
                RightTriangle.enabled = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            List<CompressedCharacter> compressedCharacters = new List<CompressedCharacter>();
            foreach (var character in Characters)
            {
                compressedCharacters.Add(
                    new CompressedCharacter
                {
                        Type = character.Type,
                        IsPlayer = character.IsPlayer
                });
            }

            SaveData saveData = new SaveData
            {
                Characters = compressedCharacters
            };
            SaveController.Save(saveData);

            ChosenCharacter.GetComponentInChildren<Animator>().SetTrigger("Victory");
            Fireworks.Play();

            Invoke("LoadGame", 1f);
        }
    }

    private void LoadGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    private void SetActiveChar()
    {
        foreach (var character in Characters)
        {
            character.gameObject.SetActive(false);
            character.IsPlayer = false;
        }

        var chosenCharacter = Characters[_charIndex];
        chosenCharacter.gameObject.SetActive(true);
        chosenCharacter.IsPlayer = true;
        ChosenCharacter = chosenCharacter;
    }
}
