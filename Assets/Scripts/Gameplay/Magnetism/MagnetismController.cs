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

	public static List<MagneticObject> magneticObjectsList = new List<MagneticObject>();
	private Rigidbody2D rigidBody;
	private Collider2D collider;
	
	const float G = 667.4f;

	private void Awake()
	{
		_instance = this;

		if (magneticObjectsList == null)
			magneticObjectsList = new List<MagneticObject>();

		rigidBody = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
	}
	private void FixedUpdate()
	{
        foreach (MagneticObject magnet in magneticObjectsList)
        {
            Attract(magnet);
        }
    }
	public void AddToList(MagneticObject magneticObject)
    {
		magneticObjectsList.Add(magneticObject);
	}
	public void RemoveFromList(MagneticObject magneticObject)
	{
		magneticObjectsList.Remove(magneticObject);
	}

    private void Attract(MagneticObject objToAttract)
    {
        Rigidbody2D rbToAttract = objToAttract.Rigidbody;

        Vector3 direction = rigidBody.position - rbToAttract.position;
        float distance = direction.sqrMagnitude;

        if (distance == 0f)
            return;

        float forceMagnitude = G * (rigidBody.mass * rbToAttract.mass) / distance;
        Vector3 force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
    }
}
