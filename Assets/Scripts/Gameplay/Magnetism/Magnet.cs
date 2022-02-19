using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Magnet : MonoBehaviour
{
	protected MagnetismController magnetismController;
	protected PlayerController playerController;
	public Collider2D objectCollider { get; private set; }
	public Rigidbody2D objectRigidbody { get; private set; }

	public Polarization currentPolarization { get; private set; }
	[SerializeField] private Polarization defaultPolarization = Polarization.neutral;
	[SerializeField] private float chargeValue = 10;
	public float currentCharge { get; private set; }
	[SerializeField] private bool isMoveable;
	[SerializeField] private bool isPlayer;

	public HashSet<Magnet> affectedByMagnets = new HashSet<Magnet>();

	protected SpriteRenderer spriteRenderer;
	private bool isHighlighted = false;

	//Debug fields
	private Vector2 magneticForce; // store the force for debug purposes

    #region SetUp
    private void Awake()
    {
		objectRigidbody = GetComponent<Rigidbody2D>();
		objectCollider = GetComponent<Collider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		magnetismController = MagnetismController.Instance;
	}

    private void OnEnable()
	{

		if (isPlayer) { 
			playerController = GetComponentInParent<PlayerController>();
		}
		ChangePolarisation(defaultPolarization);
		magnetismController.RegisterMagneticObject(this, isMoveable, isPlayer);
	}

private void OnDisable()
	{
		magnetismController.UnRegisterMagneticObject(this, isMoveable, isPlayer);
	}
    #endregion

    public void ApplyMagneticForce(Vector2 velocity)
    {
		var magneticForce = velocity;
		if (isPlayer)
        {
			playerController.playerInfo.isMagnetismActive = true;
			playerController.magneticVelocity = velocity;
		}
		else 
		{
			objectRigidbody.AddForce(velocity);
		}
	}

	#region Visuals
	public void Highlight()
	{
		if (isHighlighted) return;
		spriteRenderer.material.EnableKeyword("GLOW_ON");
		spriteRenderer.material.SetFloat("_GLOW", 50f);
		isHighlighted = true;
	}
	public void UnHighlight()
	{
		spriteRenderer.material.DisableKeyword("GLOW_ON");
		isHighlighted = false;
	}
    #endregion

    [ContextMenu("ToggleMoveable")]
	public void ToggleMoveable(bool isMoveable)
	{
		this.isMoveable = isMoveable;
		if (isMoveable)
        {
			magnetismController.RegisterMagneticObject(this, isMoveable, false);
			objectRigidbody.bodyType = RigidbodyType2D.Dynamic;
			objectRigidbody.useAutoMass = true;
			objectRigidbody.gravityScale = 15;
		}
        else
        {
			magnetismController.UnRegisterMagneticObject(this, isMoveable, false);
			objectRigidbody.bodyType = RigidbodyType2D.Kinematic;
			objectRigidbody.useFullKinematicContacts = true;
		}

	}
	public void ChangePolarisation(Polarization newPolarization)
    {
        switch (newPolarization)
        {
			case Polarization.negative:
				currentCharge = Mathf.Abs(chargeValue) * -1;
				currentPolarization = Polarization.negative;
				break;
			case Polarization.positive:
				currentCharge = Mathf.Abs(chargeValue) * 1;
				currentPolarization = Polarization.positive;
				break;
			case Polarization.neutral:
				currentCharge = 0;
				currentPolarization = Polarization.neutral;
				break;
		}
	}
	public void ChangeCharge(float newCharge)
    {
		chargeValue = newCharge;
    }
    #region Debug
    private void OnDrawGizmosSelected()
    {
		if (Application.isPlaying)
		{
			GUIStyle customStyle = new GUIStyle();
			customStyle.richText = true;

			Vector3 textPosition = transform.position + (Vector3.down * 3f);
			string _stateText = $"{currentCharge}";
			string richText = "<color=red><size=14>[" + _stateText + "]</size></color>";

			Handles.Label(textPosition, richText, customStyle);

			textPosition = transform.position + (Vector3.down * 4f);
			_stateText = $"{magneticForce}";
			richText = "<color=cyan><size=14>[" + _stateText + "]</size></color>";
			Handles.Label(textPosition, richText, customStyle);
			Gizmos.color = Color.cyan;
			Gizmos.DrawRay(transform.position, magneticForce);
		}
	}
    #endregion
}
