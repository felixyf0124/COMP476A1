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
    List<GameObject> freezeTagObjs;


    bool isTeamA;

    public static bool isTagGame;
    // Start is called before the first frame update
    void Start()
    {
        isTeamA = true;
        isTagGame = false;
        teamAList = new List<GameObject>();
        teamBList = new List<GameObject>();
        freezeTagObjs = new List<GameObject>();

        gameInit();
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

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            isTagGame = !isTagGame;

            gameInit();
        }

    }

    void gameInit()
    {
        if (isTagGame)
        {
            humanCharBehaviourDestroyer();
            freezeTagGameInit();
        }
        else
        {
            freezeTagGameDestroyer();
            humanCharBehaviourInit();
        }
    }

    void humanCharBehaviourInit()
    {
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
            Quaternion initRota = Quaternion.Euler(new Vector3(
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
            teamAChar.transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(1.0f, 0.0f, 0.0f);
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

    void humanCharBehaviourDestroyer()
    {
        for (int i = teamAList.Count; i > 0; --i)
        {
            Destroy(teamAList[i - 1]);
        }

        for (int i = teamBList.Count; i > 0; --i)
        {
            Destroy(teamBList[i - 1]);
        }
        teamAList = null;
        teamBList = null;
        
    }

    void freezeTagGameInit()
    {
        freezeTagObjs = new List<GameObject>();
        LastFrozen.lastFrozenList = new List<GameObject>();

        float edge = 40;

        for (int i = 0; i < numberOfMember * 2; ++i)
        {
            

            Vector3 initPos = new Vector3(
                Random.Range(0.7f * -edge, 0.7f * edge),
                0.0f,
                Random.Range(0.7f * -edge, 0.7f * edge));
            Quaternion initRota = Quaternion.Euler(new Vector3(
                0.0f,
                Random.Range(0.0f, 360.0f),
                0.0f
                ));

            GameObject freezeTagChar = Instantiate(character, initPos, initRota);
            freezeTagChar.GetComponent<Character>().m_maxVelocity = maxVelocity;
            freezeTagChar.GetComponent<Character>().m_acceleration = acceleration;
            freezeTagChar.GetComponent<Character>().m_deccelerationMultiplier = deccelerationMultiplyer;
            freezeTagChar.GetComponent<Character>().rSat = rSat;
            freezeTagChar.GetComponent<Character>().t2t = t2t;
            freezeTagChar.GetComponent<Character>().moveType = teamABehaviourType;

            if (i == 0)
            {
                freezeTagChar.tag = "tagged";
                freezeTagChar.transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(1.0f, 0.0f, 0.0f);
            }
            else
            {
                freezeTagChar.tag = "untagged";
                freezeTagChar.transform.GetChild(3).GetComponent<ParticleSystem>().startColor = new Color(1.0f, 1.0f, 1.0f);
            }
            freezeTagObjs.Add(freezeTagChar);

        }
    }

    void freezeTagGameDestroyer()
    {

        for (int i = freezeTagObjs.Count; i > 0; --i)
        {
            Destroy(freezeTagObjs[i - 1]);
        }
        freezeTagObjs = null;
    }
}
