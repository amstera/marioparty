using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShyGuyController : MonoBehaviour
{
    public List<FlagCharacter> Characters;
    public ShyGuy ShyGuy;
    public SpecialText Text;
    public ParticleSystem Fireworks;
    public FadePanel FadePanel;
    public CharacterType Winner;
    public FlagType FlagType;

    public AudioSource MiniGameAS;
    public AudioSource MusicAS;

    public List<AudioClip> MiniGameSounds;

    private SaveData _saveData;
    private bool _isDraw;
    private float _timeSinceFlagShown;
    private float _timeBetweenTurns = 3f;

    void Start()
    {
        _saveData = SaveController.Load();
        SetMainPlayer();
        Text.Show("Ready...", 1.5f);
        Invoke("ShowGoText", 1.5f);

        foreach (FlagCharacter character in Characters)
        {
            character.ShyGuy = ShyGuy;
        }
    }

    void Update()
    {
        if (FlagType != FlagType.None && (Time.time - _timeSinceFlagShown > _timeBetweenTurns || Characters.All(c => c.Flag != FlagType.None)))
        {
            bool eliminatedCharacter = false;
            foreach (FlagCharacter character in Characters)
            {
                if (character == null)
                {
                    continue;
                }
                character.CanChangeFlag = false;
                _timeBetweenTurns = Mathf.Clamp(_timeBetweenTurns - 0.1f, 1f, 3f);
                if (character.Flag != FlagType)
                {
                    eliminatedCharacter = true;
                    character.Eliminate();
                }
            }

            Characters.RemoveAll(c => c.Eliminated);

            FlagType = FlagType.None;

            if (Characters.Count <= 1)
            {
                if (Characters.Count == 1)
                {
                    Winner = Characters.First().Type;

                    MusicAS.Stop();
                    MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Win];
                    MiniGameAS.Play();

                    Characters.First().Win();
                    Text.Show($"{Winner} Wins!", 2f);
                    Fireworks.Play();
                    Invoke("FadeOut", 2.5f);
                }
                else
                {
                    _isDraw = true;

                    MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Tie];
                    MiniGameAS.Play();
                    Text.Show($"Draw!", 2f);
                    Invoke("FadeOut", 2);
                }
            }
            else
            {
                Invoke("WaitForNewTurn", eliminatedCharacter ? 3f : 0.5f);
            }
        }
    }

    private void ShowGoText()
    {
        MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Go];
        MiniGameAS.Play();
        Text.Show("Go!", 0.5f);
        ChooseFlag();
    }

    private void ChooseFlag()
    {
        FlagType = Random.Range(0, 2) == 1 ? FlagType.A : FlagType.B;
        ShyGuy.WaitForSeconds = _timeBetweenTurns;
        ShyGuy.ShowChosenFlag(FlagType);
        _timeSinceFlagShown = Time.time;
        foreach (FlagCharacter character in Characters)
        {
            character.CorrectFlag = FlagType;
            character.TimeBetweenTurns = _timeBetweenTurns;
            character.MakeChangeFlag();
        }
    }

    private void WaitForNewTurn()
    {
        Characters.ForEach(c => c.HideFlag());
        ShyGuy.HideFlag();

        Invoke("ChooseFlag", Random.Range(0.35f, 1.5f));
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
            _saveData.LastMiniGame = "Shy Guy";
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
                var matchingCharacter = _saveData.Characters.Find(c => c.Type == character.Type);
                character.IsPlayer = matchingCharacter.IsPlayer;
            }
        }
    }
}
