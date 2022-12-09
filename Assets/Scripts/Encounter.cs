using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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

    TMPro.TextMeshProUGUI encountertext;

    public Encounter(EncounterManager manager, EncounterTypes type) {

        //this function is called when the encounter is first generated (off screen);

        this.manager = manager;

        GC = manager.gameObject.GetComponent<GameController>();

        me = GameObject.Instantiate(manager.EncounterPrefab, new Vector3(GC.playerObj.transform.position.x + 6, -0.88f, 0), Quaternion.identity);

        me.name = System.Enum.GetNames(typeof(EncounterTypes))[(int)type] + ": " + name;

        this.type = type;

        characterEncounter = true;

        if (GameObject.Find("EncounterText") != null){
          encountertext = GameObject.Find("EncounterText").GetComponent<TextMeshProUGUI>();
        }

        switch (type)
            {
                case EncounterTypes.CHEST:

                    characterEncounter = false;
                    name = "Chest";
                    SetEncounterText("Open the chest?");

                break;

                case EncounterTypes.BOUNTY:

                    _base = manager.ChooseEncounterCharacter(type, true);
                    name = _base.character_name;
                    encounterDialog = _base.bounty_dialogue;
                    SetEncounterText(me.name + _base.bounty_dialogue);

                break;

                case EncounterTypes.FREE_ME:

                    _base = manager.ChooseEncounterCharacter(type, true);
                    price = _base.free_me_price_O2;
                    name = _base.character_name;
                    SetEncounterText(me.name + _base.freeme_dialogue);

                break;

                case EncounterTypes.PAY_ME:

                    _base = manager.ChooseEncounterCharacter(type, true);
                    price = _base.pay_me_price_food;
                    name = _base.character_name;
                    SetEncounterText(me.name + _base.payme_dialogue);

                break;

            }

        me.GetComponent<EncounterScript>().Setup(type, this);

        

    }
    public void startEncounter() {

        switch (type)
        {

            case EncounterTypes.PAY_ME:
                Debug.Log("");

                break;

            case EncounterTypes.BOUNTY:
                Debug.Log("");
                FightPlayer();

                break;

            case EncounterTypes.FREE_ME:
                Debug.Log("");
                Character ch = new Character(_base);

                GC.playerTeam.AddCharacter(ch);

                break;

            case EncounterTypes.CHEST:
                Debug.Log("");

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
                  "Base: " + name + "\n");
                   
    }

    public void SetEncounterText(string text) {

        if (encountertext != null) { encountertext.text = text; }

    }

}
public enum EncounterTypes
{
    FREE_ME,
    PAY_ME,
    BOUNTY,
    CHEST
};
