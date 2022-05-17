using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used to control the enemy's variables
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

    // Speeds
    private const float SIDE_SPEED = 0.3f, NORMAL_SPEED = 0.4f, FAST_SPEED = 0.8f, SMART_SPEED = 0.5f;
    private float randomSpeed = 0.5f, bossSpeedMultiplier = 1;

    // Sizes
    private const float NORMAL_SIZE = 1, MID_SIZE = 0.5f, SMALL_SIZE = 0.35f;
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
            score = groupId / 3;
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
            case EnemyMovementState.Normal: // Normal movement - Moves up (groupId = 0)
                transform.Translate(Vector3.up * Time.deltaTime * NORMAL_SPEED);
                break;
            case EnemyMovementState.Fast: // Fast movement - Moves up a bit faster (groupId = 1)
                transform.Translate(Vector3.up * Time.deltaTime * FAST_SPEED);
                break;
            case EnemyMovementState.Smart: // Smart movement - Moves up and to the sides (groupId = 2)
                transform.Translate(Vector3.up * Time.deltaTime * SMART_SPEED);
                if ((counter / 800) % 2 == 0)
                    transform.Translate(Vector3.right * Time.deltaTime * SIDE_SPEED);
                else
                    transform.Translate(Vector3.left * Time.deltaTime * SIDE_SPEED);
                break;
            case EnemyMovementState.FastSmart: // Fast and smart movement - Moves up and to the sides faster (groupId = 3)
                transform.Translate(Vector3.up * Time.deltaTime * FAST_SPEED);
                if ((counter / 800) % 2 == 0)
                    transform.Translate(Vector3.right * Time.deltaTime * SMART_SPEED);
                else
                    transform.Translate(Vector3.left * Time.deltaTime * SMART_SPEED);
                break;
            case EnemyMovementState.Random: // Random movement - Moves to all sides randomly (groupId = 4)
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.5f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(SIDE_SPEED, FAST_SPEED * 2);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomFast: // Random fast movement - Moves to all sides randomly a bit faster (groupId = 5)
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.5f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(SIDE_SPEED, FAST_SPEED * 4);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomFaster: // Random faster movement - Moves to all sides randomly faster (groupId = 6)
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.5f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(SIDE_SPEED * 2, FAST_SPEED * 5);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomerFaster: // Randomer faster movement - Moves to all sides randomly faster and more frequent (groupId = 7)
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.4f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(SIDE_SPEED * 2, FAST_SPEED * 6);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomerFastest: // Randomer fastest movement - Moves to all sides randomly very fast and more frequent (groupId = 8)
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.4f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(SIDE_SPEED * 2, FAST_SPEED * 8);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomestFastest: // Randomest fastest movement - Moves to all sides randomly very fast and a lot more frequent (groupId = 9)
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.30f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(SIDE_SPEED * 3, FAST_SPEED * 8);
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomerFasterSize: // Randomer faster movement - Moves to all sides randomly faster and more frequent while changing size (groupId = 10)
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.4f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(SIDE_SPEED * 2, FAST_SPEED * 6);
                    if (!isSmall)
                    {
                        isSmall = true;
                        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    }
                    else
                    {
                        isSmall = false;
                        transform.localScale = new Vector3(NORMAL_SIZE, NORMAL_SIZE, NORMAL_SIZE);
                    }
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.RandomestFastestSize:
                /* Randomest fastest movement with size - Moves to all sides randomly very fast and a lot more 
                frequent while changing (to a much smaller) size (groupId = 11) */
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.30f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(SIDE_SPEED * 3, FAST_SPEED * 8);
                    if (!isSmall)
                    {
                        isSmall = true;
                        transform.localScale = new Vector3(SMALL_SIZE, SMALL_SIZE, SMALL_SIZE);
                    }
                    else
                    {
                        isSmall = false;
                        transform.localScale = new Vector3(MID_SIZE, MID_SIZE, MID_SIZE);
                    }
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            case EnemyMovementState.Boss: // Boss movement - getting faster with each time of getting hit while changing size (groupId = 12)
                if (Time.time >= nextTimeToRandomize)
                {
                    nextTimeToRandomize = Time.time + 0.30f;
                    transform.Translate(direction * 0f);
                    direction = Random.insideUnitCircle.normalized;
                    randomSpeed = Random.Range(SIDE_SPEED * 3, FAST_SPEED * bossSpeedMultiplier * (groupId / 100));
                    if (!isSmall)
                    {
                        isSmall = true;
                        transform.localScale = new Vector3(SMALL_SIZE, SMALL_SIZE, SMALL_SIZE);
                    }
                    else
                    {
                        isSmall = false;
                        transform.localScale = new Vector3(MID_SIZE, MID_SIZE, MID_SIZE);
                    }
                }
                transform.Translate(direction * Time.deltaTime * randomSpeed);
                break;
            default:
                transform.Translate(Vector3.up * Time.deltaTime * NORMAL_SPEED);
                break;
        }
        if (Vector3.Distance(transform.position, position) > 15f) // If the enemy is too far from the player, it will be destroyed or teleported (if boss)
        {
            if (groupId < 100)
            {
                Destroy(gameObject); // Destroy the enemy
            }
            else
            {
                transform.position = SpawnPointsScript.CreateNewSpawnPoint().position; // Teleport the enemy
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

    // Appends the speed multiplier of the boss
    public void AppenedBossSpeedMultiplier()
    {
        bossSpeedMultiplier++;
    }

    public void SetFreeze(bool freeze)
    {
        this.freeze = freeze;
    }

}