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

            QuitText.text = "Yes (Q)";
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.N))
        {
            FindObjectOfType<CameraMove>().Blur.enabled = false;
            gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
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
            Application.Quit();
        }
    }

    public void Show()
    {
        FindObjectOfType<CameraMove>().Blur.enabled = true;
        gameObject.SetActive(true);
    }
}
