using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    public string text = "";	


    // Update is called once per frame
    void Update()
    {
        transform.GetComponent<TextMesh>().text = text;
    }

    public void SetText(string text)
    {
        this.text = text;
    }
}
