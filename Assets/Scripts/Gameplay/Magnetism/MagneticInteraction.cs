using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MagneticInteraction : Magnet
{
	[Header("Color")]
	[SerializeField] private Color colorChangeTarget;
	[SerializeField] private Color colorChangeNewColor;
	
	[Header("Events")]
	public UnityEvent OnInteraction;
	public bool isPush { get; private set; }

	#region Magnet functionality
	public override void ApplyMagneticForce(Vector2 velocity)
	{
		magneticForce = velocity;
		isPush = (velocity.magnitude > 0) ? true : false;
		OnInteraction.Invoke();
	}
	public override void ChangePolarisation(Polarization newPolarization)
	{
		switch (newPolarization)
		{
			case Polarization.negative:
				currentCharge = Mathf.Abs(chargeValue) * -1;
				currentPolarization = Polarization.negative;
				spriteRenderer.material.EnableKeyword("CHANGECOLOR_OFF");
				break;
			case Polarization.positive:
				currentCharge = Mathf.Abs(chargeValue) * 1;
				currentPolarization = Polarization.positive;
				spriteRenderer.material.EnableKeyword("CHANGECOLOR_ON");
				spriteRenderer.material.SetFloat("_ColorChangeTolerance", 0.3f);
				spriteRenderer.material.SetColor("_ColorChangeTarget", colorChangeTarget);
				spriteRenderer.material.SetColor("_ColorChangeNewCol", colorChangeNewColor);
				spriteRenderer.material.SetFloat("_ColorChangeLuminosity", 0.5f);
				break;
			case Polarization.neutral:
				currentCharge = 0;
				currentPolarization = Polarization.neutral;
				spriteRenderer.material.EnableKeyword("CHANGECOLOR_OFF");
				break;
		}
	}
	#endregion
}
