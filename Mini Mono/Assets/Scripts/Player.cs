using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerCam;
    [SerializeField] private int hp;
    [SerializeField] private bool auto;
    [SerializeField] private string objColor;
    [SerializeField] GameObject currentPlate;
    [SerializeField] private Button diceBut;
    [SerializeField] private GameObject healthObj;
    private int maxHp = 9;
    private float rollFast = 0.15f;
    private float rollMedium = 0.35f;
    private float rollSlow = 0.75f;
    private float actionCoolDown = 1f;
    private float jumpLimit = 1.1f;
    private Collector diceCollector;
    private Collector plateCollector;
    GameplayService gameplayService;
    GameObject diceObj;
    int rollGet;
    Animator anim;

    //Jump param
    [SerializeField] GameObject startPos;
    [SerializeField] GameObject destPos;
    float timer;
    float jumpTime = 1f;
    bool stateJump;

    //Move Camera
    bool stateMoveCam;
    float camTimer;
    [SerializeField] private GameObject camStart;
    float camJumpTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        healthObj.SetActive(false);
        auto = (gameObject.tag == "Bot");
        gameplayService = GameObject.Find("GameplayObj").GetComponent<GameplayService>();
        hp = maxHp;
        healthObj.transform.GetChild(1).GetComponent<Text>().text = hp.ToString();
        timer = 5f;
        camTimer = 5f;
        stateJump = false;
        stateMoveCam = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (timer <= 1.0)
        {
            doJump();
            doRotate();
        }
        else
        {
            if (stateJump)
            {
                JumpSet();
            }
        }
        if (stateMoveCam)
        {
            doMoveCamera();
        }
    }
    public string getObjColor()
    {
        return objColor;
    }
    public GameObject getPlayerCam()
    {
        return playerCam;
    }
    public void TurnStart()
    {
        healthObj.SetActive(true);
        playerCam.SetActive(true);
        rollGet = 0;
        diceCollector = GameObject.Find("Dice").GetComponent<Collector>();
        plateCollector = GameObject.Find("PlateCollector").GetComponent<Collector>();
        diceBut = GameObject.Find("DicePreview").GetComponent<Button>();
        diceObj = GameObject.Find("Dice");
        StartCoroutine(onStateMoveCam());
    }
    public void RollDice()
    {
        FindObjectOfType<AudioService>().Play("Click");
        StartCoroutine(Roll());
    }
    public void JumpSet()
    {
        if (timer > 1.0f)
        {
            if (rollGet >= 0)
            {
                FindObjectOfType<AudioService>().Play("Jump");
                startPos = currentPlate.transform.GetChild(0).gameObject;
                destPos = CalculateNextPlate();
                timer = 0.0f;
                stateJump = true;
                setCurrentPlate(destPos);
                rollGet--;
            }
            else
            {
                stateJump = false;
                HealthCalculate();
            }
        }
    }
    public void doJump()
    {
        timer += Time.deltaTime / jumpTime;
        var height = Mathf.Sin(Mathf.PI * timer) * jumpLimit;
        transform.position = Vector3.Lerp(startPos.transform.position, destPos.transform.GetChild(0).position, timer) + Vector3.up * height;
        camStart.transform.position = playerCam.transform.position;
        camStart.transform.rotation = playerCam.transform.rotation;
    }
    public void doRotate()
    {
        transform.rotation = Quaternion.Lerp(startPos.transform.rotation, destPos.transform.GetChild(0).rotation, timer);
    }
    public void HealthCalculate()
    {
        var hpCalculate = 0;
        var doInsert = true;
        if (currentPlate.transform.childCount == 1)
        {
            //regen hp
            doInsert = false;
            if (currentPlate.name.Contains(objColor))
            {
                hpCalculate = 3;
            }
            else
            {
                hpCalculate = 1;
            }
        }
        else
        {
            var coinCollector = currentPlate.GetComponent<Collector>().getCollect();
            for (int i = 0; i < coinCollector.Length; i++)
            {
                if (coinCollector[i].GetComponent<TokenService>().getColor() == ColorService.Instance.defultColor) { break; }
                hpCalculate--;
            }
        }
        RegenHp(hpCalculate, doInsert);
    }
    public void CoinInsert()
    {
        var coinCollector = currentPlate.GetComponent<Collector>().getCollect();
        var changeObj = new GameObject();
        var currentColor = "";
        changeObj = null;
        //No Color
        if (coinCollector[0].GetComponent<TokenService>().getColor() == ColorService.Instance.defultColor)
        {
            changeObj = coinCollector[0];
            currentColor = ColorService.Instance.defultColor;
        }
        //Same Color
        else if (coinCollector[0].GetComponent<TokenService>().getColor() == objColor)
        {
            if(coinCollector[coinCollector.Length-1].GetComponent<TokenService>().getColor() != objColor)
            {
                for (int i = 0; i < coinCollector.Length; i++)
                {
                    if (coinCollector[i].GetComponent<TokenService>().getColor() == ColorService.Instance.defultColor)
                    {
                        changeObj = coinCollector[i];
                        currentColor = changeObj.GetComponent<TokenService>().getColor();
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("Same Color Same Coin Already Full");
            }
        }
        //Other Color
        else
        {
            var changeCoin = 0;
            for (int i = 0; i < coinCollector.Length; i++)
            {
                if (coinCollector[i].GetComponent<TokenService>().getColor() == ColorService.Instance.defultColor)
                {
                    changeCoin--;
                    break;
                }
                changeCoin++;
            }
            if(changeCoin == 0)
            {
                changeObj = coinCollector[changeCoin];
                currentColor = changeObj.GetComponent<TokenService>().getColor();
            }
        }
        if(changeObj == null)
        {
            TurnEnd();
        }
        else
        {
            StartCoroutine(SetColor(changeObj, currentColor));
        }
    }
    IEnumerator SetColor(GameObject obj,string color)
    {
        yield return new WaitForSeconds(jumpLimit);
        obj.GetComponent<TokenService>().SetColor(objColor);
        FindObjectOfType<AudioService>().Play("CoinWink");
        yield return new WaitForSeconds(rollMedium);
        obj.GetComponent<TokenService>().SetColor(color);
        yield return new WaitForSeconds(rollMedium);
        obj.GetComponent<TokenService>().SetColor(objColor);
        FindObjectOfType<AudioService>().Play("CoinWink");
        yield return new WaitForSeconds(rollMedium);
        obj.GetComponent<TokenService>().SetColor(color);
        yield return new WaitForSeconds(rollMedium);
        obj.GetComponent<TokenService>().SetColor(objColor);
        FindObjectOfType<AudioService>().Play("CoinFull");
        yield return new WaitForSeconds(jumpLimit);
        TurnEnd();
    }
    IEnumerator Die()
    {
        FindObjectOfType<AudioService>().Play("DeathVoice");
        healthObj.SetActive(false);
        anim.SetBool("isDie", true);
        yield return new WaitForSeconds(actionCoolDown);
        if (tag != "Player")
        {
            gameplayService.Eliminate();
            TurnEnd();
        }
        else
        {
            gameplayService.doEndScene("Game Over");
        }
    }
    IEnumerator Roll()
    {
        if(gameObject.tag != "Player") { yield return new WaitForSeconds(actionCoolDown); }
        diceBut.gameObject.SetActive(false);
        int rollFastCount = Random.Range(8,12);
        var diceOut = diceCollector.getCollect();
        while (rollFastCount > 0)
        {
            rollGet = Random.Range(0, 5);
            diceOut[rollGet].SetActive(true);
            yield return new WaitForSeconds(rollFast);
            diceOut[rollGet].SetActive(false);
            rollFastCount--;
        }
        int rollMediumCount = Random.Range(6, 10);
        while (rollFastCount > 0)
        {
            rollGet = Random.Range(0, 5);
            diceOut[rollGet].SetActive(true);
            yield return new WaitForSeconds(rollMedium);
            diceOut[rollGet].SetActive(false);
            rollFastCount--;
        }
        int rollSlowCount = Random.Range(4, 8);
        while (rollFastCount > 0)
        {
            rollGet = Random.Range(0, 5);
            diceOut[rollGet].SetActive(true);
            yield return new WaitForSeconds(rollSlow);
            diceOut[rollGet].SetActive(false);
            rollFastCount--;
        }
        rollGet = Random.Range(0, 5);
        diceOut[rollGet].SetActive(true);
        yield return new WaitForSeconds(actionCoolDown);
        ResetDice();
        JumpSet();
    }
    public void TurnEnd()
    {
        healthObj.SetActive(false);
        gameplayService.NextPlayer();
        gameplayService.NextTurn();
    }
    public int GetCurrentHP()
    {
        return hp;
    }
    public void RegenHp(int healAmount,bool isInsert)
    {
        hp = (hp + healAmount) > maxHp ? maxHp : (hp + healAmount);
        healthObj.transform.GetChild(1).GetComponent<Text>().text = hp.ToString();
        if (hp > 0)
        {
            if (isInsert) { CoinInsert(); }
            else { TurnEnd(); }
        }
        else { StartCoroutine(Die()); }
    }
    public void setObjColor(string color)
    {
        objColor = color;
    }
    public void setCurrentPlate(GameObject plate)
    {
        currentPlate = plate;
    }
    GameObject CalculateNextPlate()
    {
        var returnObj = new GameObject();
        var plateArr = plateCollector.getCollect();
        for (int i = 0;i < plateArr.Length; i++)
        {
            if(plateArr[i].name == currentPlate.name)
            {
                if(i+1 == plateArr.Length)
                {
                    returnObj = plateArr[0];
                }
                else
                {
                    returnObj = plateArr[i+1];
                }
                break;
            }
        }
        return returnObj;
    }
    public void ResetDice()
    {
        for(int i = 0;i < diceObj.transform.childCount - 1; i++)
        {
            diceObj.transform.GetChild(i).gameObject.SetActive(false);
        }
        diceObj.transform.GetChild(diceObj.transform.childCount-1).gameObject.SetActive(true);
        diceObj.SetActive(false);
    }
    public void setStateMoveCam()
    {
        stateMoveCam = true;
        camTimer = 0f;
        camStart = Camera.main.gameObject;
    }
    IEnumerator onStateMoveCam()
    {
        setStateMoveCam();
        yield return new WaitForSeconds(camJumpTime);

        if (auto)
        {
            RollDice();
        }
        else
        {
            diceBut.onClick.AddListener(RollDice);
        }
    }
    public void doMoveCamera()
    {
        if(camTimer <= 1.0f)
        {
            camTimer += Time.deltaTime / camJumpTime;
            camStart.transform.position = Vector3.Lerp(camStart.transform.position, playerCam.transform.position, camTimer);
            camStart.transform.rotation = Quaternion.Lerp(camStart.transform.rotation, playerCam.transform.rotation, camTimer);
        }
        else
        {
            stateMoveCam = false;
        }
    }
}
