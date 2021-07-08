using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraMove : MonoBehaviour
{
    public GameObject Target;
    public BlurOptimized Blur;

    private Vector3 _offset;
    private GameController _gameController;

    void Start()
    {
        _offset = transform.position - (Target.transform.position + new Vector3(0, -2f, -0.25f));
        _gameController = GameController.Instance;
    }

    void LateUpdate()
    {
        Target = _gameController.GetCurrentCharacter()?.gameObject;
        if (Target != null)
        {
            transform.position = Vector3.Lerp(transform.position, Target.transform.position + _offset, Time.deltaTime * 2.5f);
        }
    }
}