using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticObject : Magnet
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Magnet"))
		{
			magnetsInRange.Add(other.GetComponent<Magnet>());
			other.GetComponentInParent<Magnet>().inRangeOfMagnets.Add(this);
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Magnet"))
		{
			magnetsInRange.Remove(other.GetComponent<Magnet>());
			other.GetComponentInParent<Magnet>().inRangeOfMagnets.Remove(this);
		}
	}
}
