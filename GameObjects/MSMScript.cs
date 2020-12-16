using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MSMScript : MonoBehaviour
{
    //Objects for enemies
    public GameObject coin; //coin dropped
    public GameObject purpleCoin;//coin from laser Enemy
    public GameObject chest; //chest dropped
    public EnemyAIScript Enemy; //enemy prefab
    public EnemyAIScript LaserEnemy; //enemy prefab
    public GameObject HVT; //high value target
    //public int numEnemies; //number of enemies in map
    //public int numLaserEnemies; //number of laser enemies in map
    protected int MAX_ENEMIES = 250;
    protected int MAX_HVT = 5;
    protected int MAX_LASER_ENEMIES = 10;
    public PatrolPointsScript patrolScript; //points the enemies will walk to/spawn

    //timer variables 
    protected float startTime;
    protected float timePassed;
    protected float timeRemaining;
    private bool gameRunning = true;

    //guards us from double dipping when adding points
    protected float incrementGuardP1;
    protected float incrementGuardP2; 

    //Event Vars
    const float TIME_BTWN_EVENTS = 60; //how long between each event?
    protected float nextEvent = 30; //the first event starts at 30 sec (will increment after by 1 min)
    protected int chosenEvent; //our random variable that will pick our next event 
    protected bool eventWarning = true; //do we need to warn players?
    protected float timeInGame = 180;

    //Crate Object
    public GameObject crate;

    //Spawnpoint locations for players
    public Transform p1SpawnPoint;
    public Transform p2SpawnPoint;
    public Transform p1SpawnPointEntrance;
    public Transform p2SpawnPointEntrance;
    public Vector2 eventSpawnPoint; //where will the crate and HVTs spawn?

    //Scoring variables
    protected int p1Score = 0;
    protected int p2Score = 0;
    //score constants (points given for each event in game)
    public int CRATE_SCORE = 15, HVT_SCORE = 15, PLAYER_SCORE = 5, DEATH_SCORE = -10, LASER_ENEMY_SCORE = 5, ENEMY_SCORE = 1;

    //UI variables
    public Text p1ScoreText;
    public Text p2ScoreText;
    public Text timerText;
    public Text queueText;
    public Text WinnerText;
    public Text BigTimer;
    public GameObject GameOver;


    //ObjectPooling
    public ObjectPool easyEnemyAIPool;
    public ObjectPool LaserEnemyAIPool;
    public ObjectPool HVTPool;
    public ObjectPool coinPool;
    public ObjectPool purpleCoinPool;

    //AudioFiles
    public AudioSource audio; //our game audiosource
    public AudioClip playerDeath;
    public AudioClip gruntDeath;
    public AudioClip hvtDeath;
    public AudioClip point;
    public AudioClip gameover;

    //Singleton
    private static MSMScript _instance = null;

    public static MSMScript Instance
    {
        get
        {
            return _instance;
        }
    }

    //instance handling
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;

        }
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        //ObjectPooling
        easyEnemyAIPool = new ObjectPool(Enemy.gameObject, false, MAX_ENEMIES);
        LaserEnemyAIPool = new ObjectPool(LaserEnemy.gameObject, false, MAX_LASER_ENEMIES);
        HVTPool = new ObjectPool(HVT, false, MAX_HVT);
        coinPool = new ObjectPool(coin, true, 50);
        purpleCoinPool = new ObjectPool(purpleCoin, true, 10);

        Time.timeScale = 1;
        startTime = Time.time;
        timePassed = 0;
        incrementGuardP1 = Time.time;
        incrementGuardP2 = Time.time;

        //start spawning enemies every 1.5 seconds
        InvokeRepeating("enemySpawn", 0f, 1.5f);
        //numEnemies = 0;

        queueText.text = "FIGHT!"; //set our queue text to an initial val
    }

    // Update is called once per frame
    void Update()
    {
        timePassed = Time.time - startTime;
        timeRemaining = timeInGame - timePassed; //3 minutes minus time passed

        HandleUI();
        HandleEvents();

        //Is game over
        if ((int)timeRemaining <= 0 && gameRunning)
        {
            gameOver();
            gameRunning = false;
        }


        //if you press escape, the game will quit
        if (Input.GetButtonDown("Quit"))
            Application.Quit();
    }

    //handles all of our UI functionality in one method
    private void HandleUI()
    {
        //display score
        p1ScoreText.text = "Score: " + p1Score;
        p2ScoreText.text = "Score: " + p2Score;
        timerText.text = FormatTime(timeRemaining);

        if (timeRemaining >= 6)
        {
            BigTimer.text = "";
        }
        else
        {
            BigTimer.text = Mathf.Floor(timeRemaining).ToString();
        }
    }

    //Handles when our HVT and Crate events happen based on timePassed
    public void HandleEvents()
    {

        if (timePassed >= nextEvent - 5 && eventWarning) //5 seconds prior to an event, give a heads up 
        {
            chosenEvent = Random.Range(1, 3); //random 1 - 2

            if (chosenEvent == 1)
                UpdateQueue("CRATE INCOMING");
            else
                UpdateQueue("HVT INCOMING");

            eventWarning = false;
        }
        if(timePassed > nextEvent)
        {
            if(chosenEvent == 1)//spawn Crate
            {
                SpawnCrate(eventSpawnPoint);
                UpdateQueue("CRATE DROPPED");
            }
            else//spawn HVT
            {
                GameObject HVT = HVTPool.GetObject();
                if (!HVT)
                    return;
                HVT.transform.position = eventSpawnPoint;
                HVT.GetComponent<monsterScript>().isDead = false;
                HVT.GetComponent<monsterScript>().currentHealth = HVT.GetComponent<monsterScript>().maxHealth;
                HVT.GetComponent<monsterScript>().HealthBar.SetMaxHealth(HVT.GetComponent<monsterScript>().maxHealth);
                UpdateQueue("HVT SPAWNED");
            }

            nextEvent += TIME_BTWN_EVENTS;
            eventWarning = true;
        }
    }

    //Spawn Enemy
    public void enemySpawn() {

        int patrolIndex = Random.Range(0, patrolScript.numPatrolPoints);

        GameObject enemy = easyEnemyAIPool.GetObject();
        if (!enemy)
            return;
        enemy.transform.position = patrolScript.patrols[patrolIndex];
        enemy.GetComponent<EnemyAIScript>().isDead = false;
        enemy.GetComponent<EnemyAIScript>().currentHealth = enemy.GetComponent<EnemyAIScript>().maxHealth;
        enemy.GetComponent<EnemyAIScript>().HealthBar.SetMaxHealth(enemy.GetComponent<EnemyAIScript>().maxHealth);

        //EnemyAIScript p = Instantiate(Enemy, patrolScript.patrols[patrolIndex], Quaternion.identity);

    }

    //kills specified enemy when called
    public void KillEnemy(GameObject enemy) {
        //create coin
        GameObject coin = coinPool.GetObject();
        if (!coin)
            return;
        coin.transform.position = enemy.transform.position;
        //kill enemy
        enemy.SetActive(false);
        audio.PlayOneShot(gruntDeath);
    }

    //kills HVT when called
    public void KillMonster(GameObject monster)
    {
        audio.PlayOneShot(hvtDeath);
        Instantiate(chest, monster.transform.position, Quaternion.identity);
        monster.SetActive(false);
    }


    //kills and respawns specified player when called
    public void KillPlayer(GameObject player, bool wasBullet)
    {
        if(player.tag == "Player1")
        {
            
            player.transform.position = p1SpawnPoint.position; //back to spawn
            player.GetComponent<Player1Script>().playerStats.resetPlayerHealth(); //reset health
            Invoke("player1DeathScorePopUp", 0.8f);
            AddOrSubPoints(1, "Death"); //subtract points from p1
            if (wasBullet) {
                AddOrSubPoints(2, "Player"); //award player points 
                PopUpScript.Create(GameObject.Find("Player2").transform.position, PLAYER_SCORE, "score");//popUp for awarded Points
            }
                
        }
        else if(player.tag == "Player2")
        {
            player.transform.position = p2SpawnPoint.position; //back to spawn
            player.GetComponent<Player2Script>().playerStats.resetPlayerHealth(); //reset health
            Invoke("player2DeathScorePopUp",0.8f);
            AddOrSubPoints(2, "Death"); //subtract points from p2
            if (wasBullet)
            {
                AddOrSubPoints(2, "Player2"); //award player points 
                PopUpScript.Create(GameObject.Find("Player").transform.position, PLAYER_SCORE, "score");//popUp for awarded Points
            }
        }

        audio.PlayOneShot(playerDeath);
    }

    //player Pop up on offset time
    public void player1DeathScorePopUp() {
        PopUpScript.Create(GameObject.Find("Player").transform.position, -DEATH_SCORE, "negScore");//popUp for lost Points
    }

    public void player2DeathScorePopUp()
    {
        PopUpScript.Create(GameObject.Find("Player2").transform.position, -DEATH_SCORE, "negScore");//popUp for lost Points
    }


    //add points to a player
    //can input a pos or neg value 
    public void AddOrSubPoints(int player, string pointType)
    {
        if(player == 1)
        {
            if(Time.time - incrementGuardP1 > 0.005)
            {
                switch (pointType)
                {
                    case "Death":
                        p1Score += DEATH_SCORE;
                        if (p1Score < 0)//does not allow negative score
                            p1Score = 0;
                        break;
                    case "Crate":
                        p1Score += CRATE_SCORE;
                        break;
                    case "HVT":
                        p1Score += HVT_SCORE;
                        break;
                    case "Player":
                        p1Score += PLAYER_SCORE;
                        break;
                    case "Enemy":
                        p1Score += ENEMY_SCORE;
                        break;
                    case "LaserEnemy":
                        p1Score += LASER_ENEMY_SCORE;
                        break;
                }

                incrementGuardP1 = Time.time;
            }
        }
        else if(player == 2)
        {
            if (Time.time - incrementGuardP2 > 0.005)
            {
                switch (pointType)
                {
                    case "Death":
                        p2Score += DEATH_SCORE;
                        if (p2Score < 0)//does not allow negative score
                            p2Score = 0;
                        break;
                    case "Crate":
                        p2Score += CRATE_SCORE;
                        break;
                    case "HVT":
                        p2Score += HVT_SCORE;
                        break;
                    case "Player":
                        p2Score += PLAYER_SCORE;
                        break;
                    case "Enemy":
                        p2Score += ENEMY_SCORE;
                        break;
                    case "LaserEnemy":
                        p2Score += LASER_ENEMY_SCORE;
                        break;
                }

                incrementGuardP2 = Time.time;
            }
        }

        if (pointType != "Death" && pointType != "Player")
            audio.PlayOneShot(point);
    }

    //CRATE SPAWNING
    //takes in the position to spawn at 
    public void SpawnCrate(Vector2 pos)
    {
        Instantiate(crate, pos, Quaternion.identity);
    }


    //Our queue tells the players what event is going on in the map 
    //this method updates the queue text to whatever message is passed in
    public void UpdateQueue(string msg)
    {
        queueText.text = msg;
    }

    //formats our timer so it displays in minutes and seconds
    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    //Game Over Screen
    public void gameOver() {
        Time.timeScale = 0;
        GameOver.SetActive(true);
        audio.PlayOneShot(gameover);
        if (p1Score > p2Score)
        {
            GameOver.GetComponent<Image>().color = new Color(22 / 255f, 18 / 255f, 158 / 255f);
            WinnerText.text = "Blue Player Wins with " + p1Score + " points.";
        }
        else if (p2Score > p1Score)
        {
            GameOver.GetComponent<Image>().color = new Color(210 / 255f, 41 / 255f, 41 / 255f); 
            WinnerText.text = "Red Player Wins with " + p2Score + " points.";
        }
        else {
            WinnerText.text = "Tie Game with " + p2Score + " points each.";
        }
    }

}
