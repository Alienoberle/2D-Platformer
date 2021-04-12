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

	private float cycleInterval = 0.01f;
	const float G = 100;

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
	public void AddToList(MagneticObject magneticObject, bool isMoveable)
    {
		allMagneticObjects.Add(magneticObject);
		if (isMoveable)
			movableMagneticObjects.Add(magneticObject);
	}
	public void RemoveFromList(MagneticObject magneticObject, bool isMoveable)
	{

		allMagneticObjects.Remove(magneticObject);
		if (isMoveable)
			movableMagneticObjects.Remove(magneticObject);
	}

	public IEnumerator Cycle(MagneticObject magneticObject)
    {
        while (true)
        {
			ApplyMagneticForce(magneticObject);
			yield return new WaitForSeconds(cycleInterval);
        }
    }

    private void ApplyMagneticForce(MagneticObject magneticObjectToMove)
    {
		Vector3 force = Vector3.zero;
		Rigidbody2D rbToMove = magneticObjectToMove.Rigidbody;

		foreach(MagneticObject otherMagneticObject in allMagneticObjects)
        {
			if (magneticObjectToMove == otherMagneticObject)
				continue;

			Vector3 direction = rbToMove.position - otherMagneticObject.Rigidbody.position;
			float distance = direction.sqrMagnitude;

			if (distance == 0f)
				return;

			float forceMagnitude = G * (magneticObjectToMove.charge * otherMagneticObject.charge) / distance;
			force += direction.normalized * forceMagnitude;

			rbToMove.AddForce(force);
			//rbToMove.velocity = direction.normalized * forceMagnitude * Time.deltaTime;
		}
	}
}
