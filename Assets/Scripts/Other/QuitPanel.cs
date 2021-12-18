using UnityEngine;

public class QuitPanel : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.N))
        {
            FindObjectOfType<CameraMove>().Blur.enabled = false;
            gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            Application.Quit();
        }
    }
    public void Show()
    {
        FindObjectOfType<CameraMove>().Blur.enabled = true;
        gameObject.SetActive(true);
    }
}
