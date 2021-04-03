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
	public MagnetCharge magnetCharge { get; set; }
	public bool isDynamic { get; set; }

	public MagneticObject(MagnetCharge magnetCharge, bool isDynamic)
	{
		this.magnetCharge = magnetCharge;
		this.isDynamic = isDynamic;
	}
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
		Collider = GetComponent<Collider2D>();
		magnetismController = MagnetismController.instance;
	}
	private void OnEnable()
	{
		magnetismController.AddToList(this);
	}

	private void OnDisable()
	{
		magnetismController.RemoveFromList(this);
	}

	public enum MagnetCharge
	{
		neutral = 0,
		positive = 10,
		negative = 20,
	}
}
