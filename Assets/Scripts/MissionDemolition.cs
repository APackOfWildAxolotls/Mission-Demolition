using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;

    [Header("Set in Inspector")]
    public Text uitLevel;
    public Text uitShots;
    public Text uitButton;
    public Text uitPower;
    public Vector3 castlePOS;
    public GameObject[] castles;
    public GameObject prefabProjectile;
    public GameObject Slingshot;

    [Header("Set Dynamically")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";
    public bool powerUpUsed = false;
    public GameObject projectile;

    // Start is called before the first frame update
    void Start()
    {
        S = this;
        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel()
    {
        if (castle != null)
        {
            Destroy(castle);
        }

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach(GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePOS;
        shotsTaken = 0;

        Goal.goalMet = false;
        UpdateGUI();
        mode = GameMode.playing;
    }

    void UpdateGUI()
    {
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGUI();
        if ((mode == GameMode.playing) && Goal.goalMet)
        {
            mode = GameMode.levelEnd;
            SwitchView("Show Both");
            Invoke("NextLevel", 2f);
        }

        if (Input.GetKeyDown("e"))
        {
            if (FollowCam.POI == null) 
            {
                print("No Shots Fired");
            }
            else
            {
                if (!powerUpUsed)
                {
                    Rigidbody PRRigid = FollowCam.POI.GetComponent<Rigidbody>();

                    projectile = Instantiate(prefabProjectile) as GameObject;
                    projectile.transform.position = FollowCam.POI.transform.position + new Vector3(0, 2, 0);
                    projectile.GetComponent<Rigidbody>().velocity = PRRigid.velocity;

                    projectile = Instantiate(prefabProjectile) as GameObject;
                    projectile.transform.position = FollowCam.POI.transform.position + new Vector3(0, -2, 0);
                    projectile.GetComponent<Rigidbody>().velocity = PRRigid.velocity;
                    powerUpUsed = true;
                    uitPower.text = "Split Shot: 0";
                }
                else if (powerUpUsed)
                {
                    print("No Power Ups");
                }
            }

        }



    }

    void NextLevel()
    {
        level++;
        if(powerUpUsed == true)
        {
            powerUpUsed = false;
            uitPower.text = "Split Shot(press e): 1";
        }

        if(level == levelMax)
        {
            level = 0;
        }

        StartLevel();
    }

    public void SwitchView(string eView = "")
    {
        if (eView == "")
        {
            eView = uitButton.text;
        }
        showing = eView;
        switch(showing)
        {
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.text = "Show Castle";
                break;
            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.text = "Show Both";
                break;
            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.text = "Show Slingshot";
                break;

        }
    }

    public static void ShotsFired()
    {
        S.shotsTaken++;
    }

}
