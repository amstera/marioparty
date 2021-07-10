using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rankings : MonoBehaviour
{
    public List<RankingPanel> RankingPanels;
    public GameObject CharacterStats;
    public GameObject TurnText;
    public CameraMove Cam;

    private GameController _gameController;
    private bool _isShowingRanking;
    private Action _callback;

    void Start()
    {
        _gameController = GameController.Instance;
    }

    void Update()
    {
        if (_isShowingRanking && Input.GetKeyDown(KeyCode.Space))
        {
            Hide();
            _callback?.Invoke();
        }
    }

    public void ShowRankings(Action callback, CharacterType miniGameWinner, bool useSaveData)
    {
        Cam.Blur.enabled = true;
        _isShowingRanking = true;

        RankingPanels.ForEach(r => r.gameObject.SetActive(true));
        if (CharacterStats != null)
        {
            CharacterStats.SetActive(false);
        }
        if (TurnText != null)
        {
            TurnText.SetActive(false);
        }
        _callback = callback;

        if (useSaveData)
        {
            SaveData saveData = SaveController.Load();
            List<Character> rankedCharacters = new List<Character>();
            var rankedCompressedCharacters = saveData.Characters.OrderByDescending(c => c.Stars).ThenByDescending(c => c.Coins).ToList();
            foreach (var rankedCharacter in rankedCompressedCharacters)
            {
                rankedCharacters.Add(new Character
                {
                    Coins = rankedCharacter.Coins,
                    Stars = rankedCharacter.Stars,
                    Type = rankedCharacter.Type
                });
            }

            for (int i = 0; i < RankingPanels.Count; i++)
            {
                RankingPanels[i].Reload(GameController.GetPlace(rankedCharacters[i].Type, rankedCharacters), rankedCharacters[i], rankedCharacters[i].Type == miniGameWinner);
            }
        }
        else
        {
            List<Character> rankedCharacters = _gameController.Characters.OrderByDescending(c => c.Stars).ThenByDescending(c => c.Coins).ToList();
            for (int i = 0; i < RankingPanels.Count; i++)
            {
                RankingPanels[i].Reload(_gameController.GetPlace(rankedCharacters[i].Type), rankedCharacters[i], rankedCharacters[i].Type == miniGameWinner);
            }
        }
    }

    private void Hide()
    {
        Cam.Blur.enabled = false;
        _isShowingRanking = false;

        RankingPanels.ForEach(r => r.gameObject.SetActive(false));
        if (CharacterStats != null)
        {
            CharacterStats.SetActive(true);
        }
        if (TurnText != null)
        {
            TurnText.SetActive(true);
        }
    }
}
