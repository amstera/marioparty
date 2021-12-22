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
    public GameObject StartText;
    public CharacterType Winner;

    public AudioSource MiniGameAS;
    public AudioSource MusicAS;

    public List<AudioClip> MiniGameSounds;

    private SaveData _saveData;
    private bool _isDraw;
    private bool _gameStarted;

    void Awake()
    {
        SetCharacterPositions();
    }

    void Start()
    {
        _saveData = SaveController.Load();
        SetMainPlayer();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_gameStarted)
        {
            _gameStarted = true;
            StartText.SetActive(false);
            Text.Show("Ready...", 1.5f);
            Invoke("ShowGoText", 1.5f);
        }

        if (Characters.Count(c => c != null) == 1 && Winner == CharacterType.Unknown && !_isDraw)
        {
            CharacterFollow winningCharacter = Characters.Find(c => c != null);
            Winner = winningCharacter.CharacterType;
            MusicAS.Stop();
            MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Win];
            MiniGameAS.Play();
            winningCharacter.Win();
            Text.Show($"{winningCharacter.CharacterType} Wins!", 2f);
            Fireworks.Play();
            Invoke("FadeOut", 2);
        }
        else if ((Characters.Count(c => c != null) == 0 || Time.timeSinceLevelLoad >= 30) && Winner == CharacterType.Unknown && !_isDraw)
        {
            _isDraw = true;
            foreach (var character in Characters)
            {
                if (character != null)
                {
                    character.Draw();
                }
            }
            MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Tie];
            MiniGameAS.Play();
            Text.Show($"Draw!", 2f);
            Invoke("FadeOut", 2);
        }
    }

    private void ShowGoText()
    {
        MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Go];
        MiniGameAS.Play();
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
            _saveData.LastWinningCharacters = new List<CharacterType> { _isDraw ? CharacterType.Unknown : Winner };
            _saveData.LastMiniGame = "Bumper Ball";
            if (Winner != CharacterType.Unknown)
            {
                var winningCharacter = _saveData.Characters.Find(c => c.Type == Winner);
                winningCharacter.Coins += 10;
                winningCharacter.MiniGamesWon++;
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

    private void SetCharacterPositions()
    {
        var positions = Characters.Select(c => c.transform.position).ToList();
        positions.Shuffle();
        for (int i = 0; i < Characters.Count; i++)
        {
            var character = Characters[i];
            character.transform.position = new Vector3(positions[i].x, character.transform.position.y, positions[i].z);
        }
    }
}
