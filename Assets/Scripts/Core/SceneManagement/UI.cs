using UnityEngine;

[CreateAssetMenu(fileName = "NewUI", menuName = "ScriptableObject/SceneManagement/UI")]
public class UI : GameScene
{
	/// <summary>
	/// This class contains Settings specific to Menus only
	/// </summary>

	[Header("Menu specific")]
	public UIType menuType;
}
public enum UIType
{
	Main_Menu,
	Pause_Menu,
	Upgrade_Menu,
	Shop_Menu,
	HUD,
	Debug_Menu
}

