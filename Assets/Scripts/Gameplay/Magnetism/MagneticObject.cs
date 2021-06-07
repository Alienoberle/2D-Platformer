using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MagneticObject : MonoBehaviour
{
	private PlayerController playerController;
	private MagnetismController magnetismController;
    public Rigidbody2D oRigidbody { get; private set; }
	public Collider2D oCollider { get; private set; }

	[SerializeField] private Polarization defaultPolarization = Polarization.neutral;
	[SerializeField] private float defaultCharge = 1;
	public float chargeValue { get; private set; }
	public float currentCharge { get; private set; }
	public Polarization currentPolarization { get; private set; }

	public bool isMoveable;
	[SerializeField] private bool isPlayer;

	[HideInInspector] public HashSet<MagneticObject> magnetsInRange = new HashSet<MagneticObject>();
	[HideInInspector] public HashSet<MagneticObject> inRangeOfMagnets = new HashSet<MagneticObject>();

	private Vector2 newPosition;
	private Vector2 magneticForce;

	private void Awake()
    {
        oRigidbody = GetComponent<Rigidbody2D>();
		oCollider = GetComponent<Collider2D>();
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
			playerController.magneticForce.x = magneticForce.x;
			playerController.magneticForce.y = magneticForce.y;
		}
		else 
		{
			if(magneticForce.x != 0 || magneticForce.y != 0)
            {
				newPosition = transform.position;
				newPosition.x += magneticForce.x * Time.fixedDeltaTime;
				newPosition.y += magneticForce.y * Time.fixedDeltaTime;
				oRigidbody.MovePosition(newPosition);
			}
        }
	}
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Magnet") || other.CompareTag("Player"))
        {
			magnetsInRange.Add(other.GetComponent<MagneticObject>());
			other.GetComponent<MagneticObject>().inRangeOfMagnets.Add(this);
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Magnet") || other.CompareTag("Player"))
		{
			magnetsInRange.Remove(other.GetComponent<MagneticObject>());
			other.GetComponent<MagneticObject>().inRangeOfMagnets.Remove(this);
		}
	}
	[ContextMenu("ToggleMoveable")]
	public void ToggleMoveable(bool isMoveable)
	{
		this.isMoveable = isMoveable;
		if (isMoveable)
        {
			magnetismController.RegisterMagneticObject(this, isMoveable, false);
			oRigidbody.bodyType = RigidbodyType2D.Dynamic;
			oRigidbody.useAutoMass = true;
			oRigidbody.gravityScale = 15;
		}
        else
        {
			magnetismController.UnRegisterMagneticObject(this, isMoveable, false);
			oRigidbody.bodyType = RigidbodyType2D.Kinematic;
			oRigidbody.useFullKinematicContacts = true;
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
