using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{

    public Image origin;
    public Sprite[] tutorialImages;

    public static int index= 0;


    // Update is called once per frame
    void Update()
    {
        origin.sprite = tutorialImages[index];
        
    }

    public void Next()
    {
        if (index < tutorialImages.Length - 1)
        {
            index++;
            origin.sprite = tutorialImages[index];
        }
        else
        {
            index = 0;
            origin.sprite = tutorialImages[index];
        }
    }

    public void Previous()
    {
        if (index > 0)
        {
            index--;
            origin.sprite = tutorialImages[index];
        }
        else
        {
            index = tutorialImages.Length - 1;
            origin.sprite = tutorialImages[index];
        }
    }

    public void onExit()
    {
        index = 0;
    }
}
