using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowDice : MonoBehaviour
{
	public float Speed = 125;

	void Update()
	{
		transform.Rotate(Vector3.left * Speed * Time.deltaTime, Space.Self);
	}
}
