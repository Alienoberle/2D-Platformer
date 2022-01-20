using UnityEngine;

/// <summary>
/// This class contains Settings specific to Menus only
/// </summary>

public enum Menu
{
	Main_Menu,
	Pause_Menu
}

[CreateAssetMenu(fileName = "NewMenu", menuName = "ScriptableObject/SceneManagement/Menu")]
public class MenuSO : GameScene
{
	[Header("Menu specific")]
	public Menu menuType;
}
