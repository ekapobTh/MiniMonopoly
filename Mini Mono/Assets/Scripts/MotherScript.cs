using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MotherScript : MonoBehaviour
{
    [SerializeField] private string playerColor;
    // Start is called before the first frame update

    void Start()
    {
        DontDestroyOnLoad(this);
    }
    public string getPlayerColor()
    {
        return playerColor;
    }
    public void setPlayerColor(string playerCol)
    {
        playerColor = playerCol;
    }

}
