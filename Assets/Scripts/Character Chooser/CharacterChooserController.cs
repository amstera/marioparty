using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterChooserController : MonoBehaviour
{
    public SpriteRenderer LeftTriangle;
    public SpriteRenderer RightTriangle;
    public List<CharacterChoose> Characters;
    public CharacterChoose ChosenCharacter;
    public TextMeshProUGUI TotalTurnsText;
    public ParticleSystem Fireworks;
    public QuitPanel QuitPanel;
    public FadePanel FadePanel;
    public int TotalTurns = 20;

    public AudioSource SelectSound;

    private int _charIndex;
    private int _turnIndex = 1;

    void Start()
    {
        SaveData saveData = SaveController.Load();
        if (saveData != null)
        {
            if (saveData.Turn >= TotalTurns)
            {
                PlayerPrefs.DeleteAll();
            }
            else
            {
                SceneManager.LoadSceneAsync("Game");
            }
        }
        

        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && _charIndex > 0)
        {
            SelectSound.Play();
            _charIndex--;
            SetActiveChar();
            RightTriangle.enabled = true;
            if (_charIndex == 0)
            {
                LeftTriangle.enabled = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) && _charIndex < 3)
        {
            SelectSound.Play();
            _charIndex++;
            SetActiveChar();
            LeftTriangle.enabled = true;
            if (_charIndex == 3)
            {
                RightTriangle.enabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && _turnIndex > 0)
        {
            SelectSound.Play();
            _turnIndex--;
            TotalTurns -= 5;
            UpdateTotalTurns();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && _turnIndex < 3)
        {
            SelectSound.Play(); 
            _turnIndex++;
            TotalTurns += 5;
            UpdateTotalTurns();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !QuitPanel.gameObject.activeSelf)
        {
            QuitPanel.Show();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectSound.Play();

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
                Characters = compressedCharacters,
                TotalTurns = TotalTurns
            };
            SaveController.Save(saveData);

            ChosenCharacter.Win();
            Fireworks.Play();

            Invoke("FadeOut", 1f);
        }
    }

    private void FadeOut()
    {
        FadePanel.FadeOut();
        Invoke("LoadGame", 1f);
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

    private void UpdateTotalTurns()
    {
        TotalTurnsText.text = $"Total Turns: {TotalTurns}";
        if (TotalTurns == 20)
        {
            TotalTurnsText.text += " (Recommended)";
        }
    }
}
