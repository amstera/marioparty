using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraMove : MonoBehaviour
{
    public Vector3 StartOffset;
    public Character Target;
    public BlurOptimized Blur;

    private Vector3 _offset;
    private GameController _gameController;

    void Start()
    {
        _offset = transform.position - StartOffset;
        _gameController = GameController.Instance;
    }

    void LateUpdate()
    {
        Target = _gameController?.GetCurrentCharacter() ?? null;
        if (Target != null)
        {
            var newPos = Target.transform.position + _offset;
            if (Target.Position >= 22 && Target.Position <= 32)
            {
                newPos += Vector3.up * 0.5f;
                newPos += Vector3.forward * 2;
            }
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 2.5f);
        }
    }
}