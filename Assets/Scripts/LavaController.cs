using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LavaController : MonoBehaviour
{
    public Spinner Spinner;
    public List<CharacterGame> Characters;
    public SpecialText Text;
    public ParticleSystem Fireworks;
    public FadePanel FadePanel;
    public CharacterType Winner;

    private SaveData _saveData;

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
            CharacterGame winningCharacter = Characters.Find(c => c != null);
            Winner = winningCharacter.Type;
            winningCharacter.Win();
            Text.Show($"{winningCharacter.Type} Wins!", 2f);
            Spinner.Stop();
            Fireworks.Play();
            Invoke("FadeOut", 2);
        }
    }

    private void ShowGoText()
    {
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
            _saveData.LastWinningCharacter = Winner;
            var winningCharacter = _saveData.Characters.Find(c => c.Type == Winner);
            winningCharacter.Coins += 10;

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
