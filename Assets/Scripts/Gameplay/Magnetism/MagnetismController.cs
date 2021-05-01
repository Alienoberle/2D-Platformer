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

	public static List<MagneticObject> allMagneticObjects = new List<MagneticObject>();
	public static List<MagneticObject> movableMagneticObjects = new List<MagneticObject>();

	const float magnetismFactor = 100;

	private void Awake()
	{
		_instance = this;

		if (allMagneticObjects == null)
			allMagneticObjects = new List<MagneticObject>();

		if (movableMagneticObjects == null)
			movableMagneticObjects = new List<MagneticObject>();
	}
	private void FixedUpdate()
	{
        foreach (MagneticObject magneticObject in movableMagneticObjects)
        {
            CalculateMagneticForce(magneticObject);
        }
    }
	public void RegisterMagneticObject(MagneticObject magneticObject, bool isMoveable)
    {
		allMagneticObjects.Add(magneticObject);
		if (isMoveable)
			movableMagneticObjects.Add(magneticObject);
	}
	public void UnRegisterMagneticObject(MagneticObject magneticObject, bool isMoveable)
	{

		allMagneticObjects.Remove(magneticObject);
		if (isMoveable)
			movableMagneticObjects.Remove(magneticObject);
	}
	private void CalculateMagneticForce(MagneticObject objectToMove)
	{
		Vector2 force = Vector2.zero;
		Vector2 objectPosition = objectToMove.transform.position;

		if (objectToMove.inRangeOfMagnets.Count < 1)
        {
			objectToMove.ApplyMagneticForce(Vector2.zero);
			return;
		}

		foreach (MagneticObject otherObject in objectToMove.inRangeOfMagnets)
		{
			Vector2 closestPoint = otherObject.Collider.ClosestPoint(objectPosition);
			Vector2 direction = objectPosition - closestPoint;
			float distance = Vector2.Distance(objectPosition, closestPoint);

			if (distance == 0f)
				continue;

            float forceMagnitude = Mathf.Clamp(magnetismFactor * (objectToMove.currentCharge * otherObject.currentCharge) / Mathf.Pow(distance, 2), -300, 300);
			force += direction.normalized * forceMagnitude;

			if (Physics2D.IsTouching(objectToMove.Collider, otherObject.Collider))
				otherObject.ApplyMagneticForce(-force);
		}

		objectToMove.ApplyMagneticForce(force);
	}
}
