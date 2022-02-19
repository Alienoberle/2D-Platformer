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
}
