using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private bool AllowEscapeHatchToNextScene = true;

    [SerializeField]
    private bool AllowEscapeHatchToTOC = true;

    // Update is called once per frame
    void Update()
    {
        // Hotkey to quit the app
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyUp(KeyCode.Q))
        {
            ExitDemo();
        }

        // Hotkey to escape to the table of contents
        if (AllowEscapeHatchToTOC && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyUp(KeyCode.M))
        {
            GoToTableOfContents();
        }

        // Hotkey to jump to the next scene
        if (AllowEscapeHatchToNextScene && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyUp(KeyCode.N))
        {
            GoToNextScene();
        }
    }

    public void ExitDemo()
    {
        HeadTrackedCameraBehavior.Listener.Stop();
        Application.Quit();
    }

    public void GoToNextScene()
    {
        var next = SceneManager.GetActiveScene().buildIndex + 1;

        if (next >= SceneManager.sceneCountInBuildSettings)
            return;

        SceneManager.LoadScene(next);
    }

    public void GoToTableOfContents()
    {
        var last = SceneManager.sceneCountInBuildSettings - 1;

        if (last == SceneManager.GetActiveScene().buildIndex)
            return;

        SceneManager.LoadScene(last);
    }
}
