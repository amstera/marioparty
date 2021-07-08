using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    public Spinner Spinner;
    public List<CharacterGame> Characters;
    public SpecialText Text;
    public ParticleSystem Fireworks;
    public FadePanel FadePanel;

    private bool _hasWinner;

    // Start is called before the first frame update
    void Start()
    {
        Text.Show("Ready...", 1.5f);
        Invoke("ShowGoText", 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Characters.Count(c => c != null) == 1 && !_hasWinner)
        {
            CharacterGame winningCharacter = Characters.Find(c => c != null);
            winningCharacter.Win();
            Text.Show($"{winningCharacter.Type} Wins!", 2f);
            Spinner.Stop();
            Fireworks.Play();
            Invoke("FadeOut", 2);
            _hasWinner = true;
        }
    }

    private void ShowGoText()
    {
        Text.Show("Go!", 0.5f);
    }

    private void FadeOut()
    {
        FadePanel.FadeOut();
    }
}
