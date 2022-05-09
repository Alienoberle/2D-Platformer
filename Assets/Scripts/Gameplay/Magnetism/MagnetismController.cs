using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetismController : StaticInstance<MagnetismController>
{
	[SerializeField] private ForceVisualisation forceVisualisation;
	[SerializeField] public HashSet<Magnet> allMagneticObjects = new HashSet<Magnet>();
	[SerializeField] public HashSet<Magnet> movableMagneticObjects = new HashSet<Magnet>();
	[SerializeField] public HashSet<Magnet> players = new HashSet<Magnet>();
	[SerializeField] public HashSet<MagneticForce> activeForces = new HashSet<MagneticForce>();

	public LayerMask layerMask;
	private RaycastHit2D[] hits = new RaycastHit2D[10];
	private Vector2 closestPoint;
	private Vector2 otherClosestPoint;

	[SerializeField] private float distanceFactor = 1.5f;
	[SerializeField] private float maxForce = 20f;

	private void FixedUpdate()
	{
		activeForces.Clear();
		foreach (Magnet magnet in players)
		{
			HandleMagneticObjects(magnet);
		}
		foreach (Magnet magnet in movableMagneticObjects)
		{
			HandleMagneticObjects(magnet);
		}
	}
	public void RegisterMagneticObject(Magnet magnet)
    {
		allMagneticObjects.Add(magnet);
        if (magnet is PlayerMagnetism)
        {
			players.Add(magnet);
			return;
		}
		if (magnet.isMoveable)
			movableMagneticObjects.Add(magnet);
	}
	public void UnRegisterMagneticObject(Magnet magnet)
	{
		allMagneticObjects.Remove(magnet);
		if (magnet is PlayerMagnetism)
		{
			players.Remove(magnet);
			return;
		}
		if (magnet.isMoveable)
			movableMagneticObjects.Remove(magnet);
	}
	private void HandleMagneticObjects(Magnet objectToMove)
	{
		Vector2 magneticVelocity = Vector2.zero;
		if (objectToMove.affectedByMagnets.Count < 1 || objectToMove.currentCharge == 0) //setting velocity to 0 and stop execution at tis point
		{
			objectToMove.ApplyMagneticForce(magneticVelocity);  
			return;
		}

		foreach (Magnet otherObject in objectToMove.affectedByMagnets)
		{
            if (otherObject.currentCharge == 0 || otherObject.GetType() == typeof(MagneticPickUp))
			{
                continue;
            }
            Vector2 direction = objectToMove.transform.position - otherObject.transform.position;
			hits = Physics2D.RaycastAll(otherObject.transform.position, direction, 50.0f, layerMask);
			for (int i = 0; i < hits.Length; i++)
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
			hits = Physics2D.RaycastAll(objectToMove.transform.position, direction, 50.0f, layerMask);
			for (int i = 0; i < hits.Length; i++)
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
			Vector2 directionClosest = closestPoint - (Vector2)otherObject.transform.position;
			Debug.DrawLine(closestPoint, otherClosestPoint);

            float forceMagnitude = (objectToMove.currentCharge * otherObject.currentCharge) / Mathf.Pow(distance, distanceFactor);
            forceMagnitude = Mathf.Clamp(forceMagnitude, -maxForce, maxForce);

            if (objectToMove.GetType() == typeof(MagneticPickUp))
            {
                forceMagnitude = Mathf.Abs(forceMagnitude) * -1;
            }

            magneticVelocity += forceMagnitude * directionClosest.normalized;
			activeForces.Add(new MagneticForce(objectToMove, otherObject, closestPoint, otherClosestPoint, directionClosest, distance, magneticVelocity));
		}
		objectToMove.ApplyMagneticForce(magneticVelocity);
	}
}
