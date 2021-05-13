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
	private MagnetismController magnetismController;
    public Rigidbody2D Rigidbody { get; private set; }
	public Collider2D Collider { get; private set; }

	[SerializeField] private Polarization defaultPolarization = Polarization.neutral;
	[SerializeField] private float defaultCharge = 1;
	public float currentCharge { get; private set; }
	public Polarization currentPolarization { get; private set; }

	[SerializeField] private bool isMoveable;
	
	[HideInInspector] public List<MagneticObject> magnetsInRange;
	[HideInInspector] public List<MagneticObject> inRangeOfMagnets;

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
		CalculateMagneticCharge(defaultPolarization);
		magnetismController.RegisterMagneticObject(this, isMoveable);
	}

	private void OnDisable()
	{
		magnetismController.UnRegisterMagneticObject(this, isMoveable);
		magnetsInRange.Clear();
	}

	public void ApplyMagneticForce(Vector2 forceToApply)
    {
		Rigidbody.AddForce(forceToApply);

		magneticForce = forceToApply;
		//Debug.Log(this.name + " is moved by: " + forceToApply);
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
	public void ToggleMoveable(bool isMoveable)
	{
		this.isMoveable = isMoveable;
		if (isMoveable)
			magnetismController.RegisterMagneticObject(this, isMoveable);
		else
			magnetismController.UnRegisterMagneticObject(this, isMoveable);
	}
	public virtual void CalculateMagneticCharge(Polarization newCharge)
    {
        switch (newCharge)
        {
			case Polarization.negative:
				currentCharge = Mathf.Abs(defaultCharge) * -1;
				currentPolarization = Polarization.negative;
				break;
			case Polarization.positive:
				currentCharge = Mathf.Abs(defaultCharge) * 1;
				currentPolarization = Polarization.positive;
				break;
			case Polarization.neutral:
				currentCharge = 0;
				currentPolarization = Polarization.neutral;
				break;
		}
	}
    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.cyan;
		Gizmos.DrawRay(transform.position, magneticForce);

		if (Application.isPlaying)
		{
			string _stateText = $"{currentCharge}";

			GUIStyle customStyle = new GUIStyle();
			customStyle.richText = true;
			Vector3 textPosition = transform.position + (Vector3.up * 2f);
			string richText = "<color=red><size=14>[" + _stateText + "]</size></color>";

			Handles.Label(textPosition, richText, customStyle);
		}

	}
}
