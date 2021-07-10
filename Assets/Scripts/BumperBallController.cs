using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BumperBallController : MonoBehaviour
{
    public List<CharacterFollow> Characters;
    public SpecialText Text;
    public ParticleSystem Fireworks;
    public FadePanel FadePanel;
    public CharacterType Winner;

    private SaveData _saveData;
    private bool _isDraw;

    void Start()
    {
        _saveData = SaveController.Load();
        SetMainPlayer();
        Text.Show("Ready...", 1.5f);
        Invoke("ShowGoText", 1.5f);
    }

    void Update()
    {
        if (Characters.Count(c => c != null) == 1 && Winner == CharacterType.Unknown)
        {
            CharacterFollow winningCharacter = Characters.Find(c => c != null);
            Winner = winningCharacter.CharacterType;
            winningCharacter.Win();
            Text.Show($"{winningCharacter.CharacterType} Wins!", 2f);
            Fireworks.Play();
            Invoke("FadeOut", 2);
        }
        else if (Time.timeSinceLevelLoad >= 30 && !_isDraw)
        {
            _isDraw = true;
            foreach (var character in Characters)
            {
                if (character != null)
                {
                    character.Draw();
                }
            }
            Text.Show($"Draw!", 2f);
            Invoke("FadeOut", 2);
        }
    }

    private void ShowGoText()
    {
        Characters.ForEach(c => c.CanMove = true);
        Text.Show("Go!", 0.5f);
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
            _saveData.LastWinningCharacter = _isDraw ? CharacterType.Unknown : Winner;
            _saveData.LastMiniGame = "Bumper Ball";
            if (Winner != CharacterType.Unknown)
            {
                var winningCharacter = _saveData.Characters.Find(c => c.Type == Winner);
                winningCharacter.Coins += 10;
            }

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
                var matchingCharacter = _saveData.Characters.Find(c => c.Type == character.CharacterType);
                character.IsPlayer = matchingCharacter.IsPlayer;
            }
        }
    }
}
