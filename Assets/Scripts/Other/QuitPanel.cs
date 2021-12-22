using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitPanel : MonoBehaviour
{
    public GameObject QuitSave;
    public GameObject Quit;
    public Text QuitText;
    public GameObject Cancel;

    private bool _isMainPage;

    void Start()
    {
        _isMainPage = SceneManager.GetActiveScene().name == "Character Chooser";

        if (_isMainPage)
        {
            QuitSave.gameObject.SetActive(false);
            Cancel.transform.position = Quit.transform.position + Vector3.down * 25;
            Quit.transform.position = QuitSave.transform.position + Vector3.down * 25;

            QuitText.text = "Yes <b><color=yellow>(Q)</color></b>";
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.N))
        {
            Time.timeScale = 1;
            FindObjectOfType<CameraMove>().Blur.enabled = false;
            gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = 1;
            if (_isMainPage)
            {
                Application.Quit();
            }
            else
            {
                PlayerPrefs.DeleteAll();
                SceneManager.LoadSceneAsync("Character Chooser");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            Time.timeScale = 1;
            Application.Quit();
        }
    }

    public void Show()
    {
        Time.timeScale = 0;
        FindObjectOfType<CameraMove>().Blur.enabled = true;
        gameObject.SetActive(true);
    }
}
