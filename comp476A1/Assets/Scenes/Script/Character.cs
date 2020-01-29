using System.Collections;
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

	public float vMax;

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

	float unfreezeTime;

	float startUnfreeze;

	void Start()
	{
		//cache the animator
		//_animator = GetComponent<Animator>();

		moveType = 0;

		move = new MoveStrategy();

		vMax = m_maxVelocity;

		unfreezeTime = 5.0f;

	}

	void Update()
	{
		if (GameController.isTagGame)
		{
			//R3
			gameOfFreeTag();
		}
		else
		{
			//R2
			humanCharacterBehaviour();
		}
		


		//adjust spot angle

		float ratio = 1 - (_velocity) / (vMax * 1.5f);
		line1.localRotation = Quaternion.Euler(
			new Vector3(
				0.0f,
				m_spotAngle * (ratio) / 2.0f,
				0.0f));
		line2.localRotation = Quaternion.Euler(
			new Vector3(
				0.0f,
				-m_spotAngle * (ratio) / 2.0f,
				0.0f));



		// TODO: Translate the game object in world space
		transform.position += transform.forward * Time.deltaTime * _velocity;


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


		


		// set the blend parameter in your animator's movement blend tree
		_animator.SetFloat("Blend", _velocity / m_maxVelocity);

	}

	void humanCharacterBehaviour()
	{
		vMax = m_maxVelocity;

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
					Vector3 vel = move.fleeKinematic(tar.transform.position, transform.position, vMax);
					_velocity = vel.magnitude;

					if (shortestDis < rSat)
					{
						if (vel.magnitude != 0)
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
						if (transform.forward != vel.normalized)
						{
							transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vel.normalized), rotationDegreesPerSecond * Time.deltaTime);
						}

					}
					//transform.position = tar.transform.position;
					break;
				}

			case 0:// kinematic arrive A 
				{

					Vector3 vel = move.arriveKinematic(tar.transform.position, transform.position, vMax, rSat, t2t, false);
					_velocity = vel.magnitude;

					if (_velocity < 0.03 && shortestDis <= rSat)
					{
						_velocity = 0;
						transform.position = tar.transform.position;
					}
					else
					{
						Vector3 dir = tar.transform.position - transform.position; ;

						float angle = Vector3.Angle(dir, transform.forward);
						float spotAngle = line1.localEulerAngles.y;

						if (angle > spotAngle)
						{
							_velocity = 0;
						}
						else
						{
							_velocity = vel.magnitude;

						}
						transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vel.normalized), rotationDegreesPerSecond * Time.deltaTime);
					}


					break;
				}
		}

	}

	void gameOfFreeTag()
	{
		vMax = m_maxVelocity;

		if (tag == "tagged")
		{
			taggedAction();
		}
		else if (tag == "pursued")
		{
			pursuedAction();
		}
		else if (tag == "frozen")
		{
			frozenAction();
		}
		else if (tag == "untagged")
		{
			untaggedAction();
		}

	}

	void taggedAction()
	{
		GameObject[] untagged = GameObject.FindGameObjectsWithTag("untagged");
		GameObject[] pursued = GameObject.FindGameObjectsWithTag("pursued");

		if (untagged.Length != 0 ||pursued.Length!=0)
		{
			//no target
			if (pursued.Length == 0)
			{
				GameObject bestTar = null;

				float shortestDis = 0;


				for (int i = 0; i < untagged.Length; ++i)
				{
					if (i == 0)
					{
						Vector3 dir = untagged[i].transform.position - transform.position;
						float dis = dir.magnitude;
						shortestDis = dis;
						bestTar = untagged[i];
					}
					else
					{
						Vector3 dir = untagged[i].transform.position - transform.position;
						float dis = dir.magnitude;
						if (dis < shortestDis)
						{
							shortestDis = dis;
							bestTar = untagged[i];
						}
					}
				}

				bestTar.tag = "pursued";
				bestTar.transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(1.0f, 1.0f, 0.0f);
			}
			else // there is a target
			{
				//do pursue
				vMax = m_maxVelocity * 1.5f;
				Vector3 currentTarPos = pursued[0].transform.position;
				Vector3 tarPos2 = new Vector3();

				//if (Mathf.Abs(currentTarPos.x) > 0.25 * movingAreaX)
				//{
				//	if (transform.position.x > 0)
				//	{
				//		tarPos2.x += movingAreaX;
				//	}
				//	else
				//	{
				//		tarPos2.x -= movingAreaX;

				//	}
				//}

				//if (Mathf.Abs(currentTarPos.z) > 0.25 * movingAreaX)
				//{
				//	if (transform.position.z > 0)
				//	{
				//		tarPos2.z += movingAreaX;
				//	}
				//	else
				//	{
				//		tarPos2.z -= movingAreaX;

				//	}
				//}

				Vector3 cDir = currentTarPos - transform.position;
				//Vector3 cDir2 = tarPos2 - transform.position;

				//if(cDir.magnitude> cDir2.magnitude)
				//{
				//	cDir = cDir2;
				//	currentTarPos = tarPos2;
				//}


				Vector3 nextTarPos = pursued[0].transform.forward * vMax + currentTarPos;
				if (cDir.magnitude < 3)
				{
					pursued[0].tag = "frozen";
					pursued[0].transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(0.0f, 0.0f, 1.0f);

					_velocity = 0;
					LastFrozen.lastFrozenList.Add(pursued[0]);
				}
				else
				{
					Vector3 vel = move.seekKinematic(nextTarPos, transform.position, vMax);
					_velocity = vel.magnitude;
					if (vel.magnitude != 0)
					{
						transform.forward = vel.normalized;
					}
				}

			}
		}
		else
		{//game over & reset
			LastFrozen.lastFrozenList[LastFrozen.lastFrozenList.Count - 1].transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(1.0f, 0.0f, 0.0f);

			float edge = 40;
			Vector3 initPos = new Vector3(
				Random.Range(0.7f * -edge, 0.7f * edge),
				0.0f,
				Random.Range(0.7f * -edge, 0.7f * edge));
			Quaternion initRota = Quaternion.Euler(new Vector3(
				0.0f,
				Random.Range(0.0f, 360.0f),
				0.0f
				));
			LastFrozen.lastFrozenList[LastFrozen.lastFrozenList.Count - 1].transform.position = initPos;
			LastFrozen.lastFrozenList[LastFrozen.lastFrozenList.Count - 1].transform.rotation = initRota;
			LastFrozen.lastFrozenList[LastFrozen.lastFrozenList.Count - 1].tag = "tagged";


			tag = "untagged";
			transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(1.0f, 1.0f, 1.0f);

			initPos = new Vector3(
			Random.Range(0.7f * -edge, 0.7f * edge),
			0.0f,
			Random.Range(0.7f * -edge, 0.7f * edge));
			initRota = Quaternion.Euler(new Vector3(
				0.0f,
				Random.Range(0.0f, 360.0f),
				0.0f
				));

			transform.position = initPos;
			transform.rotation = initRota;
			_velocity = 0;

			GameObject[] frozen = GameObject.FindGameObjectsWithTag("frozen");
			for (int i = 0; i < frozen.Length; ++i)
			{
				frozen[i].tag = "untagged";
				frozen[i].transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(1.0f, 1.0f, 1.0f);

				initPos = new Vector3(
				Random.Range(0.7f * -edge, 0.7f * edge),
				0.0f,
				Random.Range(0.7f * -edge, 0.7f * edge));
				initRota = Quaternion.Euler(new Vector3(
					0.0f,
					Random.Range(0.0f, 360.0f),
					0.0f
					));

				frozen[i].transform.position = initPos;
				frozen[i].transform.rotation = initRota;

			}

		}
	}

	void pursuedAction()
	{
		vMax = m_maxVelocity * 1.0f;

		GameObject tagged = GameObject.FindGameObjectWithTag("tagged");

		Vector3 tarPos = tagged.transform.position;

		//Vector3 tarPos2 = new Vector3();

		//if (Mathf.Abs(tarPos.x) > 0.25 * movingAreaX)
		//{
		//	if (transform.position.x > 0)
		//	{
		//		tarPos2.x += movingAreaX;
		//	}
		//	else
		//	{
		//		tarPos2.x -= movingAreaX;

		//	}
		//}

		//if (Mathf.Abs(tarPos.z) > 0.25 * movingAreaX)
		//{
		//	if (transform.position.z > 0)
		//	{
		//		tarPos2.z += movingAreaX;
		//	}
		//	else
		//	{
		//		tarPos2.z -= movingAreaX;

		//	}
		//}


		Vector3 cDir = tarPos - transform.position;
		//Vector3 cDir2 = tarPos2 - transform.position;
		//if (cDir.magnitude > cDir2.magnitude)
		//{
		//	cDir = cDir2;
		//}



		Vector3 vel = move.fleeKinematic(tagged.transform.position, transform.position, vMax);
		_velocity = vel.magnitude;

		if (cDir.magnitude < rSat)
		{
			if (vel.magnitude != 0)
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
			if (transform.forward != vel.normalized)
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vel.normalized), rotationDegreesPerSecond * Time.deltaTime);
			}

		}


	}

	void frozenAction()
	{
		_velocity = 0;
	}

	void untaggedAction()
	{
		vMax = m_maxVelocity * 0.9f;

		GameObject[] frozens = GameObject.FindGameObjectsWithTag("frozen");

		GameObject bestTar = null;

		float shortestDis = 0;


		for (int i = 0; i < frozens.Length; ++i)
		{
			if (i == 0)
			{
				Vector3 dir = frozens[i].transform.position - transform.position;
				float dis = dir.magnitude;
				shortestDis = dis;
				bestTar = frozens[i];
			}
			else
			{
				Vector3 dir = frozens[i].transform.position - transform.position;
				float dis = dir.magnitude;
				if (dis < shortestDis)
				{
					shortestDis = dis;
					bestTar = frozens[i];
				}
			}
		}

		GameObject tagged = GameObject.FindGameObjectWithTag("tagged");

		Vector3 fToTDir = new Vector3();
		if(bestTar != null)
		{
			fToTDir = bestTar.transform.position - tagged.transform.position;
		}
		Vector3 toTDir = transform.position - tagged.transform.position;

		//if (frozens.Length >0 && fToTDir.magnitude > 5 && toTDir.magnitude >30)
		Debug.Log(frozens.Length);
		if (bestTar == null||frozens.Length == 0 || fToTDir.magnitude < 5 || toTDir.magnitude < 30)
		{

			Vector3 vel = move.fleeKinematic(tagged.transform.position, transform.position, vMax);
			_velocity = vel.magnitude * (1- toTDir.magnitude/50);

			if (toTDir.magnitude < rSat)
			{
				if (vel.magnitude != 0)
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
				if (transform.forward != vel.normalized)
				{
					transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vel.normalized), rotationDegreesPerSecond * Time.deltaTime);
				}

			}
		}
		else
		{

			Vector3 vel = move.arriveKinematic(bestTar.transform.position, transform.position, vMax, rSat, t2t, false);
			_velocity = vel.magnitude;

			if (shortestDis <= 2)
			{
				_velocity = 0;
				transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(0.0f, 1.0f, 0.0f);
				if (Time.time - startUnfreeze >= unfreezeTime)
				{
					bestTar.tag = "untagged";
					bestTar.transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(1.0f, 1.0f, 1.0f);
				}
			}
			else
			{
				startUnfreeze = Time.time;
				transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(1.0f, 1.0f, 1.0f);

				Vector3 dir = bestTar.transform.position - transform.position; ;

				float angle = Vector3.Angle(dir, transform.forward);
				float spotAngle = line1.localEulerAngles.y;
				//Debug.Log(angle);
				if (angle > spotAngle)
				{
					_velocity = 0;
				}
				else
				{
					_velocity = vel.magnitude;

				}
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vel.normalized), rotationDegreesPerSecond * Time.deltaTime);
			}

		}



	}

}
