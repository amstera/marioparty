using UnityEngine;

public class MashCamera : MonoBehaviour
{
    public FlyGuy Target;

    private float _offset;

    void Update()
    {
        if (Target == null)
        {
            return;
        }

        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, Target.transform.localPosition.z + _offset);
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 2.5f);
    }

    public void StartMoving(FlyGuy flyGuy)
    {
        Target = flyGuy;
        _offset = transform.position.z - flyGuy.transform.localPosition.z;
    }
}
