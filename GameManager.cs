using System; 
using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    AssetPalette assetPalette;
    int enemyPrefabCount;

    bool showMessage;
    bool firstMessage;
    int messageIndex;
    string messageText;
    float messageTimer;
    float messageDelay = 2.5f;

    bool isGameOver;
    bool playerReady;
    bool initReadyScreen;

    int playerScore;

    float gameRestartTime;
    float gamePlayerReadyTime;

    public float gameRestartDelay = 5f;
    public float gamePlayerReadyDelay = 3f;

    public struct WorldViewCoordinates
    {
        public float Top;
        public float Right;
        public float Left;
        public float Bottom;
    }
    public WorldViewCoordinates worldViewCoords;

    TextMeshProUGUI playerScoreText;
    TextMeshProUGUI screenMessageText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerReady)
        {
            if (initReadyScreen)
            {
                FreezePlayer(true);
                FreezeEnemies(true);
                screenMessageText.alignment = TextAlignmentOptions.Center;
                screenMessageText.alignment = TextAlignmentOptions.Top;
                screenMessageText.fontStyle = FontStyles.UpperCase;
                screenMessageText.fontSize = 24;
                screenMessageText.text = "\n\n\n\nREADY";
                initReadyScreen = false;
            }

            gamePlayerReadyTime -= Time.deltaTime;
            if (gamePlayerReadyTime < 0)
            {
                FreezePlayer(false);
                FreezeEnemies(false);
                screenMessageText.text = "";
                playerReady = false;
            }
            return;
        }

        if (playerScoreText != null)
        {
            playerScoreText.text = String.Format("<mspace=\"{0}\">{1:0000000}</mspace>", playerScoreText.fontSize, playerScore);
        }

        if (!isGameOver)
        {
            // do stuff and things during the course of the game  
            GetWorldViewCoordinates();
            RepositionEnemies();
            ShowMessage();
            SpawnEnemies();
        }
        else
        {
            gameRestartTime -= Time.deltaTime;
            if (gameRestartTime < 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartGame();
    }

    private void StartGame()
    {
        isGameOver = false;
        playerReady = true;
        initReadyScreen = true;
        firstMessage = true;
        gamePlayerReadyTime = gamePlayerReadyDelay;
        playerScoreText = GameObject.Find("PlayerScore").GetComponent<TextMeshProUGUI>();
        screenMessageText = GameObject.Find("ScreenMessage").GetComponent<TextMeshProUGUI>();

    }

    public void AddScorePoints(int points)
    {
        playerScore += points;
    }

    private void FreezePlayer(bool freeze)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<PlayerController>().FreezeInput(freeze);
            player.GetComponent<PlayerController>().FreezeInput(freeze);
        }
    }

    private void FreezeEnemies(bool freeze)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyController>().FreezeEnemy(freeze);
        }
    }

    private void FreezeBlasts(bool freeze)
    {
        GameObject[] blasts = GameObject.FindGameObjectsWithTag("Blast");
        foreach (GameObject blast in blasts)
        {
            blast.GetComponent<BlastScript>().FreezeBlast(freeze);
        }
    }

    public void PlayerDefeated()
    {
        isGameOver = true;
        gameRestartTime = gameRestartDelay;
        FreezePlayer(true);
        FreezeEnemies(true);
        GameObject[] blasts = GameObject.FindGameObjectsWithTag("Blast");
        foreach (GameObject blast in blasts)
        {
            Destroy(blast);
        }
        GameObject[] explosions = GameObject.FindGameObjectsWithTag("Explosion");
        foreach (GameObject explosion in explosions)
        {
            Destroy(explosion);
        }
    }

    private void ShowMessage()
    {
        string[] messages =
        {
            "GO GO GO",
            "YOU'RE DOING IT",
            "DONT STOP NOW",
            "SCORE THOSE POINTS",
            "GOOD LUCK YOU'RE GONNA NEED IT"
        };

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            messageIndex = firstMessage ? 0 : UnityEngine.Random.Range(0, messages.Length);
            messageText = messages[messageIndex];
            messageTimer = messageDelay;
            showMessage = true;
        }

        if (showMessage)
        {
            screenMessageText.alignment = TextAlignmentOptions.Center;
            screenMessageText.alignment = TextAlignmentOptions.Top;
            screenMessageText.fontStyle = FontStyles.UpperCase;
            screenMessageText.fontSize = 24;
            screenMessageText.text = messageText;

            messageTimer -= Time.deltaTime;
            if (messageTimer < 0)
            {
                screenMessageText.text = "";
                firstMessage = false;
                showMessage = false;
            }
        }
    }

    private void GetWorldViewCoordinates()
    {
        // get camera world coordinates just outside the left-bottom and top right views 
        Vector3 wv0 = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, -0.1f, 0));
        Vector3 wv1 = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 1.1f, 0));
        // and then update the world view coords var so it can be used 
        worldViewCoords.Left = wv0.x;
        worldViewCoords.Bottom = wv0.y;
        worldViewCoords.Right = wv1.x;
        worldViewCoords.Top = wv1.y;
    }

    private void SpawnEnemies()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            if (assetPalette == null)
            {
                assetPalette = GetComponent<AssetPalette>();
                enemyPrefabCount = Enum.GetNames(typeof(AssetPalette.EnemyList)).Length;
            }
            int randomEnemyCount = UnityEngine.Random.Range(1, 6);
            GameObject[] randomEnemies = new GameObject[randomEnemyCount];
            for (int i = 0; i < randomEnemyCount; i++)
            {
                int enemyIndex = UnityEngine.Random.Range(0, enemyPrefabCount);
                randomEnemies[i] = Instantiate(assetPalette.enemyPrefabs[enemyIndex]);
                randomEnemies[i].name = assetPalette.enemyPrefabs[enemyIndex].name;
                randomEnemies[i].transform.position = new Vector3(worldViewCoords.Right + UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(-1, 1f), 0);

                switch (randomEnemies[i].name)
                {
                    case "MushroomMan":
                        randomEnemies[i].transform.position = new Vector3(worldViewCoords.Right - UnityEngine.Random.Range(0.75f, 1.5f), 2f, 0);
                        break;
                    case "FlyingEye":
                        break;
                }
            }
        }
    }
    private void RepositionEnemies()
    {
        Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0, 0));
        Vector3 worldRight = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0, 0));

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.transform.position.y < worldViewCoords.Bottom)
            {
                switch (enemy.name)
                {
                    case "MushroomMan":
                        enemy.transform.position = new Vector3(worldViewCoords.Right - 2.5f, 2f, 0);
                        break;
                }
            }
            if (enemy.transform.position.x < worldLeft.x)
            {
                switch (enemy.name)
                {
                    case "Flying eye":
                        enemy.transform.position = new Vector3(worldViewCoords.Right, UnityEngine.Random.Range(-1.5f, 1.5f), 0);
                        enemy.GetComponent<FlyingEyeController>().ResetFollowingPath();
                        break;
                }
            }
        }
    }
    
    /*
    //Probabilities to pick what items to spawn
    private ItemScript.ItemTypes PickRandomBonusItem()
    {
        float[] probabilities =
        {
        12, 53, 15,
    };
        float total = 0;

        ItemScript.ItemTypes[] items = { ItemScript.ItemTypes.Nothing, ItemScript.ItemTypes.LifeEnergySmall, ItemScript.ItemTypes.LifeEnergyBig };

        foreach (float prob in probabilities)
        {
            total += prob;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < probabilities.Length; i++)
        {
            if (randomPoint < probabilities[i])
            {
                return items[i];
            }
            else
            {
                randomPoint -= probabilities[i];
            }
        }
        return items[probabilities.Length - 1];
    }
    
    public GameObject GetBonusItem(ItemScript.ItemTypes itemType)
    {
        GameObject bonusItem = null; 

        if(itemType == ItemScript.ItemTypes.Random)
        {
            itemType = PickRandomBonusItem(); 
        }

        switch(itemType)
        {
            case ItemScript.ItemTypes.Nothing:
                bonusItem = null; 
                break; 
            case ItemScript.ItemTypes.LifeEnergySmall:
                bonusItem = assetPalette.itemPrefabs[(int)AssetPalette.ItemList.LifeEnergySmall];
                break;
            case ItemScript.ItemTypes.LifeEnergyBig:
                bonusItem = assetPalette.itemPrefabs[(int)AssetPalette.ItemList.LifeEnergyBig];
                break;
        }

        return bonusItem; 
    }
    */ 
}
