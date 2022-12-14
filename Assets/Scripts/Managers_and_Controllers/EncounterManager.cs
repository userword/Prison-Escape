using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EncounterManager : MonoBehaviour
{
    // Stores all the Unique enemies data
    private List<CharacterBase> Characters;

    // Stores all the Repeating "Generic Enemies data
    private List<CharacterBase> repeatCharacters;

    public GameObject EncounterPrefab; // phase these out! // nvm lol

    GameObject player;
    MovementScript movementScript;

    public Sprite chestSprite;
    public Sprite randomPrisonerSprite;
    public Sprite fishfoodSprite;

    Canvas EncounterCanvas;

    //assigned via inspector
    public GameObject encounterCanvasTextObject;
    public TMPro.TextMeshProUGUI encounterCanvasText;
    public TMPro.TextMeshProUGUI encounterCanvasSubText;

    public Encounter active;

    public static int level = 1; // stores the games current Level
    public static EncounterTypes desired_type;

    public GameController GC;


    PlayerInfo playerInfo;

    //For the steps thing probably move this to the Game controller
    int steps = 0;
    int root = 0;
    int range = 1;

    public void step() {

        steps++;
        player.GetComponent<PlayerInfo>().loseO2(Random.Range(1,4)); //random range from 1 - 4 excluding 4

    }
    private void Awake()
    {
        GC = this.gameObject.GetComponent<GameController>();

        Characters = new List<CharacterBase>();
        repeatCharacters = new List<CharacterBase>();

        player = GameObject.Find("Player");
        movementScript = player.GetComponent<MovementScript>();

        playerInfo = player.GetComponent<PlayerInfo>();

        EncounterCanvas = GameObject.Find("GameController").GetComponent<GameController>().EncounterCanvas;

        // encounterCanvasText = encounterCanvasTextObject.GetComponent<TMPro.TextMeshProUGUI>();

        // put this all in a function somewhere Jesus

        Debug.Log("Attempting to Load characters");
        Object[] chars = Resources.LoadAll("Unique", typeof(CharacterBase));

        foreach (var cbase in chars) {
            if (cbase != null)
            {
                //Debug.Log("Attempting to add " + cbase + "To Character Database");
                Characters.Add(cbase as CharacterBase); // Load all CharacterBases into a list
            }
        }

        Object[] repeat_chars = Resources.LoadAll("Repeating", typeof(CharacterBase));

        foreach (var cbase in repeat_chars)
        {
            if (cbase != null)
            {
                //Debug.Log("Attempting to add " + cbase + "To repeating Character Database");
                repeatCharacters.Add(cbase as CharacterBase); // Load all CharacterBases into a list
            }
        }
    }
    void Update() {
        if (steps > root + range) {

            //Debug.Log("Generating Encounter...");
            root = steps;
            GenerateRandomEncounter();

        }
    }
    void GenerateRandomEncounter() {

        Encounter encounter = new Encounter(this, ChooseEncounterType());

    }
    private static bool FindCharacterBasedOnLevel(CharacterBase cb)
    {
        return cb.GetLevel() == EncounterManager.level;
    }
    private static bool FindCharacterBasedOnType(CharacterBase cb)
    {
        return cb.IsTypeUsable(desired_type);
    }
    public CharacterBase ChooseEncounterCharacter(EncounterTypes type, bool unique) {

        CharacterBase result;
        List<CharacterBase> sample;
        List<CharacterBase> sample2;

        if (unique)
        {

        sample = Characters.FindAll(FindCharacterBasedOnLevel); // find characters of the right level

        desired_type = type;

        sample2 = sample.FindAll(FindCharacterBasedOnType); // and of the right type

            if (sample2.Count != 0)
            {

                // Gving an error find out why
                
                int val = (int)Random.Range(0, sample2.Count-1);
               
                result = sample2[val];

                // Since they are Unique
                // Error happens below. When characters are removed, eventually sample.Count = 0 and it breaks
                Characters.Remove(result);

            }
            else
            {
                if (sample2.Count == 0 && EncounterManager.level == 3)
                {


                    // You win!


                }
                EncounterManager.level++;
                return ChooseEncounterCharacter(type, unique);
            }

        }
        else
        {

        sample2 = repeatCharacters.FindAll(FindCharacterBasedOnLevel);

            if (sample2 != null)
            {
                int val = (int)Random.Range(0, sample2.Count-1);
                result = repeatCharacters[val];
            }
            else { return null; }

        }
        return result;
    }
    public static int GenerateRandomNumber() {

        //free me, pay me, bounty, chest

        int[] nums = new int[] { 0, 0, 1, 2, 2, 2, 2,};

        int val = (int)Random.Range(0, nums.Length);

        return nums[val];

    }
    public EncounterTypes ChooseEncounterType()
    {
        EncounterTypes chosenType;

        int numEncounterTypes = System.Enum.GetNames(typeof(EncounterTypes)).Length;

        int val = GenerateRandomNumber();

        chosenType = (EncounterTypes)val;

        //print("Type Chosen: " + chosenType);

        return chosenType;

    }
    public int getLevel() { return level; }
    public void DisplayEncounter() {

    EncounterCanvas.enabled = true;

    GameController.gameState = GameState.BUSY;

        //encounterCanvasText.text = System.Enum.GetNames(typeof(EncounterTypes))[(int)active.type];
        //encounterCanvasText.text = "work please";

        Debug.Log("Display should say :" + active.type);

    }
    public void HideEncounter()
    {

        EncounterCanvas.enabled = false;

    }
    public void AcceptEncounter() {

        switch (active.type) {

            case EncounterTypes.BOUNTY:

                if (playerInfo.getFF() >= GameController.bounty)
                {

                    playerInfo.loseFF(GameController.bounty);

                    GameController.bounty += 10;

                    active.EndEncounter();
                }

                break;

            case EncounterTypes.CHEST:

                active.ExecuteEncounter();
                EncounterCanvas.enabled = false;

                break;

            case EncounterTypes.FREE_ME:

                if (GC.playerTeam.full)
                {

                    GC.ThinNotification("Your Team is Full!", null, 1);

                }
                else {

                    if (playerInfo.get02() >= active.price)
                    {
                        playerInfo.loseO2(active.price);

                        active.ExecuteEncounter();

                    }

                }

                break;

            case EncounterTypes.PAY_ME:

                if (GC.playerTeam.full)
                {

                    GC.ThinNotification("Your Team is Full!", null , 1);

                }
                else
                {

                    if (playerInfo.getFF() >= active.price)
                    {
                        playerInfo.loseFF(active.price);

                        active.ExecuteEncounter();
                    }
                }

                break;

        }

    }
    public void DeclineEncounter()
    {

        switch (active.type)
        {

            case EncounterTypes.BOUNTY:
                active.ExecuteEncounter();
                break;

            case EncounterTypes.CHEST:
                active.EndEncounter();
                break;

            case EncounterTypes.FREE_ME:
                active.EndEncounter();
                break;

            case EncounterTypes.PAY_ME:
                active.EndEncounter();
                break;

        }

        EncounterCanvas.enabled = false;

    }

    //Player makes x steps
    //Generate Encounter Box
    //On collision with Encounter Box
    //  Display options GUI
    //  Do Options

}
