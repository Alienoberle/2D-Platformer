using System;
using UnityEngine;

/// <summary>
/// This class contains Settings specific to Locations only
/// </summary>

[CreateAssetMenu(fileName = "NewLevel", menuName = "ScriptableObject/SceneManagement/Level")]
public class Level : GameScene
{
	[Header("Audio")]
	public AudioClip music;
	[Range(0.0f, 1.0f)]
	public float musicVolume;
}
