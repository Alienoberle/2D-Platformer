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

	[SerializeField] public static HashSet<Magnet> allMagneticObjects = new HashSet<Magnet>();
	[SerializeField] public static HashSet<Magnet> movableMagneticObjects = new HashSet<Magnet>();
	[SerializeField] public static HashSet<Magnet> players = new HashSet<Magnet>();

	[SerializeField] private float distanceFactor = 1.2f;
	[SerializeField] private float maxForce = 18f;
    private Color yellow;

    private void Awake()
	{
		_instance = this;
	}

	private void FixedUpdate()
	{
		foreach (Magnet magneticObject in players)
		{
			HandleMagneticObjects(magneticObject);
		}
		foreach (Magnet magneticObject in movableMagneticObjects)
		{
			HandleMagneticObjects(magneticObject);
		}
	}

	public void RegisterMagneticObject(Magnet magneticObject, bool isMoveable, bool isPlayer)
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
	public void UnRegisterMagneticObject(Magnet magneticObject, bool isMoveable, bool isPlayer)
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
	private void HandleMagneticObjects(Magnet objectToMove)
	{
		Vector2 magneticVelocity = Vector2.zero;
		if (objectToMove.inRangeOfMagnets.Count < 1 || objectToMove.currentCharge == 0)
        {
			objectToMove.ApplyMagneticForce(magneticVelocity);
			return;
		}

		foreach (Magnet otherObject in objectToMove.inRangeOfMagnets)
		{
            Vector2 direction = objectToMove.transform.position - otherObject.transform.position;
            int objectsHit = otherObject.objectCollider.Raycast(direction, filter, hits);
            for (int i = 0; i < objectsHit; i++)
            {
                if (hits[i].transform.gameObject == objectToMove.gameObject)
                {
                    closestPoint = hits[i].point;

                    Debug.DrawRay(hits[i].point, new Vector2(0.5f, 0), Color.green);
                    Debug.DrawRay(hits[i].point, new Vector2(-0.5f, 0), Color.green);
                    Debug.DrawRay(hits[i].point, new Vector2(0, 0.5f), Color.green);
                    Debug.DrawRay(hits[i].point, new Vector2(0, -0.5f), Color.green);
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

                    Debug.DrawRay(hits[i].point, new Vector2(0.5f, 0));
                    Debug.DrawRay(hits[i].point, new Vector2(-0.5f, 0));
                    Debug.DrawRay(hits[i].point, new Vector2(0, 0.5f));
                    Debug.DrawRay(hits[i].point, new Vector2(0, -0.5f));
                }
            }

			float distance = Vector2.Distance(closestPoint, otherClosestPoint);
			//Vector2 directionClosest = closestPoint - otherClosestPoint;  // lots of jittering
			Vector2 directionClosest = closestPoint - (Vector2)otherObject.transform.position;
			Debug.DrawLine(closestPoint, otherClosestPoint);

            float forceMagnitude = (objectToMove.currentCharge * otherObject.currentCharge) / Mathf.Pow(distance, distanceFactor);
            forceMagnitude = Mathf.Clamp(forceMagnitude, -maxForce, maxForce);
            magneticVelocity = forceMagnitude * directionClosest.normalized;
        }
		objectToMove.ApplyMagneticForce(magneticVelocity);
	}
}
