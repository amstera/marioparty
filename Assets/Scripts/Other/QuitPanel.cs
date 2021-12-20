using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (SceneManager.GetActiveScene().name == "Character Chooser")
            {
                Application.Quit();
            }
            else
            {
                PlayerPrefs.DeleteAll();
                SceneManager.LoadSceneAsync("Character Chooser");
            }
        }
    }
    public void Show()
    {
        FindObjectOfType<CameraMove>().Blur.enabled = true;
        gameObject.SetActive(true);
    }
}
