using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

public class EarlyTransitionHelper : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector DirectorToStop;

    [SerializeField]
    private PlayableDirector DirectorToStart;

    [SerializeField]
    private KeyCode TransitionKeyCode = KeyCode.Space;

    private bool isTransitioned = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(TransitionKeyCode) && !isTransitioned)
        {
            isTransitioned = true;
            DirectorToStop.Stop();
            DirectorToStart.Play();
        }
    }
}
