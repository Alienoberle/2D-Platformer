using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticPickUp : Magnet
{
	[SerializeField] private float maxVelocity = 5.0f;
	private Vector2 newVelocity;

    public override void ApplyMagneticForce(Vector2 velocity)
	{
		magneticForce = velocity;
		newVelocity = objectRigidbody.velocity + velocity;

		objectRigidbody.velocity += velocity;
		if (newVelocity.sqrMagnitude > (maxVelocity * maxVelocity))
		{
			var breakSpeed = newVelocity.magnitude - maxVelocity;
			var brakeVelocity = velocity.normalized * breakSpeed;
			objectRigidbody.velocity -= brakeVelocity;
		}
	}
}
