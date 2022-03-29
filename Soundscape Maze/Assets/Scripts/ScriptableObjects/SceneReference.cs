using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Scene Management", menuName = "ScriptableObjects/SceneReference", order = 1)]
public class SceneReference : ScriptableObject
{
    public string Filename;
}
