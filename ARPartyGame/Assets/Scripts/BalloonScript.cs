using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonScript : MonoBehaviour
{
    public int counter = 0;
    // Update is called once per frame
    void Update()
    {
        counter++;
        transform.Translate(Vector3.up * Time.deltaTime * 0.2f);
       /* if((counter/800)%2 == 0)
            transform.Translate(Vector3.right * Time.deltaTime * 0.1f);
        else
            transform.Translate(Vector3.left * Time.deltaTime * 0.1f);*/
    }
}
