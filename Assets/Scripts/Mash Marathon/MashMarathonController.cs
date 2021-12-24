using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MashMarathonController : MonoBehaviour
{
    public List<MashCharacter> Characters;
    public List<FlyGuy> FlyGuys;
    public SpecialText Text;
    public ParticleSystem Fireworks;
    public FadePanel FadePanel;
    public CharacterType Winner;
    public GameObject StartText;
    public Timer Timer;
    public GameObject ExclamationBubbles;
    public MashCamera Camera;

    public AudioSource MiniGameAS;
    public AudioSource MusicAS;

    public List<AudioClip> MiniGameSounds;

    private SaveData _saveData;
    private bool _isDraw;
    private bool _gameStarted;
    private CharacterType _winningCharacter;

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

        if (_gameStarted && Winner == CharacterType.Unknown && FlyGuys.All(f => f.IsFinished))
        {
            Winner = _winningCharacter;
            var winningFlyGuy = FlyGuys.Find(c => c.CharacterType == Winner);
            Fireworks.transform.position = new Vector3(Fireworks.transform.position.x, Fireworks.transform.position.y, winningFlyGuy.transform.position.z);

            MusicAS.Stop();
            MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Win];
            MiniGameAS.Play();

            var winningCharacter = Characters.Find(c => c.Type == Winner);
            winningCharacter.Win();
            Text.Show($"{Winner} Wins!", 2f);
            Fireworks.Play();
            Invoke("FadeOut", 2.5f);
        }
    }

    public void FlyGuyStoppedSpinning(CharacterType characterType)
    {
        if (FlyGuys.All(f => f.IsFinished))
        {
            return;
        }

        var character = Characters.Find(c => c.Type == characterType);
        character.Lose();
    }

    private void ShowGoText()
    {
        MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Go];
        MiniGameAS.Play();
        Text.Show("Go!", 0.5f);
        Timer.Countdown(MoveFlyGuy);
        ExclamationBubbles.SetActive(true);
        FlyGuys.ForEach(f => f.CanMash = true);
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
                var matchingCharacter = _saveData.Characters.Find(c => c.Type == character.Type);
                var matchingFlyGuy = FlyGuys.Find(f => f.CharacterType == matchingCharacter.Type);
                character.IsPlayer = matchingCharacter.IsPlayer;
                matchingFlyGuy.IsPlayer = matchingCharacter.IsPlayer;
            }
        }
    }

    private void MoveFlyGuy()
    {
        FlyGuys.ForEach(f => f.CanMash = false);
        ExclamationBubbles.SetActive(false);
        foreach (FlyGuy flyGuy in FlyGuys)
        {
            if (!flyGuy.IsPlayer)
            {
                flyGuy.DistanceSpun = Random.Range(9f, 13f);
            }
        }
        FlyGuys.ForEach(f => f.StartSpinning());
        Characters.ForEach(c => c.Idle());
        var winningFlyGuy = FlyGuys.OrderByDescending(f => f.DistanceSpun).First();
        _winningCharacter = winningFlyGuy.CharacterType;
        Camera.StartMoving(winningFlyGuy);
    }
}
