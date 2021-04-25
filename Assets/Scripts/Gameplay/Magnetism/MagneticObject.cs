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
	public float charge = 0;
	public MagnetCharge magnetCharge { get; set; }
	[SerializeField] private bool isMoveable;
	public float magneticDistance;
	public List<MagneticObject> magnetsInRange;
	public List<MagneticObject> inRangeOfMagnets;

	public MagneticObject(float charge, bool isMoveable)
	{
		this.charge = charge;
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
		magnetismController.RegisterMagneticObject(this, isMoveable);
	}

	private void OnDisable()
	{
		magnetismController.UnRegisterMagneticObject(this, isMoveable);
		magnetsInRange.Clear();
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Magnet"))
        {
			magnetsInRange.Add(other.GetComponent<MagneticObject>());
			other.GetComponent<MagneticObject>().inRangeOfMagnets.Add(this);
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Magnet"))
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

	public enum MagnetCharge
	{
		negative = 1,
		neutral = 2,
		positive = 3,
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, magneticDistance);

	}
}
