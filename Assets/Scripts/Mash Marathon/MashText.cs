using UnityEngine;

public class MashText : MonoBehaviour
{
    private Vector3 _startScale;
    private Vector3 _finalScale;
    private bool _scaleUp = true;

    void Start()
    {
        _startScale = transform.localScale;
        _finalScale = transform.localScale * 1.2f;
    }

    void Update()
    {
        if (_scaleUp)
        {
            if (transform.localScale.x < _finalScale.x)
            {
                transform.localScale += new Vector3(1, 1, 0) * Time.deltaTime/5;
            }
            else
            {
                _scaleUp = false;
            }
        }
        else
        {
            if (transform.localScale.x > _startScale.x)
            {
                transform.localScale -= new Vector3(1, 1, 0) * Time.deltaTime/5;
            }
            else
            {
                _scaleUp = true;
            }
        }
    }
}
