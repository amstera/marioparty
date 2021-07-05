using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rankings : MonoBehaviour
{
    public List<RankingPanel> RankingPanels;
    public CameraMove Cam;

    private GameController _gameController;
    private bool _isShowingRanking;

    void OnEnable()
    {
        _gameController = GameController.Instance;
    }

    void Update()
    {
        if (_isShowingRanking && Input.GetKeyDown(KeyCode.Space))
        {
            Hide();
        }
    }

    public void ShowRankings()
    {
        Time.timeScale = 0;
        Cam.Blur.enabled = true;
        _isShowingRanking = true;

        List<Character> rankedCharacters = _gameController.Characters.OrderByDescending(c => c.Stars).ThenByDescending(c => c.Coins).ToList();
        for (int i = 0; i < RankingPanels.Count; i++)
        {
            RankingPanels[i].Reload(_gameController.GetPlace(rankedCharacters[i].Type), rankedCharacters[i]);
        }

        gameObject.SetActive(true);
    }

    private void Hide()
    {
        Time.timeScale = 1;
        Cam.Blur.enabled = false;
        gameObject.SetActive(false);
        _isShowingRanking = false;
    }
}
