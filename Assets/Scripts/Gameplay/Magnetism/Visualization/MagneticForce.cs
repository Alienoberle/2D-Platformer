using UnityEngine;

[System.Serializable]
public struct MagneticForce
{
	public Magnet _magnet1, _magnet2;
	public Vector2 _magnet1Point, _magnet2Point, _magneticVelocity, _direction;
	float _distance;

	public MagneticForce(Magnet magnet1, Magnet magnet2, Vector2 magnet1Point, Vector2 magnet2Point, Vector2 direction, float distance, Vector2 magneticVelocity)
	{
		_magnet1 = magnet1;
		_magnet2 = magnet2;
		_magnet1Point = magnet1Point;
		_magnet2Point = magnet2Point;
		_magneticVelocity = magneticVelocity;
		_direction = direction;
		_distance = distance;
	}
}
