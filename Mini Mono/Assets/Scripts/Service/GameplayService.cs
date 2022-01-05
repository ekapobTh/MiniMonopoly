using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayService : MonoBehaviour
{
    List<string> botColor;
    public GameObject[] prefabs;
    List<GameObject> unit;
    Collector spawnerCollect;
    Collector plateCollector;
    [SerializeField] GameObject diceObj;
    [SerializeField] GameObject endObj;

    private float spawnCooldown = 0.75f;
    private float turnCooldown = 1f;
    GameObject playerPrefabs;
    Transform playerPos;
    GameObject botPrefabs;
    Transform botPos;
    Player currentPlayerScript;
    public GameObject dicePanel;
    public GameObject exitPauseBut;

    [SerializeField] private int currentPlayer;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AudioService>().Play("GameIntro");
        endObj.SetActive(false);
        diceObj.SetActive(false);
        MotherScript motherScript = GameObject.Find("MotherObj").GetComponent<MotherScript>();
        plateCollector = GameObject.Find("PlateCollector").GetComponent<Collector>();
        spawnerCollect = gameObject.GetComponent<Collector>();
        initInGame(motherScript.getPlayerColor());
        StartCoroutine(InitPlayer(motherScript.getPlayerColor()));
    }
    void initInGame(string playerColor)
    {
        unit = new List<GameObject>();
        botColor = new List<string>();
        botColor.Add("red");
        botColor.Add("blue");
        botColor.Add("green");
        botColor.Add("yellow");
        botColor.Remove(playerColor);
    }
    IEnumerator InitPlayer(string playerColor)
    {
        yield return new WaitForSeconds(spawnCooldown);
        var plateSet = new GameObject();
        for(int i = 0;i < prefabs.Length; i++)
        {
            if (prefabs[i].name.Contains(playerColor))
            {
                playerPrefabs = prefabs[i];
                break;
            }
        }
        for(int i = 0;i < spawnerCollect.getCollect().Length; i++)
        {
            if (spawnerCollect.getCollect()[i].name.Contains(playerColor))
            {
                plateSet = spawnerCollect.getCollect()[i];
                playerPos = plateSet.transform.GetChild(0);
                break;
            }
        }
        FindObjectOfType<AudioService>().Play("Spawn");
        GameObject player = Instantiate(playerPrefabs, playerPos.position, playerPos.rotation);
        player.GetComponent<Player>().setObjColor(playerColor);
        player.GetComponent<Player>().setCurrentPlate(plateSet);
        player.tag = "Player";
        unit.Add(player);
        yield return new WaitForSeconds(spawnCooldown);
        StartCoroutine(InitBot());
    }
    IEnumerator InitBot()
    {
        while (botColor.Count > 0)
        {
            var plateSet = new GameObject();
            string currentBotColor = botColor[Random.Range(0, botColor.Count - 1)];
            botColor.Remove(currentBotColor);
            for (int i = 0; i < prefabs.Length; i++)
            {
                if (prefabs[i].name.Contains(currentBotColor))
                {
                    botPrefabs = prefabs[i];
                    break;
                }
            }
            for (int i = 0; i < spawnerCollect.getCollect().Length; i++)
            {
                if (spawnerCollect.getCollect()[i].name.Contains(currentBotColor))
                {
                    plateSet = spawnerCollect.getCollect()[i];
                    botPos = plateSet.transform.GetChild(0);
                    break;
                }
            }
            FindObjectOfType<AudioService>().Play("Spawn");
            GameObject bot = Instantiate(botPrefabs, botPos.position, botPos.rotation);
            bot.GetComponent<Player>().setObjColor(currentBotColor);
            bot.GetComponent<Player>().setCurrentPlate(plateSet);
            bot.tag = "Bot";
            unit.Add(bot);
            yield return new WaitForSeconds(spawnCooldown);
            bot.GetComponent<Player>().getPlayerCam().SetActive(false);
        }
        FindObjectOfType<AudioService>().Play("InGameBGM");
        NextTurn();
    }
    public void NextTurn()
    {
        if (unit.Count > 1)
        {
            currentPlayerScript = unit[currentPlayer].GetComponent<Player>();
            diceObj.SetActive(true); 
            if (unit[currentPlayer].tag == "Player") { dicePanel.SetActive(false); }
            else { dicePanel.SetActive(true); }
            currentPlayerScript.TurnStart();
        }
        else
        {
            doEndScene("Winner : " + unit[unit.Count- 1].GetComponent<Player>().getObjColor());
        }
    }
    public void NextPlayer()
    {
        currentPlayer = currentPlayer + 1 < unit.Count ? currentPlayer + 1 : 0;
    }
    public void doEndScene(string endStr)
    {
        endObj.SetActive(true);
        endObj.transform.GetChild(0).GetComponent<Text>().text = endStr;
    }
    public void Eliminate()
    {
        var coinCollector = transform.GetChild(0).GetComponent<Collector>().getCollect();
        for(int i = 0;i < coinCollector.Length; i++)
        {
            if(coinCollector[i].transform.childCount > 1)
            {
                var coinObj = coinCollector[i].GetComponent<Collector>().getCollect();
                if (coinObj[0].GetComponent<TokenService>().getColor() == unit[currentPlayer].GetComponent<Player>().getObjColor())
                {
                    for(int j = 0;j < coinObj.Length; j++)
                    {
                        coinObj[j].GetComponent<TokenService>().SetColor(ColorService.Instance.defultColor);
                    }
                }
            }
        }
        unit.RemoveAt(currentPlayer);
        //clear coin
    }
    public void OnClickMainMenu()
    {
        FindObjectOfType<AudioService>().Stop("InGameBGM");
        FindObjectOfType<AudioService>().Play("Click");
        SceneManager.LoadScene("MainMenu");
    }
    public void OnClickContinueWatch()
    {
        FindObjectOfType<AudioService>().Play("Click");
        Eliminate();
        unit[currentPlayer].GetComponent<Player>().TurnEnd();
    }
    public void OnClickPause()
    {
        FindObjectOfType<AudioService>().Play("Click");
        doEndScene("Main menu ?");
        exitPauseBut.SetActive(true);
    }
    public void OnClickExitPause()
    {
        FindObjectOfType<AudioService>().Play("Click");
        endObj.SetActive(false);
        exitPauseBut.SetActive(false);
    }
}
