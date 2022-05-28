using UnityEngine;

public class MagneticObject : Magnet
{
	[Header("Magnet Colors")]
	[SerializeField] private Color colorPositive = Color.white;
	[SerializeField] private Color colorNegative = Color.white;

	[SerializeField] private float maxVelocity = 5.0f;
	private Vector2 newVelocity;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Magnet"))
		{
			other.GetComponentInParent<Magnet>().affectedByMagnets.Add(this);
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Magnet"))
		{
			other.GetComponentInParent<Magnet>().affectedByMagnets.Remove(this);
		}
	}
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
	public override void ChangePolarisation(Polarization newPolarization)
	{
		base.ChangePolarisation(newPolarization);
		switch (newPolarization)
		{
			case Polarization.negative:
				spriteRenderer.material.SetColor("_Color", colorNegative);
				break;
			case Polarization.positive:
				spriteRenderer.material.SetColor("_Color", colorPositive);
				break;
			case Polarization.neutral:
				spriteRenderer.material.SetColor("_Color", Color.white);
				affectedByMagnets.Clear();
				break;
		}
	}
}
