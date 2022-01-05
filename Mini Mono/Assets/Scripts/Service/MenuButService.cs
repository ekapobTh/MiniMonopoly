using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButService : MonoBehaviour
{
    public GameObject chooseColorPanel;
    public Text countDownTxt;
    public GameObject countDownObj;
    public GameObject cancelCountDownObj;
    public Image playerImg;

    public Button colorPlayerButton;

    private int playerCount;
    private int playerColorCount;

    public Color[] playerColor;
    private bool startGame;
    
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AudioService>().Play("BGM");
        InitStart();
    }
    void InitStart()
    {
        Button countDownBut = countDownObj.GetComponent<Button>();
        playerColorCount = 0;
        playerImg.color = playerColor[playerColorCount];
        playerCount = 2;
        countDownTxt.text = "";
        startGame = false;
        cancelCountDownObj.SetActive(startGame);
        chooseColorPanel.SetActive(false);
        countDownBut.interactable = true;
        colorPlayerButton.interactable = true;
    }
    public void OnClickStart()
    {
        FindObjectOfType<AudioService>().Play("Click");
        FindObjectOfType<AudioService>().Stop("BGM");
        chooseColorPanel.SetActive(true);
    }
    public void OnClickExit()
    {
        FindObjectOfType<AudioService>().Play("Click");
        Application.Quit();
    }
    public void OnClickChangeColor()
    {
        FindObjectOfType<AudioService>().Play("Click");
        playerColorCount = playerColorCount + 1 == playerColor.Length ? 0 : playerColorCount + 1;
        playerImg.color = playerColor[playerColorCount];
    }
    public void OnClickCountDown()
    {
        FindObjectOfType<AudioService>().Play("Click");
        MotherScript motherScript = GameObject.Find("MotherObj").GetComponent<MotherScript>();
        motherScript.setPlayerColor(getColorName(playerImg.color));
        SceneManager.LoadScene("GamePlay");
    }
    string getColorName(Color color)
    {
        if (color == playerColor[0])
        {
            return "yellow";
        }
        else if (color == playerColor[1])
        {
            return "red";
        }
        else if (color == playerColor[2])
        {
            return "green";
        }
        else
        {
            return "blue";
        }
    }

}
