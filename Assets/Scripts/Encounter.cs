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

    public int price; // price for whatever type of encounter this is

    public CharacterBase _base;

    TMPro.TextMeshProUGUI encountertext;

    TMPro.TextMeshProUGUI encounterSubtext;

    public Encounter(EncounterManager manager, EncounterTypes type) {

        //this function is called when the encounter is first generated (off screen);

        this.manager = manager;

        GC = manager.gameObject.GetComponent<GameController>();

        me = GameObject.Instantiate(manager.EncounterPrefab, new Vector3(GC.playerObj.transform.position.x + 6, -0.88f, 0), Quaternion.identity);

        this.type = type;

        characterEncounter = true;

        if (GameObject.Find("EncounterText") != null){

            encountertext = manager.encounterCanvasText;

            encounterSubtext = manager.encounterCanvasSubText;

        }

        me.name = System.Enum.GetNames(typeof(EncounterTypes))[(int)type] + ": ";

        switch (type)
        {
            case EncounterTypes.CHEST:

                characterEncounter = false;
                name = "Chest";
                SetEncounterText("Open the chest?");

                break;

            case EncounterTypes.BOUNTY:

                int val = (int)Random.Range(0, 10);
                if (val > 5)
                {
                    _base = manager.ChooseEncounterCharacter(type, true);
                }
                else
                {
                    _base = manager.ChooseEncounterCharacter(type, false);
                }

                name = _base.character_name + ": ";
                encounterDialog = _base.bounty_dialogue;

                SetEncounterSubText("Pay Your Bounty? (" + GameController.bounty + ")" );
                    SetEncounterText(name + _base.bounty_dialogue);

                break;

                case EncounterTypes.FREE_ME:

                int val2 = (int)Random.Range(0, 10);
                if (val2 > 6)
                {
                    _base = manager.ChooseEncounterCharacter(type, true);
                }
                else
                {
                    _base = manager.ChooseEncounterCharacter(type, false);
                }

                price = _base.free_me_price_O2;

                name = _base.character_name + ": ";

                    SetEncounterSubText("Free This Prisoner?(" + price + " Oxygen)");
                    SetEncounterText(_base.freeme_dialogue);

                break;

                case EncounterTypes.PAY_ME:

                int val3 = (int)Random.Range(0, 10);
                if (val3 > 7)
                {
                    _base = manager.ChooseEncounterCharacter(type, true);
                }
                else
                {
                    _base = manager.ChooseEncounterCharacter(type, false);
                }

                price = _base.pay_me_price_food;

                name = _base.character_name + ": ";

                    SetEncounterSubText("Recruit This Prisoner?(" + price + " Fish food)");

                    SetEncounterText(name + _base.payme_dialogue);

                break;

            }

        me.name += name;

        me.GetComponent<EncounterScript>().Setup(type, this);

    }
    public void ExecuteEncounter() {
        // do the encounters intended effect
        switch (type)
        {

            case EncounterTypes.PAY_ME:
                //Debug.Log("");
                Character ch = new Character(_base);
                GC.playerTeam.AddCharacter(ch);

                manager.GC.ThinNotification(ch._base.character_name + ": Joined your team", ch._base.icon, 1);

                EndEncounter();

                Hide();

                break;

            case EncounterTypes.BOUNTY:
                //Debug.Log("");
                FightPlayer();

                me.GetComponent<SpriteRenderer>().sprite = manager.fishfoodSprite;

                break;

            case EncounterTypes.FREE_ME:
                //Debug.Log("");
                Character ch1 = new Character(_base);
                me.GetComponent<SpriteRenderer>().sprite = _base.character_sprite;
                GC.playerTeam.AddCharacter(ch1);

                manager.GC.ThinNotification(ch1._base.character_name + ": Joined your team", ch1._base.icon, 1);

                EndEncounter();
                break;

            case EncounterTypes.CHEST:
                //Debug.Log("");
                EndEncounter();
                break;
        }

    }
    public void EndEncounter() {

        GameController.gameState = GameState.OVERWORLD;

        Debug.Log(GameController.gameState);

        manager.HideEncounter();

        me.GetComponent<BoxCollider2D>().enabled = false;

    }
    public void Hide() {

        me.GetComponent<SpriteRenderer>().enabled = false;
    
    }
    void FightPlayer() {

        //creates a new character based on the selected Character
        Character newChar;
        newChar = new Character(_base);

        if (!_base.isRightFacing)
        {

            newChar.FlipSpriteOnX();

        }

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
    public void SetEncounterSubText(string text)
    {

        if (encounterSubtext != null) { encounterSubtext.text = text; }

    }

}
public enum EncounterTypes
{
    FREE_ME,
    PAY_ME,
    BOUNTY,
    CHEST
};
