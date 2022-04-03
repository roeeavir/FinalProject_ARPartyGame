using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonScript : MonoBehaviour
{

    enum BalloonMovementState
    {
        Normal,
        Fast,
        Smart,
        FastSmart,
        Random,
        RandomFast,
    }

    public int groupId = 1;
    public int counter = 0;

    private float sideSpeed = 0.3f, normalSpeed = 0.4f, fastSpeed = 0.8f, smartSpeed = 0.5f, randomSpeed = 0.5f;

    private int score = 0;
    private float nextTimeToRandomize = 0f;


    Vector2 direction = new Vector2(0, 0);

    BalloonMovementState state = BalloonMovementState.Normal;

    void Start()
    {
        // Choose state at random
        state = (BalloonMovementState)Random.Range(groupId * 2 - 2, groupId * 2);
        score = (int)state + 1;
    }

    // Update is called once per frame
    void Update()
    {
        counter++;

        switch (state)
        {
            case BalloonMovementState.Normal:
                transform.Translate(Vector3.up * Time.deltaTime * normalSpeed);
                break;
            case BalloonMovementState.Fast:
                transform.Translate(Vector3.up * Time.deltaTime * fastSpeed);
                break;
            case BalloonMovementState.Smart:
                transform.Translate(Vector3.up * Time.deltaTime * smartSpeed);
                if ((counter / 800) % 2 == 0)
                    transform.Translate(Vector3.right * Time.deltaTime * sideSpeed);
                else
                    transform.Translate(Vector3.left * Time.deltaTime * sideSpeed);
                break;
            case BalloonMovementState.FastSmart:
                transform.Translate(Vector3.up * Time.deltaTime * fastSpeed);
                if ((counter / 800) % 2 == 0)
                    transform.Translate(Vector3.right * Time.deltaTime * smartSpeed);
                else
                    transform.Translate(Vector3.left * Time.deltaTime * smartSpeed);
                break;
            case BalloonMovementState.Random:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.5f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed, fastSpeed * 2);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case BalloonMovementState.RandomFast:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.5f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed, fastSpeed * 4);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
        }
    }

    public int GetScore()
    {
        return score;
    }


}
