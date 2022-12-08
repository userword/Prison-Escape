using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Encounter
{
    GameObject me;

    GameController GC;

    EncounterManager manager;

    public EncounterTypes type;

    string name;

    string encounterDialog;

    public bool characterEncounter; //tracks if the enconter needs a character base or not.

    int price; // price for whatever type of encounter this is

    public CharacterBase _base;

    public Encounter(EncounterManager manager, EncounterTypes type) {

        this.manager = manager;

        GC = manager.gameObject.GetComponent<GameController>();

        me = GameObject.Instantiate(manager.EncounterPrefab, new Vector3(GC.playerObj.transform.position.x + 6, -0.88f, 0), Quaternion.identity);

        this.type = type;

        characterEncounter = true;

        switch (type)
            {
                case EncounterTypes.CHEST:
                characterEncounter = false;
                name = "Chest";

                break;

                case EncounterTypes.BOUNTY:
                _base = manager.ChooseEncounterCharacter(type, true);
                name = _base.character_name;
                break;

                case EncounterTypes.FREE_ME:
                _base = manager.ChooseEncounterCharacter(type, true);
                price = _base.free_me_price;
                name = _base.character_name;

                break;

                case EncounterTypes.PAY_ME:
                _base = manager.ChooseEncounterCharacter(type, true);
                price = _base.pay_me_price;
                name = _base.character_name;

                break;

            }

        me.GetComponent<EncounterScript>().Setup(type, this);

        me.name = System.Enum.GetNames(typeof(EncounterTypes))[(int)type] + ": " + name;

    }
    public void startEncounter() {

        switch (type)
        {

            case EncounterTypes.PAY_ME:
<<<<<<< HEAD
                //Debug.Log("Pay me Encounter");
                
=======
                Debug.Log("");

>>>>>>> b31f1ff2f1f20077d0cbfccad17776e023571ed7
                break;

            case EncounterTypes.BOUNTY:
                //Debug.Log("Bounty Encounter");
                FightPlayer();
                break;

            case EncounterTypes.FREE_ME:
                //Debug.Log("Free me Encounter");

                Character ch = new Character(_base);

                GC.playerTeam.AddCharacter(ch);

                break;

            case EncounterTypes.CHEST:
                //Debug.Log("Chest Encounter");

                break;
        }

    }
    void FightPlayer() {

        //creates a new character based on the selected Character
        Character newChar;
        newChar = new Character(_base);

        //Loads that Character into a new team
        Team enemy;
        enemy = new Team(newChar);

        //find the players team
        Team playersTeam = GC.playerTeam;

        GC.CM.startCombat(playersTeam, enemy);


    }
    public void Spill()
    {

        Debug.Log("Encounter of Type: " + type + "\n" +
                  "Base: " + name + "\n"
                   );
    }

}
public enum EncounterTypes
{
    FREE_ME,
    PAY_ME,
    BOUNTY,
    CHEST
};
