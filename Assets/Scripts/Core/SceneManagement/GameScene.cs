using UnityEngine;

/// <summary>
/// This class is a base class which contains what is commun for all game scenes (Locations or Menus)
/// </summary>

public class GameScene : ScriptableObject
{
	[Header("Information")]
	public string sceneName;
	public string shortDescription;

	[Header("Scene Loading")]
	public bool showSceneTransition;
	public string transitionName;
}