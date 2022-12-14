using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    OVERWORLD,
    PLAYERMOVE,
    ENEMYMOVE,
    BUSY
}
public class GameController : MonoBehaviour
{
    public static GameState gameState;

    public GameObject hpBarPrefab;
    public static GameObject hpBarPrefabStatic;

    public GameObject combatCanvasObject; // Canvas containing combat GUI
    public Canvas EncounterCanvas; // Canvas Showing options at time of encounter
    public Canvas CounterCanvas; // Canvas displaying O2 and Fish food

    CounterController CC; // Class controlling the Players O2 and Fish food GUI

    public CombatManager CM; // Class that manages combat

    public GameObject playerObj;
    [SerializeField] CharacterBase diver; // The Character base for the Player
    Character playerCharacter; // Character Object to manage player in Combat
    public Team playerTeam; // Team object to store the players team

    public static int bounty = 10;

    public GameObject NotificationPrefab;
    public GameObject NotificationThinPrefab;

    private void Awake()
    {
        gameState = GameState.OVERWORLD;
        Physics2D.gravity.Set(0, 0);

        //find and store relevant gameObjects and classes
        CC = GameObject.Find("CounterCanvas").GetComponent<CounterController>();

        EncounterCanvas = this.transform.Find("EncounterCanvas").GetComponent<Canvas>();
        CounterCanvas = this.transform.Find("CounterCanvas").GetComponent<Canvas>();

        hpBarPrefabStatic = hpBarPrefab;

    }
    void Start()
    {
        CC.Update02Counter();
        CC.UpdateFFCounter();

        EncounterCanvas.enabled = false;

        //Initialize objects
        CM = new CombatManager(this, combatCanvasObject);
        playerCharacter = new Character(diver);
        playerTeam = new Team(playerCharacter);
        playerTeam.playersTeam = true;

    }
    public static void switchCams()
    {
        Camera mainCamera;
        Camera combatCamera;

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        combatCamera = GameObject.Find("Combat Camera").GetComponent<Camera>();

        if (mainCamera.enabled == true)
        {

            mainCamera.enabled = false;
            combatCamera.enabled = true;

        }
        else if (combatCamera.enabled == true)
        {

            mainCamera.enabled = true;
            combatCamera.enabled = false;

        }
    }
    void Update() {

        if (gameState != GameState.OVERWORLD)
        {

           //CM.Update(playerTeam);

        }

        if (gameState == GameState.ENEMYMOVE)
        {
            gameState = GameState.PLAYERMOVE;

            CM.Update(playerTeam);

            if (CM.enemies != null) {

                CM.enemies.Attack(playerTeam);
                CM.Update(playerTeam);

            }



        }

    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))// for combat information
        {

            playerTeam.Log();

            CM.enemies.Log();

        }
    }
    public void AttackEnemies()
    {

        playerTeam.Attack(CM.enemies);
        gameState = GameState.ENEMYMOVE;

    }
    public void Swap(int index)
    {
        playerTeam.SwapByIndex(0,index);
        CM.Update(playerTeam);
        gameState = GameState.ENEMYMOVE;
    }
    public void FullNotification(string title, string msg, Sprite img, int secs)
    {

        GameObject temp = GameObject.Instantiate(NotificationPrefab, GameController.GetActiveCamera().gameObject.transform);

        temp.GetComponent<NotificationController>().setText(msg);

        temp.GetComponent<NotificationController>().setTitle(title);

        if (img) { 
        
            temp.GetComponent<NotificationController>().setImage(img);

        }
        
        temp.GetComponent<NotificationController>().Show();

        StartCoroutine(Hide(secs));

        IEnumerator Hide(int secs)
        {

            yield return new WaitForSeconds(secs);

            temp.GetComponent<NotificationController>().Hide();

            Object.Destroy(temp);

        }

    }
    public void BackInput(){

      CM.EndCombat();

    }
    public void ThinNotification(string msg, Sprite img, int secs)
    {

        GameObject temp = GameObject.Instantiate(NotificationThinPrefab, GameController.GetActiveCamera().gameObject.transform);

        temp.GetComponent<NotificationController>().setText(msg);

        if (img != null) {

         temp.GetComponent<NotificationController>().setImage(img);

        }

        temp.GetComponent<NotificationController>().Show();

        StartCoroutine(Hide(secs));

        IEnumerator Hide(int secs)
        {

            yield return new WaitForSeconds(secs);

            temp.GetComponent<NotificationController>().Hide();

            Object.Destroy(temp);

        }

    }
    public static Camera GetActiveCamera() {

        Camera mainCamera;
        Camera combatCamera;

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        combatCamera = GameObject.Find("Combat Camera").GetComponent<Camera>();

        if (mainCamera.enabled == true)
        {

            return mainCamera;

        }
        else {

            return combatCamera;
        
        }

    }

}
public class Team
{
    public List<Character> Characters;

    public bool playersTeam = false;

    public bool defeated = false;

    public bool full = false;

    public int reward;
    public Team(Character initalChar) {

        Characters = new List<Character>();

        AddCharacter(initalChar);

        //Debug.Log(Characters[0].Obj.name + ": Joined the team!");

    }
    public void AddCharacter(Character ch) {

        if (!full)
        {

           Characters.Add(ch);

            if (!ch._base.isRightFacing)
            {

                ch.FlipSpriteOnX();

            }

            ch.JoinTeam(this);

        }

        if (Characters.Count >= 4) {

            full = true;
        
        }

    }
    public void RemoveCharacter(Character ch)
    {

        Debug.Log(ch._base.character_name + " died... Removing from Team \n");

        Characters.Remove(ch);

        if (Characters.Count == 0) {

            defeated = true;

        }

    }
    public void Attack(Team Enemy)
    {

        Debug.Log("Attacking Enemy\n");

        Characters[0].Attack(Enemy.Characters[0]);

    }
    public void SwapByCharacter(Character ch1, Character ch2)
    {

        int index1 = Characters.FindIndex(ch1.Equals);

        int index2 = Characters.FindIndex(ch2.Equals);

        SwapByIndex(index1, index2);

    }
    public void SwapByIndex(int index1, int index2)
    {
        Character temp;

        temp = Characters[index1];

        Characters[index1] = Characters[index2];

        Characters[index2] = temp;

    }
    public void SwapByOne()
    {
      Character temp;
      temp = Characters[0];
      Characters.RemoveAt(0);
      Characters.Add(temp);
    }
    public void Log() {

        Debug.Log("Team Data: \n");

        foreach (Character ch in Characters) {

            Debug.Log(ch._base.character_name + ": \n" +
                "HP: " + ch.currHP + "\n\n"

                );


        }
    }

}
public class CombatManager {

    Vector3 origin; // get center of combat screen

    Canvas canvas; //combat canvas

    GameController GC;

    EncounterManager EM;
    public GameObject backbutton;
    public GameObject fightbutton;
    public GameObject switchbutton1;
    public GameObject switchbutton2;
    public GameObject switchbutton3;
    public GameObject winpopup;
    public Team enemies;

    
    public CombatManager(GameController gc, GameObject combatCanvasObject) {

        GC = gc;

        EM = GC.gameObject.GetComponent<EncounterManager>();

        canvas = combatCanvasObject.GetComponent<Canvas>();

        origin = canvas.transform.position;

        backbutton = GameObject.Find("BACK");
        fightbutton = GameObject.Find("FIGHT");

        switchbutton1 = GameObject.Find("SWITCH1");
        switchbutton2 = GameObject.Find("SWITCH2");
        switchbutton3 = GameObject.Find("SWITCH3");

        backbutton.SetActive(false);

        winpopup = GameObject.Find("temp_win_popup");

        winpopup.SetActive(false);


    }
    public void activateSwitch(int partymembers){

      switchbutton1.SetActive(false);

      switchbutton2.SetActive(false);

      switchbutton3.SetActive(false);

      if (partymembers>1){
        switchbutton1.SetActive(true);
      }

      if (partymembers >2){
        switchbutton2.SetActive(true);
      }

      if (partymembers >3){
        switchbutton3.SetActive(true);
      }

    }
    public void startCombat(Team playerTeam, Team EnemyTeam)
    {

        // do not set the Gamestate from Overworld until all combat variables are initialized
        fightbutton.SetActive(true);
        activateSwitch(playerTeam.Characters.Count);
        enemies = EnemyTeam;

        GameController.switchCams();

        GameController.gameState = GameState.PLAYERMOVE;

        displayTeams(playerTeam, enemies);

    }
    public void EndCombat() {

        enemies = null;

        backbutton.SetActive(false);
        winpopup.SetActive(false);

        GameController.switchCams();

        EM.active.EndEncounter();

    }
    public void displayTeams(Team playerTeam, Team EnemyTeam) {

        //displays the sprites for each team on screen
        // Player team : 1st (-13.5, 11.5) 2nd (-14.5, 11.5) 3rd (-15.5, 11.5) 4th (16.5, 11.5)
        // Enemy team : 1st (-10.5, 11.5) 2nd (-9.5, 11.5) 3rd (-8.5, 11.5) 4th (-7.5, 11.5)

        float order = 0f;

        foreach (Character ch in playerTeam.Characters) {

            float width = ch.Obj.GetComponent<SpriteRenderer>().bounds.size.x;

            order += width;

            ch.Obj.transform.position = origin + new Vector3(-0.5f - order, 0.5f, 0f);

            ch.Show();
            ch.hpBar.GetComponent<LiveHPBar>().Set(ch);
            ch.ShowHpBar();
            
        }

        order = 0;

        foreach (Character ch in EnemyTeam.Characters)
        {

            float width = ch.Obj.GetComponent<SpriteRenderer>().size.x;

            order += width;

            ch.Obj.transform.position = origin + new Vector3(0.5f + width, 0.5f, 0f);

            ch.Show();
            ch.hpBar.GetComponent<LiveHPBar>().Set(ch);
            ch.ShowHpBar();

        }

    }
    public void Update(Team playerTeam) {

        displayTeams(playerTeam, enemies);

        activateSwitch(playerTeam.Characters.Count);

        if (enemies.defeated == true) {

            GC.playerObj.GetComponent<PlayerInfo>().addFF(enemies.reward);

            backbutton.SetActive(true);

            //Replacing with Notification
            //winpopup.SetActive(true);

            GC.FullNotification("Victory!", "You earned " + enemies.reward + " fish food!", null, 3);

            fightbutton.SetActive(false);



            //pressing backbutton now calls EndCombat function

            //EndCombat();

        }

        if (playerTeam.Characters[0] == null)
        {

            EndCombat();

        }
    }

}
