using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. Asks to examine our water well
/// </summary>

public class WitchDoctorNPC : NPC
{

    [SerializeField] NPC _waterWell;
    [SerializeField] NPC _cart;

    [SerializeField] GameObject _mummyPrefab;
    private int _mummyCount = 0;

    // Use this for initialization
    void Start()
    {
        //Part 1
        this.Dialogue = "Please go and examine the Water Well.";
        this.OnTalk = () => {
            _waterWell.Dialogue = "The water is empty. I need to report this to the Doctor.";
            //Part 2
            _waterWell.OnTalk = () => {
                this.Dialogue = "I knew something weird was happening. Please, go and pull the lever behind the Cart on the east part of town.";
                //Part 3
                this.OnTalk = () => {
                    _cart.Dialogue = "\"Clank\". I should hurry and check the Well.";
                    //Part 4 (When we pull the lever)
                    _cart.OnTalk = () => {
                        _cart.OnTalk = null;
                        this.Dialogue = "Good Job, no check the water well again.";
                        _waterWell.Dialogue = "Oh! Something is coming up!";
                        _waterWell.OnTalk = () => {
                            CreateMummies();
                        };
                    };
                };
            };
        };

    }

    void CreateMummies()
    {
        //Prevent more mummies to come up
        _waterWell.OnTalk = null;
        _waterWell.Dialogue = "Mummies appeared";
        //Declare a GameObject called mummy
        GameObject mummy;
        //Make 1st mummy
        mummy = GameObject.Instantiate(_mummyPrefab);
        mummy.transform.position = new Vector3(4, 0, 0);
        mummy.GetComponent<EnemyController>().OnKilled = () => {
            AddKillAndCheckCompletion();
        };
        //Make 2nd mummy
        mummy = GameObject.Instantiate(_mummyPrefab);
        mummy.transform.position = new Vector3(9, 0, 0.5f);
        mummy.GetComponent<EnemyController>().OnKilled = () => {
            AddKillAndCheckCompletion();
        };
        //Make 3rd mummy
        mummy = GameObject.Instantiate(_mummyPrefab);
        mummy.transform.position = new Vector3(6.3f, 0, -0.5f);
        mummy.GetComponent<EnemyController>().OnKilled = () => {
            AddKillAndCheckCompletion();
        };
    }

    void AddKillAndCheckCompletion()
    {
        _mummyCount++;
        if (_mummyCount == 3)
        {
            Debug.Log("Quest finished");
            //Finish our quest
            //Reset dialogues
            _waterWell.Dialogue = "This is a water well";
            _cart.Dialogue = "This is an old wooden cart";
            //Set the witchdoctor reward
            this.Dialogue = "Congratulations, you saved us from those monsters. Please take this as a reward";
            this.OnTalk = () => {
                PlayerController.main.SetExperience(200);
                //give item
                //heal.. 
                this.OnTalk = null;
                this.Dialogue = "No go ahead and save the world!";
            };
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
