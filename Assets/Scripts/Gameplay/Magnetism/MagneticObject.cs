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
    public Rigidbody2D Rigidbody { get; private set; }
	public Collider2D Collider { get; private set; }


	[SerializeField] private Polarization defaultPolarization = Polarization.neutral;
	[SerializeField] private float defaultCharge = 1;
	public float chargeValue { get; private set; }
	public float currentCharge { get; private set; }
	public Polarization currentPolarization { get; private set; }

	public bool isMoveable;
	[SerializeField] private bool isPlayer;

	[HideInInspector] public List<MagneticObject> magnetsInRange;
	[HideInInspector] public List<MagneticObject> inRangeOfMagnets;

	private Vector2 newPosition;
	private Vector2 magneticForce;


    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
		Collider = GetComponent<Collider2D>();
		magnetismController = MagnetismController.instance;
		magnetsInRange = new List<MagneticObject>();
		inRangeOfMagnets = new List<MagneticObject>();
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

    //public void ApplyMagneticForce(Vector2 forceToApply)
    //{
    //    Rigidbody.AddForce(forceToApply);
    //    magneticForce = forceToApply; // for gizmos
    //}
    public void ApplyMagneticForce(Vector2 forceToApply)
    {
		magneticForce = forceToApply; // for gizmos
		if (isPlayer)
        {
			playerController.AddMagneticForce(forceToApply);
		}
		else 
		{
            newPosition = transform.position;
            newPosition.x += magneticForce.x;
            newPosition.y += magneticForce.y;
            Rigidbody.MovePosition(newPosition);
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
			Rigidbody.bodyType = RigidbodyType2D.Dynamic;
			Rigidbody.useAutoMass = true;
			Rigidbody.gravityScale = 15;
		}
        else
        {
			magnetismController.UnRegisterMagneticObject(this, isMoveable, false);
			Rigidbody.bodyType = RigidbodyType2D.Kinematic;
			Rigidbody.useFullKinematicContacts = true;
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
