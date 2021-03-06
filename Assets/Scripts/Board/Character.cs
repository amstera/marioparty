using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody Rb;
    public bool IsPlayer;
    public bool CanJump;
    public bool IgnoreLanding;
    public bool HiddenBlockHit;
    public bool CanHitDice;
    public float JumpForce = 6;
    public float Speed = 5;
    public int Roll;
    public int Position;
    public int Coins;
    public int Stars;
    public int MiniGamesWon;
    public int SpacesWalked;
    public CharacterType Type;
    public Queue<Circle> Destinations;
    public List<ItemType> Items = new List<ItemType>();
    public ItemType UsedItem;

    public AmountDisplay AmountDisplay;

    public AudioSource Step;
    public AudioSource CoinSound;
    public AudioSource StarSound;
    public AudioSource ItemSound;
    public AudioSource CharacterAS;

    public List<AudioClip> CharacterSounds;

    private bool _isJumping;
    private bool _isWalking;

    private GameController _gameController;
    public SpacesText _spacesText;

    void Start()
    {
        _gameController = GameController.Instance;
        _spacesText = FindObjectOfType<SpacesText>();
        AmountDisplay.transform.SetParent(null);
        foreach (Character character in FindObjectsOfType<Character>())
        {
            if (character.Type == Type)
            {
                continue;
            }
            Physics.IgnoreCollision(GetComponent<Collider>(), character.GetComponent<Collider>(), true);
        }
    }

    void FixedUpdate()
    {
        if (IsPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    void Update()
    {
        if (_isWalking)
        {
            Walk();
        }
        else
        {
            transform.LookAt(new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Terrain" && _isJumping)
        {
            Land();
        }
    }

    public void Jump()
    {
        if (!CanJump)
        {
            return;
        }

        Animator.SetInteger("State", (int)CharacterState.Jump);
        Rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
        _isJumping = true;
        CanJump = false;
    }

    public void WalkTowards(Queue<Circle> destinations)
    {
        _isWalking = true;
        Animator.SetInteger("State", (int)CharacterState.Walk);
        Destinations = destinations;
    }

    public void ChangeCoins(int amount, bool isBuying)
    {
        Coins = Mathf.Clamp(Coins + amount, 0, 100);
        AmountDisplay.Display(amount, AmountType.Coin, null, transform.position);
        CoinSound.Play();
        if (amount > 0)
        {
            CharacterAS.clip = CharacterSounds[(int)CharacterSoundType.Happy];
            CharacterAS.Play();
            Animator.SetTrigger("Victory");
        }
        else if (!isBuying)
        {
            CharacterAS.clip = CharacterSounds[(int)CharacterSoundType.Sad];
            CharacterAS.Play();
        }
        _gameController?.LoadAllCharacterStats(false);
    }

    public void ChangeStars(int amount)
    {
        Stars = Mathf.Clamp(Stars + amount, 0, 100);
        AmountDisplay.Display(amount, AmountType.Star, null, transform.position);

        if (amount > 0)
        {
            _gameController.MusicAS.Stop();
            StarSound.Play();
            _gameController.MusicAS.PlayDelayed(4f);

            CharacterAS.clip = CharacterSounds[(int)CharacterSoundType.Happy];
            CharacterAS.Play();
            Animator.SetTrigger("Victory");
        }
        else
        {
            CharacterAS.clip = CharacterSounds[(int)CharacterSoundType.Sad];
            CharacterAS.Play();
        }
        _gameController?.LoadAllCharacterStats(false);
    }

    public void AddItem(ItemSelection item)
    {
        Items.Add(item.Type);

        AmountDisplay.Display(1, AmountType.Item, item.ItemImage,  transform.position);

        _gameController.MusicAS.Stop();
        ItemSound.Play();
        _gameController.MusicAS.PlayDelayed(3.5f);

        CharacterAS.clip = CharacterSounds[(int)CharacterSoundType.Happy];
        CharacterAS.Play();
        Animator.SetTrigger("Victory");
    }

    public void UseItem(ItemType item)
    {
        Items.Remove(item);
        UsedItem = item;
        _gameController.UseItem(this);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void PlayHappySound()
    {
        CharacterAS.clip = CharacterSounds[(int)CharacterSoundType.Happy];
        CharacterAS.Play();
    }

    public void PlaySadSound()
    {
        CharacterAS.clip = CharacterSounds[(int)CharacterSoundType.Sad];
        CharacterAS.Play();
    }

    private void Walk()
    {
        if (Step != null && !Step.isPlaying)
        {
            Step.Play();
        }

        Circle space = Destinations.Peek();
        Vector3 pos = PositionFromSpace(space);
        if (Destinations.Count < Roll)
        {
            _spacesText.Show(Destinations.Count.ToString());
        }
        if (Vector3.Distance(transform.position, pos) < Speed * Time.deltaTime)
        {
            transform.position = pos;
            Destinations.Dequeue();
            if (Destinations.Count == 0 || space.Type == CircleType.Star || space.Type == CircleType.Boo || space.Type == CircleType.Item)
            {
                Animator.SetInteger("State", (int)CharacterState.Idle);
                _isWalking = false;
                if (Step != null)
                {
                    Step.Stop();
                }
                _spacesText.Hide();
                _gameController.ReachedSpace(this, space);
                return;
            }
            pos = PositionFromSpace(Destinations.Peek());
        }

        transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    private void Land()
    {
        if (IgnoreLanding)
        {
            IgnoreLanding = false;
            return;
        }

        _isJumping = false;
        Animator.SetInteger("State", (int)CharacterState.Idle);
        _gameController.CharacterLanded(this);
    }

    private Vector3 PositionFromSpace(Circle space)
    {
        Vector3 spacePosition = space.transform.position;
        var pos = new Vector3(spacePosition.x, transform.position.y, spacePosition.z);
        return pos;
    }
}

public enum CharacterState
{
    Idle = 0,
    Jump = 1,
    Walk = 2,
    Holding = 3
}

public enum CharacterType
{
    Unknown = 0,
    Mario = 1,
    Goomba = 2,
    Peach = 3,
    Boo = 4
}

public enum CharacterSoundType
{
    Happy = 0,
    Sad = 1
}