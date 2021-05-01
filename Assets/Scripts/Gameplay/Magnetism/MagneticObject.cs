using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MagneticObject : MonoBehaviour
{
	private MagnetismController magnetismController;
    public Rigidbody2D Rigidbody { get; private set; }
	public Collider2D Collider { get; private set; }

	public MagnetCharge magnetCharge;
	public float chargeStrenght = 1;
	public float currentCharge { get; private set; }

	[SerializeField] private bool isMoveable;

	public List<MagneticObject> magnetsInRange;
	public List<MagneticObject> inRangeOfMagnets;
	private Vector2 magneticForce;

	public MagneticObject(float charge, bool isMoveable)
	{
		this.currentCharge = charge;
		this.isMoveable = isMoveable;
	}
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
		CalculateMagneticCharge(magnetCharge);
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
		Debug.Log(this.name + " is moved by: " + forceToApply);
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
			//other.transform.root.GetComponent<MagneticObject>().ApplyMagneticForce(Vector2.zero);

			// need to find a way to stop the player from continueing sliding after leaving the magnetig trigger
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
	public void CalculateMagneticCharge(MagnetCharge newCharge)
    {
		float sign;
        switch (newCharge)
        {
			case MagnetCharge.negative:
				sign = -1;
				currentCharge = chargeStrenght * sign;
				break;
			case MagnetCharge.positive:
				sign = 1;
				currentCharge = chargeStrenght * sign;
				break;
			case MagnetCharge.neutral:
				sign = 0;
				currentCharge = chargeStrenght * sign;
				break;
		}
		Debug.Log($"New charge is {newCharge} with a strenght of {currentCharge}");
	}
    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.cyan;
		Gizmos.DrawRay(transform.position, magneticForce);
	}
}
