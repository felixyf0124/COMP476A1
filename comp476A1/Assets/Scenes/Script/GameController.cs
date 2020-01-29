using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GameController : MonoBehaviour
{

    public GameObject character;

    public int numberOfMember;

    public int teamABehaviourType;

    public int teamBBehaviourType;

    public float maxVelocity;

    public float acceleration;

    public float deccelerationMultiplyer;

    public float rSat;

    public float t2t;

    List<GameObject> teamAList;
    List<GameObject> teamBList;

    bool isTeamA;
    // Start is called before the first frame update
    void Start()
    {
        isTeamA = true;
        teamAList = new List<GameObject>();
        teamBList = new List<GameObject>();
        const float edge = 50.0f;
        const float angle = 45.0f;

        for (int i = 0; i < numberOfMember; ++i)
        {
            
            Vector3 initPos = new Vector3(
                Random.Range(0.3f * edge, 0.7f * edge), 
                0.0f,
                Random.Range(0.3f * edge, 0.7f * edge));
            Quaternion initRota =Quaternion.Euler(new Vector3(
                0.0f, 
                Random.Range(-angle + 180.0f, angle + 180.0f),
                0.0f
                ));
            GameObject teamAChar = Instantiate(character, initPos, initRota);
            teamAChar.GetComponent<Character>().m_maxVelocity = maxVelocity;
            teamAChar.GetComponent<Character>().m_acceleration = acceleration;
            teamAChar.GetComponent<Character>().m_deccelerationMultiplier = deccelerationMultiplyer;
            teamAChar.GetComponent<Character>().rSat = rSat;
            teamAChar.GetComponent<Character>().t2t = t2t;
            teamAChar.GetComponent<Character>().moveType = teamABehaviourType;
            teamAChar.transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(1.0f,0.0f,0.0f);
            teamAChar.tag = "A";
            teamAList.Add(teamAChar);
            
        }

        for (int i = 0; i < numberOfMember; ++i)
        {
            Vector3 initPos = new Vector3(
                Random.Range(0.3f * -edge, 0.7f * -edge),
                0.0f,
                Random.Range(0.3f * -edge, 0.7f * -edge));
            Quaternion initRota = Quaternion.Euler(new Vector3(
                0.0f,
                Random.Range(-45.0f, 45.0f),
                0.0f
                ));
            GameObject teamBChar = Instantiate(character, initPos, initRota);
            teamBChar.GetComponent<Character>().m_maxVelocity = maxVelocity;
            teamBChar.GetComponent<Character>().m_acceleration = acceleration;
            teamBChar.GetComponent<Character>().m_deccelerationMultiplier = deccelerationMultiplyer;
            teamBChar.GetComponent<Character>().rSat = rSat;
            teamBChar.GetComponent<Character>().t2t = t2t;
            teamBChar.GetComponent<Character>().moveType = teamBBehaviourType;
            teamBChar.transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(0.0f, 0.0f, 0.9f);
            teamBChar.tag = "B";
            //teamBChar
            teamBList.Add(teamBChar);

        }

    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKey(KeyCode.Alpha1))
        {
            if (isTeamA)
            {
                for (int i = 0; i < teamAList.Count; ++i)
                {
                    teamAList[i].GetComponent<Character>().moveType = 0;
                }
            }
            else
            {
                for (int i = 0; i < teamAList.Count; ++i)
                {
                    teamBList[i].GetComponent<Character>().moveType = 0;
                }
            }
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            if (isTeamA)
            {
                for (int i = 0; i < teamAList.Count; ++i)
                {
                    teamAList[i].GetComponent<Character>().moveType = 1;
                }
            }
            else
            {
                for (int i = 0; i < teamAList.Count; ++i)
                {
                    teamBList[i].GetComponent<Character>().moveType = 1;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isTeamA = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isTeamA = true;
        }

    }
}
