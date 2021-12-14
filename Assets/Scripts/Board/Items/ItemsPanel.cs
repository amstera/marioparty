using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemsPanel : MonoBehaviour
{
    public List<ItemSelection> ItemSelections;

    private int _curIndex;
    private Action _callback;
    private Character _currentCharacter;
    private List<ItemSelection> _shownSelections;

    public AudioSource SelectAS;

    public void Show(Character character, bool showUpdatedItems, Action callback)
    {
        _callback = callback;
        _currentCharacter = character;

        ShowUpdatedItems(showUpdatedItems);

        gameObject.SetActive(true);
    }

    public List<ItemSelection> GetItems(bool showUpdatedItems)
    {
        ShowUpdatedItems(showUpdatedItems);
        return ItemSelections;
    }

    private void ShowUpdatedItems(bool showUpdatedItems)
    {
        if (showUpdatedItems)
        {
            ItemSelections[3].gameObject.SetActive(true);
            ItemSelections[2].gameObject.SetActive(false);
        }
        else
        {
            ItemSelections[3].gameObject.SetActive(false);
            ItemSelections[2].gameObject.SetActive(true);
        }

        _shownSelections = ItemSelections.FindAll(i => i.gameObject.activeSelf);
    }

    void Update()
    {
        if (_curIndex < 2 && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            _shownSelections[_curIndex].Select(false);
            _curIndex++;
            _shownSelections[_curIndex].Select(true);

            SelectAS.Play();
        }
        else if (_curIndex > 0 && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            _shownSelections[_curIndex].Select(false);
            _curIndex--;
            _shownSelections[_curIndex].Select(true);

            SelectAS.Play();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var selectedItem = _shownSelections[_curIndex];
            if (_currentCharacter.Coins < selectedItem.Cost)
            {
                return;
            }

            _currentCharacter.ChangeCoins(-selectedItem.Cost, true);
            _currentCharacter.AddItem(selectedItem);

            FindObjectOfType<Dialog>().ShowText($"{_currentCharacter.Type} got a {selectedItem.ItemNameText.text}!", _callback);

            gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _callback.Invoke();

            gameObject.SetActive(false);
        }
    }
}
