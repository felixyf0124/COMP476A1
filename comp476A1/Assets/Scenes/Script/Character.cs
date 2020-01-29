﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// accepts input and animates the player
public class Character : MonoBehaviour
{

	//the animator component attached to this game object
	Animator _animator;

	//veloctiy upper clamp
	public float m_maxVelocity;

	//acceleration in unity units per second
	public float m_acceleration;

	//deccelaration speed with respect to acceleration speed
	public float m_deccelerationMultiplier;

	public float rSat;

	public float t2t;

	public float m_spotAngle;

	//rotation speed
	public float rotationDegreesPerSecond = 360;

	// the current velocity
	float _velocity;

	// the input vector
	Vector2 _input = Vector2.zero;

	public Transform line1;

	public Transform line2;

	public int moveType;

	public float movingAreaX;

	public float movingAreaZ;

	MoveStrategy move;

	void Start()
	{
		//cache the animator
		_animator = GetComponent<Animator>();

		moveType = 0;

		move = new MoveStrategy();
	}

	void Update()
	{
		GameObject[] enemy;

		if (this.tag == "A")
		{
			enemy = GameObject.FindGameObjectsWithTag("B");

		}
		else
		{
			enemy = GameObject.FindGameObjectsWithTag("A");

		}


		GameObject tar = null;
		float shortestDis = 0;


		//dd.text = dd.text + "\n" + enemy.Length;

		for (int i = 0; i < enemy.Length; ++i)
		{
			if (i == 0)
			{
				Vector3 dir = enemy[i].transform.position - transform.position;
				float dis = dir.magnitude;
				shortestDis = dis;
				tar = enemy[i];
			}
			else
			{
				Vector3 dir = enemy[i].transform.position - transform.position;
				float dis = dir.magnitude;
				if (dis < shortestDis + 3)
				{
					shortestDis = dis;
					tar = enemy[i];
				}
			}

		}

		switch (moveType)
		{

			case 1: // kinematic flee C
				{
					Vector3 vel = move.fleeKinematic(tar.transform.position, transform.position, m_maxVelocity);
					_velocity = vel.magnitude;

					if (shortestDis < rSat)
					{	
						if(vel.magnitude != 0)
						{
							transform.position = transform.position + vel.normalized * rSat;

						}
						else
						{
							transform.position = transform.position + transform.forward * rSat;
						}

					}
					else
					{
						if(transform.forward != vel.normalized)
						{
							transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vel.normalized), rotationDegreesPerSecond * Time.deltaTime);
						}

					}
					//transform.position = tar.transform.position;
					break;
				}

			case 0:// kinematic arrive A 
				{
					
					Vector3 vel = move.arriveKinematic(tar.transform.position, transform.position, m_maxVelocity, rSat, t2t, false);
					_velocity = vel.magnitude;

					if (_velocity < 0.03 && shortestDis <= rSat)
					{
						_velocity = 0;
						transform.position = tar.transform.position;
						Debug.Log("called");
					}
					else
					{
						transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vel.normalized), rotationDegreesPerSecond * Time.deltaTime);
					}


					break;
				}
		}





		//// Obtain input information (See "Horizontal" and "Vertical" in the Input Manager)
		//float horizontal = Input.GetAxis("Horizontal");
		//float vertical = Input.GetAxis("Vertical");

		////cache the input
		//_input.x = horizontal;
		//_input.y = vertical;

		////calculate input magnitude (you may use this to assign the blend parameter in your movement
		//// blend tree directly, the acceleration system in this example is given to showcase its
		//// potential effect on a PC without a controller)
		//float inputMag = _input.magnitude;


		//// Check for inputs
		//if (!Mathf.Approximately(vertical, 0.0f) || !Mathf.Approximately(horizontal, 0.0f))
		//{
		//	Vector3 direction = new Vector3(horizontal, 0.0f, vertical);
		//	direction = Vector3.ClampMagnitude(direction, 1.0f);

		//	// increment velocity
		//	if (_velocity < m_maxVelocity)
		//	{
		//		_velocity += m_acceleration * Time.deltaTime;
		//		if (_velocity > m_maxVelocity)
		//			_velocity = m_maxVelocity;
		//	}

		//	// look towards the input direction
		//	transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationDegreesPerSecond * Time.deltaTime);


		//}
		//else if (_velocity > 0)
		//{

		//	//decrement velocity if there is no input
		//	_velocity -= m_acceleration * m_deccelerationMultiplier * Time.deltaTime;
		//	if (_velocity < 0)
		//		_velocity = 0;
		//}




		// TODO: Translate the game object in world space
		transform.position += transform.forward * Time.deltaTime * _velocity;
		//dd.text = transform.position.x + "|" + transform.position.y;
		Vector2 test = new Vector2(1, 1);
		//test.Normalize();


		//overlap mod
		float modx, modz;
		if (transform.position.x < 0)
		{
			modx = ((transform.position.x - movingAreaX / 2.0f) % movingAreaX + movingAreaX / 2.0f);
		}
		else
		{
			modx = ((transform.position.x + movingAreaX / 2.0f) % movingAreaX - movingAreaX / 2.0f);
		}

		if(transform.position.z <0)
		{
			modz = ((transform.position.z - movingAreaZ / 2.0f) % movingAreaZ + movingAreaZ / 2.0f);

		}
		else
		{
			modz = ((transform.position.z + movingAreaZ / 2.0f) % movingAreaZ - movingAreaZ / 2.0f);
		}

		transform.position = new Vector3(modx, transform.position.y, modz);


		//adjust spot angle

		float ratio = 1 - (_velocity) / (m_maxVelocity * 1.5f);
		line1.localRotation = Quaternion.Euler(
			new Vector3(
				0.0f,
				m_spotAngle * (ratio) / 2.0f, 
				0.0f));
		line2.localRotation = Quaternion.Euler(
			new Vector3(
				0.0f,
				- m_spotAngle * (ratio) / 2.0f, 
				0.0f));


		// set the blend parameter in your animator's movement blend tree
		_animator.SetFloat("Blend", _velocity / m_maxVelocity);

	}
}
