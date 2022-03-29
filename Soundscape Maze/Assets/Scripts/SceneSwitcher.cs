using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void ChangeScene(SceneReference reference)
    {
        SceneManager.LoadScene(reference.Filename);
    }

    public void GoToNextScene()
    {
        var next = SceneManager.GetActiveScene().buildIndex + 1;

        if (next >= SceneManager.sceneCountInBuildSettings)
            return;

        SceneManager.LoadScene(next);
    }
}
