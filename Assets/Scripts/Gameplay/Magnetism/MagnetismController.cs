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
            ApplyMagneticForce(magneticObject);
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

 //   private void ApplyMagneticForce(MagneticObject magneticObjectToMove)
 //   {
	//	Vector2 force = Vector2.zero;
	//	Rigidbody2D rbToMove = magneticObjectToMove.Rigidbody;

	//	foreach(MagneticObject otherMagneticObject in allMagneticObjects)
 //       {
	//		if (magneticObjectToMove == otherMagneticObject)
	//			continue;

	//		Vector2 closestPoint = otherMagneticObject.Collider.ClosestPoint(rbToMove.position);
	//		Vector2 direction = rbToMove.position - closestPoint;
	//		float distance = Vector2.Distance(rbToMove.position, closestPoint);

	//		if (distance == 0f)
	//			continue;

 //           if (distance > otherMagneticObject.magneticDistance)
 //               continue;

 //           float forceMagnitude = Mathf.Clamp(magnetismFactor * (magneticObjectToMove.charge * otherMagneticObject.charge) / Mathf.Pow(distance, 3), -500, 500) ;
	//		force += direction.normalized * forceMagnitude ;

	//	}

	//	rbToMove.AddForce(force);
	//	//Debug.Log(magneticObjectToMove.name + " is moved by: " + force);
	//}
	private void ApplyMagneticForce(MagneticObject objectToMove)
	{
		Vector2 force = Vector2.zero;
		Rigidbody2D rbToMove = objectToMove.Rigidbody;

		if (objectToMove.inRangeOfMagnets.Count < 1)
			return;

		foreach (MagneticObject otherObject in objectToMove.inRangeOfMagnets)
		{
			Vector2 closestPoint = otherObject.Collider.ClosestPoint(rbToMove.position);
			Vector2 direction = rbToMove.position - closestPoint;
			float distance = Vector2.Distance(rbToMove.position, closestPoint);

			if (distance == 0f)
				continue;

			float forceMagnitude = Mathf.Clamp(magnetismFactor * (objectToMove.charge * otherObject.charge) / Mathf.Pow(distance, 3), -300, 300);
			force += direction.normalized * forceMagnitude;

		}

		rbToMove.AddForce(force);
		Debug.Log(objectToMove.name + " is moved by: " + force);
	}
}
