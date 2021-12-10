using UnityEngine;

public class CloudController : MonoBehaviour
{
    public GameObject Cloud;

    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            CreateNewCloud(50 - 25 * i);
        }

        Invoke("CreateNewCloudRepeating", 4f);
    }

    private void CreateNewCloud(int x = 50)
    {
        var newCloud = Instantiate(Cloud, new Vector3(x, Random.Range(6, 10), Random.Range(-20, -9)), Quaternion.identity);
        newCloud.transform.parent = transform;
    }

    private void CreateNewCloudRepeating()
    {
        CreateNewCloud();

        Invoke("CreateNewCloudRepeating", 4f);
    }
}
