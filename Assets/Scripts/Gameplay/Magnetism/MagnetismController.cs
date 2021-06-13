using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetismController : MonoBehaviour
{
	//Singleton set up
	private static MagnetismController _instance;
	public static MagnetismController instance
	{
		get
		{
			if (_instance == null) _instance = FindObjectOfType<MagnetismController>();
			if (_instance == null)
			{
				GameObject spawned = (GameObject)Instantiate(Resources.Load("MagnetismController"));
			}
			return _instance;
		}
	}

	[SerializeField] public static HashSet<MagneticObject> allMagneticObjects = new HashSet<MagneticObject>();
	[SerializeField] public static HashSet<MagneticObject> movableMagneticObjects = new HashSet<MagneticObject>();
	[SerializeField] public static HashSet<MagneticObject> players = new HashSet<MagneticObject>();

	private float fixedDeltaTime;
	[SerializeField] private float forceMultiplier = 2f;
	[SerializeField] private float distanceFactor = 1.2f;


	private void Awake()
	{
		_instance = this;
	}

	private void FixedUpdate()
	{
		foreach (MagneticObject magneticObject in movableMagneticObjects)
		{
			HandleMagneticObjects(magneticObject);
		}
		foreach (MagneticObject magneticObject in players)
		{
			HandleMagneticObjects(magneticObject);
		}
	}

	public void RegisterMagneticObject(MagneticObject magneticObject, bool isMoveable, bool isPlayer)
    {
		allMagneticObjects.Add(magneticObject);
        if (isPlayer)
        {
			players.Add(magneticObject);
			return;
		}
		if (isMoveable)
			movableMagneticObjects.Add(magneticObject);
	}
	public void UnRegisterMagneticObject(MagneticObject magneticObject, bool isMoveable, bool isPlayer)
	{
		allMagneticObjects.Remove(magneticObject);
		if (isPlayer)
		{
			players.Remove(magneticObject);
			return;
		}
		if (isMoveable)
			movableMagneticObjects.Remove(magneticObject);
	}
	private void HandleMagneticObjects(MagneticObject objectToMove)
	{
		Vector2 force = Vector2.zero;
		Vector2 totalForce = Vector2.zero;
		if (objectToMove.inRangeOfMagnets.Count < 1 || objectToMove.currentCharge == 0)
        {
			objectToMove.ApplyMagneticForce(totalForce);
			return;
		}

		Vector2 objectPosition = objectToMove.transform.position;
		foreach (MagneticObject otherObject in objectToMove.inRangeOfMagnets)
		{
			Vector2 closestPoint = otherObject.objectCollider.ClosestPoint(objectPosition);
			Vector2 direction = objectPosition - closestPoint;
			float distance = Vector2.Distance(objectPosition, closestPoint);

			float forceMagnitude = forceMultiplier * ((objectToMove.currentCharge * otherObject.currentCharge) / (1 + Mathf.Pow(distance, distanceFactor)));
			force.x = direction.normalized.x * forceMagnitude;
			force.y = direction.normalized.y * forceMagnitude;
			totalForce += force;
		}
		objectToMove.ApplyMagneticForce(totalForce);
	}
}
