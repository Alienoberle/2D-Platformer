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
	public ContactFilter2D filter;
	private RaycastHit2D[] hits = new RaycastHit2D[10];
	private Vector2 closestPoint;
	private Vector2 otherClosestPoint;

	[SerializeField] public static HashSet<MagneticObject> allMagneticObjects = new HashSet<MagneticObject>();
	[SerializeField] public static HashSet<MagneticObject> movableMagneticObjects = new HashSet<MagneticObject>();
	[SerializeField] public static HashSet<MagneticObject> players = new HashSet<MagneticObject>();

	[SerializeField] private float forceMultiplier = 2f;
	[SerializeField] private float distanceFactor = 1.2f;
	[SerializeField] private float maxForce = 100f;


	private void Awake()
	{
		_instance = this;
	}

	private void FixedUpdate()
	{
		foreach (MagneticObject magneticObject in players)
		{
			HandleMagneticObjects(magneticObject);
		}
		foreach (MagneticObject magneticObject in movableMagneticObjects)
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

		foreach (MagneticObject otherObject in objectToMove.inRangeOfMagnets)
		{
			// Using Unitys version but, prefers corners which makes it awkward.
            //Vector2 closestPoint = objectToMove.objectCollider.ClosestPoint(otherObject.transform.position);
            //Vector2 otherClosestPoint = otherObject.objectCollider.ClosestPoint(objectToMove.transform.position);

            Vector2 direction = objectToMove.transform.position - otherObject.transform.position;
            int objectsHit = otherObject.objectCollider.Raycast(direction, filter, hits);
            for (int i = 0; i < objectsHit; i++)
            {
                if (hits[i].transform.gameObject == objectToMove.gameObject)
                {
                    closestPoint = hits[i].point;

                    Debug.DrawRay(hits[i].normal, new Vector2(0.5f, 0));
                    Debug.DrawRay(hits[i].point, new Vector2(-0.5f, 0));
                    Debug.DrawRay(hits[i].point, new Vector2(0, 0.5f));
                    Debug.DrawRay(hits[i].point, new Vector2(0, -0.5f));
                }
            }
            Array.Clear(hits, 0, hits.Length);
            direction *= -1;
            int objectsHit2 = objectToMove.objectCollider.Raycast(direction, filter, hits);
            for (int i = 0; i < objectsHit2; i++)
            {
                if (hits[i].transform.gameObject == otherObject.gameObject)
                {
                    otherClosestPoint = hits[i].point;

                    Debug.DrawRay(hits[i].normal, new Vector2(0.5f, 0));
                    Debug.DrawRay(hits[i].point, new Vector2(-0.5f, 0));
                    Debug.DrawRay(hits[i].point, new Vector2(0, 0.5f));
                    Debug.DrawRay(hits[i].point, new Vector2(0, -0.5f));
                }
            }

            Vector2 directionClosest = closestPoint - otherClosestPoint;
			float distance = Vector2.Distance(closestPoint, otherClosestPoint);
			Debug.DrawLine(closestPoint, otherClosestPoint);

			float forceMagnitude = forceMultiplier * ((objectToMove.currentCharge * otherObject.currentCharge) / Mathf.Pow(distance, distanceFactor));
			forceMagnitude = Mathf.Clamp(forceMagnitude, -maxForce, maxForce);
			totalForce += forceMagnitude * directionClosest.normalized;
		}

		objectToMove.ApplyMagneticForce(totalForce);
	}
}
