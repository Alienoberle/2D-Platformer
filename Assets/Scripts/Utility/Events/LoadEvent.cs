using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is a used for scene loading Events
/// Takes an array of the scenes we want to load 
/// and a bool to specify if we want to show a transition and/or loading screen
/// </summary>

[CreateAssetMenu(fileName = "LoadGameEvent", menuName = "ScriptableObject/Game Event/Load")]
public class LoadEvent : ScriptableObject
{
	public UnityAction<GameScene[]> loadEvent;
	public void Raise(GameScene[] locationsToLoad)
	{
		if (loadEvent != null)
			loadEvent.Invoke(locationsToLoad);
	}
}
