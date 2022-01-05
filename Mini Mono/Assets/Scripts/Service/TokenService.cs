using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenService : MonoBehaviour
{
    [SerializeField] private string color;
    // Start is called before the first frame update
    void Start()
    {
        color = ColorService.Instance.defultColor;
    }
    public void SetColor(string name)
    {
        color = name;
        GetComponent<Renderer>().material = GameObject.Find("SceneScript").GetComponent<ColorService>().findColorArr(name);
    }
    public string getColor()
    {
        return color;
    }
}
