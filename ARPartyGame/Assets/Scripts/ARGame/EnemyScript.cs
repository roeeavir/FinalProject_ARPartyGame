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
        RandomerFastest,
        RandomestFastest,
        RandomerFasterSize,
        RandomestFastestSize,
        Boss

    }

    public int groupId = 1;
    private int counter = 0;

    private float sideSpeed = 0.3f, normalSpeed = 0.4f, fastSpeed = 0.8f, smartSpeed = 0.5f, randomSpeed = 0.5f, bossSpeedMultiplier = 1;

    private int score = 0;
    private float nextTimeToRandomize = 0f;

    private bool isSmall = false;

    private Vector3 position;

    private Vector2 direction = new Vector2(0, 0);

    private EnemyMovementState state = EnemyMovementState.Normal;

    private bool freeze = false;


    void Start()
    {
        // Choose state at random
        if (groupId >= 100)
        {
            state = EnemyMovementState.Boss;
            score = groupId / 5;
        }
        else
        {
            state = (EnemyMovementState)Random.Range(groupId * 2 - 2, groupId * 2);
            score = (int)state + 1;
        }

        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze)
        {
            return;
        }

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
                    randomSpeed = Random.Range(sideSpeed * 2, fastSpeed * 5);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomerFaster:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.4f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed * 2, fastSpeed * 6);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomerFastest:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.4f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed * 2, fastSpeed * 8);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomestFastest:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.30f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed * 3, fastSpeed * 8);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomerFasterSize:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.4f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed * 2, fastSpeed * 6);
                    if (!isSmall)
                    {
                        isSmall = true;
                        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    }
                    else
                    {
                        isSmall = false;
                        transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomestFastestSize:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.30f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed * 3, fastSpeed * 8);
                    if (!isSmall)
                    {
                        isSmall = true;
                        transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                    }
                    else
                    {
                        isSmall = false;
                        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    }
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.Boss:
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.30f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(sideSpeed * 3, fastSpeed * bossSpeedMultiplier * (groupId / 100));
                    if (!isSmall)
                    {
                        isSmall = true;
                        transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                    }
                    else
                    {
                        isSmall = false;
                        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    }
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            default:
                transform.Translate(Vector3.up * Time.deltaTime * normalSpeed);
                break;
        }
        if (Vector3.Distance(transform.position, position) > 15f)
        {
            if (groupId < 100)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.position = SpawnPointsScript.CreateNewSpawnPoint().position;
                position = transform.position;
            }

        }
    }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int score)
    {
        this.score = score;
    }

    public void AppenedBossSpeedMultiplier()
    {
        bossSpeedMultiplier++;
    }

    public void SetFreeze(bool freeze)
    {
        this.freeze = freeze;
    }

}