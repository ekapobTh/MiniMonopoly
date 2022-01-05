using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorService : MonoBehaviour
{
    private static ColorService _instance;
    public static ColorService Instance { get { return _instance; } }

    public string defultColor = "white";
    [SerializeField] private Material[] colorArr;

    private void Awake()
    {
        _instance = this;
    }
    public Material[] getColorArr()
    {
        return colorArr;
    }
    public Material findColorArr(string name)
    {
        Material returnMaterial = colorArr[0];
        for(int i = 1;i < colorArr.Length; i++)
        {
            if(colorArr[i].name == name)
            {
                returnMaterial = colorArr[i];
                break;
            }
        }
        return returnMaterial;
    }
}
