using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoOpMSMScript : MonoBehaviour
{
    //Objects for enemies
    public GameObject coin; //coin dropped
    public GameObject purpleCoin;//coin from laser Enemy
    public GameObject chest; //chest dropped
    public GameObject healthOrange; //orangey drops this, it gives health
    public EnemyAIScript Enemy; //enemy prefab
    public EnemyAIScript LaserEnemy; //enemy prefab
    public GameObject HVT; //high value target
    public GameObject Skeleton; //skelly enemy
    public GameObject Exploder; //exploder enemy
    public GameObject AOE; //aoe enemy
    public GameObject orangey; //orangey gives lives
    //200 possible enemies in game total 
    protected const int MAX_ENEMIES = 100;
    protected const int MAX_LASER_ENEMIES = 10;
    protected const int MAX_HVT = 5;
    protected const int MAX_SKELETONS = 50;
    protected const int MAX_EXPLODERS = 15;
    protected const int MAX_AOE_ENEMIES = 20;
    public PatrolPointsScript patrolScript; //points the enemies will walk to/spawn

    //Our Round Spawner - 2D Array
    //keeps track of what enemies will spawn in what round 
    // 50 rounds
    //6 different enemies 
    private RoundSpawner roundSpawner;
    public int currentRound = 0;
    private int numMonsters;
    private int[] maxSpawns = { 100, 10, 5, 30, 15, 20 }; //if a player gets beyond level 50, this is what will spawn

    //timer variables 
    protected float startTime;
    protected float timePassed;
    private bool gameRunning = true;

    //guards us from double dipping when adding points
    protected float incrementGuardP1;
    protected float incrementGuardP2;
    protected float eventIncrementGuard;

    //Event Vars
    const float TIME_BTWN_EVENTS = 60; //how long between each event?
    protected float nextEvent = 60; //the first event starts at 1 min (will increment after by 1 min)
    protected string chosenEvent; //2 event options - switch off (Quarantine and Health) 
    protected bool eventWarning = true; //do we need to warn players?
    protected float QuarantineTimer = 0; //15 seconds to get to the safe zone and 30 seconds in quarantine - set to 0 for now because the event is not going on
    private bool surviveQueue = false;

    //boolean values telling us which zones are safe to be in for quarantine
    protected bool forestSafe = true;
    protected bool hilltopsSafe = true;
    protected bool ruinsSafe = true;
    protected bool pondsSafe = true;
    protected bool riversSafe = true;

    //Our Quarantine Zones
    public GameObject hillZone;
    public GameObject pondZone;
    public GameObject ruinZone;
    public GameObject forestZone;
    public GameObject riverZone;

    //Spawnpoint locations for players
    public Transform p1SpawnPoint;
    public Transform p2SpawnPoint;


    //Scoring variables
    protected int p1Score = 0;
    protected int p2Score = 0;
    protected int finalScore;
    //score constants (points given for each event in game)
    public int HVT_SCORE = 15, DEATH_SCORE = -5, LASER_ENEMY_SCORE = 5, ENEMY_SCORE = 1;

    //base life for each player 
    protected int player1Lives = 10;
    protected int player2Lives = 10;

    
    //UI variables
    public Text p1ScoreText;
    public Text p2ScoreText;
    public Text timerText;
    public Text queueText; //what words will queue the player in on events
    public Text BigTimer; //will be our round counter in this case 
    public GameObject GameOver; //game over panel
    public Text finalP1ScoreText; //final score of p1
    public Text finalP2ScoreText; //final score of p2
    public Text finalScoreText; //final overall score
    public Text highScoreText; //best score
    public Text finalTimeText; //final time
    public Text bestTimeText; //best time
    public GameObject p1Defeated; //defeated p1 screen
    public GameObject p2Defeated; //defeated p2 screen
    public GameObject quarantineCounter; //countdown for quarantine time
    public Text qCountText;
    public TextMeshProUGUI p1LivesText;
    public TextMeshProUGUI p2LivesText;

    //ObjectPooling
    public ObjectPool easyEnemyAIPool;
    public ObjectPool LaserEnemyAIPool;
    public ObjectPool HVTPool;
    public ObjectPool SkeletonPool;
    public ObjectPool ExploderPool;
    public ObjectPool AOEPool;
    public ObjectPool coinPool;
    public ObjectPool purpleCoinPool;
    public ObjectPool orangeyPool;

    //AudioFiles
    public AudioSource audio; //our game audiosource
    public AudioClip playerDeath;
    public AudioClip gruntDeath;
    public AudioClip hvtDeath;
    public AudioClip orangeyHurt;
    public AudioClip point;
    public AudioClip quarantine;
    public AudioClip forest;
    public AudioClip ponds;
    public AudioClip rivers;
    public AudioClip hilltops;
    public AudioClip ruins;
    public AudioClip survive;
    public AudioClip gameOver;
    public AudioClip health;
    public AudioClip safeQ;


    //Singleton
    private static CoOpMSMScript _instance = null;

    public static CoOpMSMScript Instance
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
        SkeletonPool = new ObjectPool(Skeleton, false, MAX_SKELETONS);
        ExploderPool = new ObjectPool(Exploder, false, MAX_EXPLODERS);
        AOEPool = new ObjectPool(AOE, false, MAX_AOE_ENEMIES);
        orangeyPool = new ObjectPool(orangey, false, 5);
        coinPool = new ObjectPool(coin, true, 50);
        purpleCoinPool = new ObjectPool(purpleCoin, true, 10);

        //initialize our rounds (start and spawn our first round)
        roundSpawner = new RoundSpawner();
        SpawnWave(roundSpawner.getNextRound(currentRound));
        numMonsters = roundSpawner.getRoundNumMonsters(currentRound);

        Time.timeScale = 1;
        startTime = Time.time;
        timePassed = 0;
        incrementGuardP1 = Time.time;
        incrementGuardP2 = Time.time;
        eventIncrementGuard = Time.time;

        //setlives
        p1LivesText.text = "" + player1Lives;
        p2LivesText.text = " " + player2Lives;

        queueText.text = "SURVIVE!"; //set our queue text to an initial val
        audio.PlayOneShot(survive);
    }



    // Update is called once per frame
    void Update()
    {
        timePassed = Time.time - startTime;

        HandleRounds();
        HandleEvents();
        HandleLives();
        HandleUI();



        //if you press escape, the game will quit
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }


    //Handles how our Rounds change and when to spawn a new wave
    private void HandleRounds()
    {
        if(numMonsters <= 0)
        {
            currentRound += 1;
            if(currentRound < 50)
            {
                numMonsters = roundSpawner.getRoundNumMonsters(currentRound);
                SpawnWave(roundSpawner.getNextRound(currentRound));
            }
            else
            {
                numMonsters = 180;
                SpawnWave(maxSpawns);
            }

            print("Current round: " + currentRound);
            print("Monsters incoming : " + numMonsters);
        }
    }

    //Spawns a new wave dependent on the array passed in
    private void SpawnWave(int[] monsters)
    {
        //spawn regular enemies
        for (int i = 0; i < monsters[0]; i++)
            enemySpawn();
        //spawn lasers
        for (int i = 0; i < monsters[1]; i++)
            enemyLaserSpawn();
        //spawn HVT
        for (int i = 0; i < monsters[2]; i++)
            hvtSpawn();
        //spawn skeletons
        for (int i = 0; i < monsters[3]; i++)
            skeletonSpawn();
        //spawn exploders
        for (int i = 0; i < monsters[4]; i++)
            exploderSpawn();
        //spawn AOEs
        for (int i = 0; i < monsters[5]; i++)
            aoeSpawn();
    }


    //Handles the two Co op Events: Quarantine Zone and Health Orangies
    public void HandleEvents()
    {
        if(timePassed >= nextEvent - 10 && eventWarning) //if we are less than 10 seconds till the event and we need to warn the player
        {
            if (chosenEvent == "Quarantine") //switch to health
            {
                chosenEvent = "Health";
                UpdateQueue("Health Incoming!");
            }
            else
            {
                chosenEvent = "Quarantine";
                UpdateQueue("Quarantine Incoming!");
                audio.PlayOneShot(quarantine);
            }

            eventWarning = false;
        }
        if(timePassed > nextEvent) //it is time for the next event
        {
            if (chosenEvent == "Quarantine")
            {
                int location = Random.Range(1, 6); //random 1 - 5

                if(location == 1) //go to forest
                {
                    UpdateQueue("Get To Forest!");
                    audio.PlayOneShot(forest);
                    forestSafe = true;

                    //make all other zones unsafe
                    hilltopsSafe = false;
                    ruinsSafe = false;
                    pondsSafe = false;
                    riversSafe = false;
                }
                else if(location == 2) //go to hilltops
                {
                    UpdateQueue("Get To Hilltops!");
                    audio.PlayOneShot(hilltops);
                    hilltopsSafe = true;

                    //make all other zones unsafe
                    ruinsSafe = false;
                    pondsSafe = false;
                    riversSafe = false;
                    forestSafe = false;
                }
                else if (location == 3) //go to rivers
                {
                    UpdateQueue("Get To Rivers!");
                    audio.PlayOneShot(rivers);
                    riversSafe = true;

                    //make all other zones unsafe
                    hilltopsSafe = false;
                    ruinsSafe = false;
                    pondsSafe = false;
                    forestSafe = false;
                }
                else if (location == 4) //go to ponds
                {
                    UpdateQueue("Get To Ponds!");
                    audio.PlayOneShot(ponds);
                    pondsSafe = true;

                    //make all other zones unsafe
                    hilltopsSafe = false;
                    ruinsSafe = false;
                    riversSafe = false;
                    forestSafe = false;
                }
                else //go to ruins
                {
                    UpdateQueue("Get To Ruins!");
                    audio.PlayOneShot(ruins);
                    ruinsSafe = true;

                    //make all other zones unsafe
                    hilltopsSafe = false;
                    pondsSafe = false;
                    riversSafe = false;
                    forestSafe = false;
                }

                QuarantineTimer = 45;
                nextEvent = timePassed + TIME_BTWN_EVENTS + 45; //1 minute for the break, 15 seconds for the time it takes to get to the quarantine, and 30 seconds in quarantine
            }
            else //spawn 5 health buddies 
            {
                UpdateQueue("Catch Your Health!");
                audio.PlayOneShot(health);
                for (int i = 0; i < 5; i++)
                    HealthOrangeSpawn();

                nextEvent = timePassed + TIME_BTWN_EVENTS;
            }

            eventWarning = true;
        }

        if(QuarantineTimer > 0)
        {
            QuarantineTimer -= Time.deltaTime;
            quarantineCounter.SetActive(true);

            if (QuarantineTimer < 30) //once our time to get to the safe zone is up, enable the zones that are not safe
            {
                qCountText.text = (int)QuarantineTimer + ""; //display the 30 second countdown
                if (!hilltopsSafe)
                    hillZone.SetActive(true);
                if (!riversSafe)
                    riverZone.SetActive(true);
                if (!pondsSafe)
                    pondZone.SetActive(true);
                if (!ruinsSafe)
                    ruinZone.SetActive(true);
                if (!forestSafe)
                    forestZone.SetActive(true);
            }
            else
                qCountText.text = (int)QuarantineTimer - 30 + ""; //display the 15 second countdown

            surviveQueue = true;

        }
        else
        {
            if(surviveQueue)
            {
                audio.PlayOneShot(safeQ);
                UpdateQueue("SURVIVE!");
                surviveQueue = false;
            }

            //make all zones safe
            hilltopsSafe = true;
            ruinsSafe = true;
            pondsSafe = true;
            riversSafe = true;
            forestSafe = true;
            forestZone.SetActive(false);
            ruinZone.SetActive(false);
            pondZone.SetActive(false);
            riverZone.SetActive(false);
            hillZone.SetActive(false);
            quarantineCounter.SetActive(false);
        }
    }

    //handles players lives and game over when both are 0
    private void HandleLives()
    {
        if (player1Lives <= 0 && gameRunning)
        {
            GameObject p1 = GameObject.Find("Player");
            p1.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            p1.transform.position = new Vector2(-110, 65);
            p1Defeated.SetActive(true);
        }

        if (player2Lives <= 0 && gameRunning)
        {
            GameObject p2 = GameObject.Find("Player2");
            p2.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            p2.transform.position = new Vector2(-110, -65);
            p2Defeated.SetActive(true);
        }

        if (player1Lives <= 0 && player2Lives <= 0 && gameRunning)
        {
            gameRunning = false;
            EndGame();
        }
    }

    //handles all of our UI functionality in one method
    private void HandleUI()
    {
        //display score
        p1ScoreText.text = "Score: " + p1Score;
        p2ScoreText.text = "Score: " + p2Score;
        timerText.text = FormatTime(timePassed);
        BigTimer.text = currentRound + 1 + "";
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    ////ENEMY SPAWN METHODS 
    ///////////////////////////////////////////////////////////////////////////////////////

    //Spawn Enemy
    public void enemySpawn()
    {
        int patrolIndex = Random.Range(0, patrolScript.numPatrolPoints);

        GameObject enemy = easyEnemyAIPool.GetObject();
        if (!enemy)
            return;
        enemy.transform.position = patrolScript.patrols[patrolIndex];
        enemy.GetComponent<EnemyAIScript>().isDead = false;
        enemy.GetComponent<EnemyAIScript>().currentHealth = enemy.GetComponent<EnemyAIScript>().maxHealth;
        enemy.GetComponent<EnemyAIScript>().HealthBar.SetMaxHealth(enemy.GetComponent<EnemyAIScript>().maxHealth);
    }

    //Spawn Laser Enemy
    public void enemyLaserSpawn()
    {
        int patrolIndex = Random.Range(0, patrolScript.numPatrolPoints);

        GameObject laserEnemy = LaserEnemyAIPool.GetObject();
        if (!laserEnemy)
            return;
        laserEnemy.transform.position = patrolScript.patrols[patrolIndex];
        laserEnemy.GetComponent<EnemyLaserAIScript>().isDead = false;
        laserEnemy.GetComponent<EnemyLaserAIScript>().currentHealth = laserEnemy.GetComponent<EnemyLaserAIScript>().maxHealth;
        laserEnemy.GetComponent<EnemyLaserAIScript>().HealthBar.SetMaxHealth(laserEnemy.GetComponent<EnemyLaserAIScript>().maxHealth);
    }

    //Spawn HVT
    public void hvtSpawn()
    {
        int patrolIndex = Random.Range(0, patrolScript.numPatrolPoints);

        GameObject HVT = HVTPool.GetObject();
        if (!HVT)
            return;
        HVT.transform.position = patrolScript.patrols[patrolIndex];
        HVT.GetComponent<monsterScript>().isDead = false;
        HVT.GetComponent<monsterScript>().currentHealth = HVT.GetComponent<monsterScript>().maxHealth;
        HVT.GetComponent<monsterScript>().HealthBar.SetMaxHealth(HVT.GetComponent<monsterScript>().maxHealth);
    }

    //Spawn Skeleton
    public void skeletonSpawn()
    {
        int patrolIndex = Random.Range(0, patrolScript.numPatrolPoints);

        GameObject skel = SkeletonPool.GetObject();
        if (!skel)
            return;
        skel.transform.position = patrolScript.patrols[patrolIndex];
        skel.GetComponent<SkeletonScript>().isDead = false;
        skel.GetComponent<SkeletonScript>().currentHealth = skel.GetComponent<SkeletonScript>().maxHealth;
        skel.GetComponent<SkeletonScript>().HealthBar.SetMaxHealth(skel.GetComponent<SkeletonScript>().maxHealth);
    }

    public void exploderSpawn()
    {
        int patrolIndex = Random.Range(0, patrolScript.numPatrolPoints);

        GameObject exploder = ExploderPool.GetObject();
        if (!exploder)
            return;
        exploder.transform.position = patrolScript.patrols[patrolIndex];
        exploder.GetComponent<ExploderScript>().isDead = false;
        exploder.GetComponent<ExploderScript>().hasntExploded = false;
        exploder.GetComponent<ExploderScript>().currentHealth = exploder.GetComponent<ExploderScript>().maxHealth;
        exploder.GetComponent<ExploderScript>().HealthBar.SetMaxHealth(exploder.GetComponent<ExploderScript>().maxHealth);
    }

    public void aoeSpawn()
    {
        int patrolIndex = Random.Range(0, patrolScript.numPatrolPoints);

        GameObject aoe = AOEPool.GetObject();
        if (!aoe)
            return;
        aoe.transform.position = patrolScript.patrols[patrolIndex];
        aoe.GetComponent<AOEScript>().isDead = false;
        aoe.GetComponent<AOEScript>().currentHealth = aoe.GetComponent<AOEScript>().maxHealth;
        aoe.GetComponent<AOEScript>().HealthBar.SetMaxHealth(aoe.GetComponent<AOEScript>().maxHealth);
    }

    //spawns an orangey that drops a life 
    public void HealthOrangeSpawn()
    {
        int patrolIndex = Random.Range(0, patrolScript.numPatrolPoints);

        GameObject orangeGuy = orangeyPool.GetObject();
        if (!orangeGuy)
            return;

        orangeGuy.transform.position = patrolScript.patrols[patrolIndex];
        orangeGuy.GetComponent<OrangeyScript>().isDead = false;
        orangeGuy.GetComponent<OrangeyScript>().currentHealth = orangeGuy.GetComponent<OrangeyScript>().maxHealth;
        orangeGuy.GetComponent<OrangeyScript>().HealthBar.SetMaxHealth(orangeGuy.GetComponent<OrangeyScript>().maxHealth);
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    ////End Spawn Methods
    /////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////
    //Kill Methods - oof kind of sounds inhumane doesnt it lol
    ////////////////////////////////////////////////////////////////////////////////////////


    //kills specified enemy when called
    public void KillEnemy(GameObject enemy)
    {
        //create coin
        GameObject coin = coinPool.GetObject();
        if (!coin)
            return;
        coin.GetComponent<CoinScript>().startTime = Time.time;
        coin.transform.position = enemy.transform.position;
        //kill enemy
        enemy.SetActive(false);
        audio.PlayOneShot(gruntDeath);
        numMonsters--;
    }


    //kills laser enemy when called
    public void KillLaserEnemy(GameObject enemy)
    {
        //create coin
        GameObject purpleCoin = purpleCoinPool.GetObject();
        if (!purpleCoin)
            return;
        purpleCoin.GetComponent<CoinScript>().startTime = Time.time;
        purpleCoin.transform.position = enemy.transform.position;
        //kill enemy
        enemy.SetActive(false);
        audio.PlayOneShot(gruntDeath);
        numMonsters--;
    }

    //kills HVT when called
    public void KillMonster(GameObject monster)
    {
        audio.PlayOneShot(hvtDeath);
        Instantiate(chest, monster.transform.position, Quaternion.identity);
        monster.SetActive(false);
        numMonsters--;
    }

    //kill an orangey character when called
    public void KillOrangey(GameObject orangey)
    {
        audio.PlayOneShot(orangeyHurt);
        Instantiate(healthOrange, orangey.transform.position, Quaternion.identity);
        orangey.SetActive(false);
    }


    //kills and respawns specified player when called
    public void KillPlayer(GameObject player, bool wasBullet)
    {
        if (player.tag == "Player1")
        {
            player1Lives -= 1;
            p1LivesText.text = "" + player1Lives;

            //ReSpawn player
            if (ruinsSafe==true)
                 player.transform.position = p1SpawnPoint.position; //back to spawn
            else if (forestSafe == true)
                player.transform.position = new Vector2(-33, -31);
            else if (hilltopsSafe == true)
                player.transform.position = new Vector2(-37,28);
            else if (riversSafe == true)
                player.transform.position = new Vector2(35,28);
            else if (pondsSafe == true)
                player.transform.position = new Vector2(30, -30);



            player.GetComponent<Player1Script>().playerStats.resetPlayerHealth(); //reset health
            Invoke("player1DeathScorePopUp", 0.8f);
            AddOrSubPoints(1, "Death"); //subtract points from p1
        }
        else if (player.tag == "Player2")
        {
            player2Lives -= 1;

            if(player2Lives>9)
                p2LivesText.text = " "+player2Lives;
            else
                p2LivesText.text = "  " + player2Lives;

            //ReSpawn player
            if (ruinsSafe == true)
                player.transform.position = p2SpawnPoint.position; //back to spawn
            else if (forestSafe == true)
                player.transform.position = new Vector2(-33, -31);
            else if (hilltopsSafe == true)
                player.transform.position = new Vector2(-37, 28);
            else if (riversSafe == true)
                player.transform.position = new Vector2(35, 28);
            else if (pondsSafe == true)
                player.transform.position = new Vector2(30, -30);


            player.GetComponent<Player2Script>().playerStats.resetPlayerHealth(); //reset health
            Invoke("player2DeathScorePopUp", 0.8f);
            AddOrSubPoints(2, "Death"); //subtract points from p2
        }

        audio.PlayOneShot(playerDeath);
    }

    //player Pop up on offset time
    public void player1DeathScorePopUp()
    {
        PopUpScript.Create(GameObject.Find("Player").transform.position, -DEATH_SCORE, "negScore");//popUp for lost Points
    }

    public void player2DeathScorePopUp()
    {
        PopUpScript.Create(GameObject.Find("Player2").transform.position, -DEATH_SCORE, "negScore");//popUp for lost Points
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    //End of Kill Methods - yeah this doesnt sound great haha
    ////////////////////////////////////////////////////////////////////////////////////////
    

    //add points to a player
    //can input a pos or neg value 
    public void AddOrSubPoints(int player, string pointType)
    {
        if (player == 1)
        {
            if (Time.time - incrementGuardP1 > 0.005)
            {
                switch (pointType)
                {
                    case "Death":
                        p1Score += DEATH_SCORE;
                        if (p1Score < 0)//does not allow negative score
                            p1Score = 0;
                        break;
                    case "HVT":
                        p1Score += HVT_SCORE;
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
        else if (player == 2)
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
                    case "HVT":
                        p2Score += HVT_SCORE;
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

    //adds 1 life to the specified player
    public void AddLife(string player)
    {
        if (player == "p1" && Time.time - incrementGuardP1 > 0.005)
        {
            player1Lives++;
            p1LivesText.text = "" + player1Lives;

            incrementGuardP1 = Time.time;
            audio.PlayOneShot(point);
        }

        else if(player == "p2" && Time.time - incrementGuardP2 > 0.005)
        {
            player2Lives++;

            if (player2Lives > 9)
                p2LivesText.text = " " + player2Lives;
            else
                p2LivesText.text = "  " + player2Lives;

            incrementGuardP2 = Time.time;
            audio.PlayOneShot(point);
        }

        print(player1Lives);
        print(player2Lives);
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


    //will display each player score, time survived, and overall score 
    //menu buttons for restart and main menu 
    public void EndGame()
    {
        Time.timeScale = 0;
        
        p1Defeated.SetActive(false);
        p2Defeated.SetActive(false);
        audio.PlayOneShot(gameOver);

        finalScore = p1Score + p2Score;
        finalP1ScoreText.text = "P1 Score: " + p1Score;
        finalP2ScoreText.text = "P2 Score: " + p2Score;
        finalScoreText.text = "Overall Score: " + finalScore;
        finalTimeText.text = "Time Survived: " + FormatTime(timePassed);

        //find our high score 
        if((PlayerPrefs.HasKey("highScore")))
        {
            if(PlayerPrefs.GetInt("highScore") < finalScore)
            {
                PlayerPrefs.SetInt("highScore", finalScore);
                highScoreText.text = "NEW High Score: " + PlayerPrefs.GetInt("highScore");
            }
            else
            {
                highScoreText.text = "High Score: " + PlayerPrefs.GetInt("highScore");
            }
        }
        else
        {
            PlayerPrefs.SetInt("highScore", finalScore);
            highScoreText.text = "High Score: " + PlayerPrefs.GetInt("highScore");
        }

        //find our best time 
        if ((PlayerPrefs.HasKey("bestTime")))
        {
            if (PlayerPrefs.GetFloat("bestTime") < timePassed)
                PlayerPrefs.SetFloat("bestTime", timePassed);
        }
        else
        {
            PlayerPrefs.SetFloat("bestTime", finalScore);
        }

        bestTimeText.text = "Longest Time Survived: " + FormatTime(timePassed);


        GameOver.SetActive(true);
    }
}
