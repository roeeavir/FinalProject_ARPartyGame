using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    enum EnemyMovementState
    {
        Normal,
        Fast,
        Smart,
        FastSmart,
        Random,
        RandomFast,
        RandomFaster,
        RandomerFaster,
        RandomestFastest,
    }

    public int groupId = 1;
    public int counter = 0;

    private float sideSpeed = 0.3f, normalSpeed = 0.4f, fastSpeed = 0.8f, smartSpeed = 0.5f, randomSpeed = 0.5f;

    private int score = 0;
    private float nextTimeToRandomize = 0f;


    Vector2 direction = new Vector2(0, 0);

    EnemyMovementState state = EnemyMovementState.Normal;

    void Start()
    {
        // Choose state at random
        state = (EnemyMovementState)Random.Range(groupId * 2 - 2, groupId * 2);
        score = (int)state + 1;
    }

    // Update is called once per frame
    void Update()
    {
        counter++;

        switch (state)
        {
            case EnemyMovementState.Normal:
                transform.Translate(Vector3.up * Time.deltaTime * normalSpeed);
                break;
            case EnemyMovementState.Fast:
                transform.Translate(Vector3.up * Time.deltaTime * fastSpeed);
                break;
            case EnemyMovementState.Smart:
                transform.Translate(Vector3.up * Time.deltaTime * smartSpeed);
                if ((counter / 800) % 2 == 0)
                    transform.Translate(Vector3.right * Time.deltaTime * sideSpeed);
                else
                    transform.Translate(Vector3.left * Time.deltaTime * sideSpeed);
                break;
            case EnemyMovementState.FastSmart:
                transform.Translate(Vector3.up * Time.deltaTime * fastSpeed);
                if ((counter / 800) % 2 == 0)
                    transform.Translate(Vector3.right * Time.deltaTime * smartSpeed);
                else
                    transform.Translate(Vector3.left * Time.deltaTime * smartSpeed);
                break;
            case EnemyMovementState.Random:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.5f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed, fastSpeed * 2);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomFast:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.5f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed, fastSpeed * 4);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomFaster:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.5f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed * 2, fastSpeed * 8);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomerFaster:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.4f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed * 2, fastSpeed * 12);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomestFastest:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.35f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed * 4, fastSpeed * 16);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            default:
                transform.Translate(Vector3.up * Time.deltaTime * normalSpeed);
                break;
        }
    }

    public int GetScore()
    {
        return score;
    }


}
