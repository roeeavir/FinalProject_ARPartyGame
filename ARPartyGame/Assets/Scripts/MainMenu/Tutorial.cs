using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{

    public Image original;
    public Sprite[] listOfImages;
    static int index = 0;


    public void Update(){
        original.sprite = listOfImages[index];
    }

    public void increaseIndex(){
        if(index < listOfImages.Length - 1){
            index++;
        }
        else
        {
            index = 0;
        }
    }

    public void decreaseIndex(){
        if(index > 0){
            index--;
        }
        else
        {
            index = listOfImages.Length - 1;
        }
    }
}
