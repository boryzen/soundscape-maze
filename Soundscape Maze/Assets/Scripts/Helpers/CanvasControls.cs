using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CanvasControls : MonoBehaviour
{
    [SerializeField]
    private GameObject TeamsWindow;

    [SerializeField]
    private GameObject MeetingStage;

    [SerializeField]
    private GameObject WebCam;

    [SerializeField]
    private GameObject WebCamOverlay;

    [SerializeField]
    private GameObject AvatarLabel;

    [SerializeField]
    private GameObject TeamsCIPWindow;

    [SerializeField]
    private GameObject ExcelWindow;

    [SerializeField]
    private GameObject ExcelTaskBarIcon;

    [SerializeField]
    private GameObject InstructionsScreen;

    [SerializeField]
    private GameObject Instructions;

    private string ParticipantName = null;

    private string AvatarInitials
    {
        get
        {
            var parts = ParticipantName.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Count() < 1)
            {
                return null;
            }

            return parts.Select(x => { return x.Substring(0, 1); }).Aggregate((x, y) => { return $"{x}{y}"; });
        }
    }

    public void ShowInstructions(string instructions)
    {
        InstructionsScreen.SetActive(true);
        Instructions.GetComponent<TextMeshPro>().text = instructions;
    }

    public void HideInstructions()
    {
        InstructionsScreen.SetActive(false);
    }

    public void ShowTeamsWindow()
    {
        TeamsWindow.SetActive(true);
    }

    public void HideTeamsWindow()
    {
        TeamsWindow.SetActive(false);
    }

    public void ShowStage()
    {
        MeetingStage.SetActive(true);
    }

    public void HideStage()
    {
        MeetingStage.SetActive(false);
    }

    public void ShowWebCam()
    {
        WebCam.SetActive(true);
    }

    public void HideWebCam()
    {
        WebCam.SetActive(false);
    }

    public void PauseWebcam()
    {
        WebCamOverlay.SetActive(true);
    }

    public void ResumeWebcam()
    {
        WebCamOverlay.SetActive(false);
    }

    public void SetParticipantName(string name)
    {
        ParticipantName = name;

        var tmp = AvatarLabel.GetComponent<TextMeshPro>();
        tmp.text = AvatarInitials ?? "";
    }

    public void ShowExcelSpreadsheet()
    {
        ExcelWindow.SetActive(true);
        ExcelTaskBarIcon.SetActive(true);
        TeamsCIPWindow.SetActive(true);
    }

    public void HideExcelSpreadsheet()
    {
        ExcelWindow.SetActive(false);
        ExcelTaskBarIcon.SetActive(false);
        TeamsCIPWindow.SetActive(false);
    }
}
