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
	public Collider2D[] magnetfields;

	public List<Collider2D> magneticObjectsInMagnetField = new List<Collider2D>();

	public float charge = 0;
	public MagnetCharge magnetCharge { get; set; }
	[SerializeField] private bool isMoveable;
	public float magneticDistance;

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
	}
	private void OnEnable()
	{
		magnetismController.AddToList(this, isMoveable);
		foreach(Collider2D magnetfield in magnetfields)
        {
			//check if another magnetic object is overlapping
        }
	}

	private void OnDisable()
	{
		magnetismController.RemoveFromList(this, isMoveable);
	}

	public void ToggleMoveable(bool isMoveable)
	{
		this.isMoveable = isMoveable;
		if (isMoveable)
			magnetismController.AddToList(this, isMoveable);
		else
			magnetismController.RemoveFromList(this, isMoveable);
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
