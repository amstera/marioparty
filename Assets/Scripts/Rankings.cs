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

    public void ShowRankings(Action callback, CharacterType miniGameWinner)
    {
        Cam.Blur.enabled = true;
        _isShowingRanking = true;

        RankingPanels.ForEach(r => r.gameObject.SetActive(true));
        CharacterStats.SetActive(false);
        TurnText.SetActive(false);
        _callback = callback;

        List<Character> rankedCharacters = _gameController.Characters.OrderByDescending(c => c.Stars).ThenByDescending(c => c.Coins).ToList();
        for (int i = 0; i < RankingPanels.Count; i++)
        {
            RankingPanels[i].Reload(_gameController.GetPlace(rankedCharacters[i].Type), rankedCharacters[i], rankedCharacters[i].Type == miniGameWinner);
        }
    }

    private void Hide()
    {
        Cam.Blur.enabled = false;
        _isShowingRanking = false;

        RankingPanels.ForEach(r => r.gameObject.SetActive(false));
        CharacterStats.SetActive(true);
        TurnText.SetActive(true);
    }
}
