using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Magnet : MonoBehaviour
{
	private PlayerController playerController;
	private MagnetismController magnetismController;
    public Rigidbody2D objectRigidbody { get; private set; }
	public Collider2D objectCollider { get; private set; }
	[SerializeField] private GameObject magnetFieldGameObject;

	public Polarization currentPolarization { get; private set; }
	[SerializeField] private Polarization defaultPolarization = Polarization.neutral;

	public float chargeValue { get; private set; }
	public float currentCharge { get; private set; }
	[SerializeField] private float defaultCharge = 1;

	public bool isMoveable;
	[SerializeField] private bool isPlayer;

	public HashSet<Magnet> magnetsInRange = new HashSet<Magnet>();
	public HashSet<Magnet> inRangeOfMagnets = new HashSet<Magnet>();

	private Vector2 magneticForce;

	private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody2D>();
		objectCollider = GetComponent<Collider2D>();
		magnetismController = MagnetismController.instance;
	}
	private void OnEnable()
	{
		if (isPlayer)
			playerController = GetComponentInParent<PlayerController>();

		ChangeCharge(defaultCharge);
		ChangePolarisation(defaultPolarization);
		magnetismController.RegisterMagneticObject(this, isMoveable, isPlayer);
	}

	private void OnDisable()
	{
		magnetismController.UnRegisterMagneticObject(this, isMoveable, isPlayer);
		magnetsInRange.Clear();
	}
    public void ApplyMagneticForce(Vector2 forceToApply)
    {
		magneticForce = forceToApply;
		if (isPlayer)
        {
			playerController.playerInfo.isMagnetismActive = true;
			playerController.magneticVelocity = forceToApply;
		}
		else 
		{
			objectRigidbody.AddForce(forceToApply);
		}
	}
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Magnet"))
        {
			magnetsInRange.Add(other.GetComponent<Magnet>());
			other.GetComponentInParent<Magnet>().inRangeOfMagnets.Add(this);
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Magnet"))
		{
			magnetsInRange.Remove(other.GetComponent<Magnet>());
			other.GetComponentInParent<Magnet>().inRangeOfMagnets.Remove(this);
		}
	}
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
				magnetFieldGameObject.SetActive(true);
				break;
			case Polarization.positive:
				currentCharge = Mathf.Abs(chargeValue) * 1;
				currentPolarization = Polarization.positive;
				magnetFieldGameObject.SetActive(true);
				break;
			case Polarization.neutral:
				currentCharge = 0;
				currentPolarization = Polarization.neutral;
				magnetFieldGameObject.SetActive(false);
				break;
		}
	}
	public void ChangeCharge(float newCharge)
    {
		chargeValue = newCharge;
    }
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
}
