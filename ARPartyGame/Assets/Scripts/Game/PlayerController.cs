using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;
    [Header("Component")]
    public Rigidbody rig;
    public Player photonPlayer;
    public Text playerNickName;
    [SerializeField]
    private float speed = 0.2f;
    private float fireRate = 2f;
    private float nextTimeToFire = 0f;

    private Target target = null;

    private bool canLeft = true, canRight = true, canUp = true, canDown = true;

    private Color[] colors = { Color.cyan, Color.grey, Color.magenta, Color.blue, Color.red, Color.black };

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        Debug.Log("Player " + id + " initialized");
        speed = 0.2f;
        target = this.gameObject.GetComponent<Target>();
        GameManager.instance.players[id - 1] = this;
        // playerNickName.text = photonPlayer.NickName;
        if (!photonView.IsMine)
        {
            rig.isKinematic = true;
        }
        photonView.RPC("ChangeColor", RpcTarget.AllBuffered, id - 1);

    }
    private void Start()
    {
        speed = 0.2f;
        rig.isKinematic = true;
    }

    private void Update()
    {
        try
        {
            if (photonPlayer.IsLocal && target != null & target.IsAlive())
            {
                Movements();
                if ((Input.GetKey(KeyCode.LeftControl) || CrossPlatformInputManager.GetButton("Shoot")) && Time.time >= nextTimeToFire)
                {
                    //  && Time.time >= nextTimeToFire
                    nextTimeToFire = Time.time + 1f / fireRate;
                    photonView.RPC("Fire", RpcTarget.All);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.StackTrace);
        }

    }
    // Control the player movements
    private void Movements()
    {
        try
        {
            float horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");
            float hori = Input.GetAxis("Horizontal");
            float verti = Input.GetAxis("Vertical");
            if (horizontal != 0 || vertical != 0 || hori != 0 || verti != 0)
            {
                speed = 2f;
            }
            else
            {
                speed = 0;
            }
            if ((canRight && canUp && (horizontal > 0 && vertical > 0) || (hori > 0 && verti > 0))) // Diagonal
            {
                transform.localEulerAngles = new Vector3(0, 45, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if (canRight && canDown && ((horizontal > 0 && vertical < 0) || (hori > 0 && verti < 0))) // Diagonal
            {
                transform.localEulerAngles = new Vector3(0, 135, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if (canLeft && canDown && ((horizontal < 0 && vertical < 0) || (hori < 0 && verti < 0))) // Diagonal
            {
                transform.localEulerAngles = new Vector3(0, -135, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if (canLeft && canUp && ((horizontal < 0 && vertical > 0) || (hori < 0 && verti > 0))) // Diagonal
            {
                transform.localEulerAngles = new Vector3(0, -45, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if (canRight && ((horizontal > 0 && vertical == 0) || (hori > 0 && verti == 0))) // Horizontal
            {
                transform.localEulerAngles = new Vector3(0, 90, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if (canLeft && ((horizontal < 0 && vertical == 0) || (hori < 0 && verti == 0))) // Horizontal
            {
                transform.localEulerAngles = new Vector3(0, -90, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if (canUp && ((vertical > 0 && horizontal == 0) || (verti > 0 && hori == 0))) // Vertical
            {
                transform.localEulerAngles = new Vector3(0, 0, 0); // up
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if (canDown && ((vertical < 0 && horizontal == 0) || (verti < 0 && hori == 0))) // Vertical
            {
                transform.localEulerAngles = new Vector3(0, 180, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Yese " + e.StackTrace);
        }

    }

    [PunRPC]
    void Fire()
    {
        GameObject bullet = Instantiate(Resources.Load("bullet", typeof(GameObject))) as GameObject;
        FindObjectOfType<SoundController>().Play("shoot"); // play sound
        bullet.name = photonPlayer.NickName; // set name
        Rigidbody rb = bullet.GetComponent<Rigidbody>(); // get rigidbody
        bullet.transform.localPosition = transform.position; // set position
        bullet.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f); // set scale
        rb.AddForce(this.transform.forward * 200f); // add force
        Destroy(bullet, 1); // destroy after 1 second
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "bullet")
        {
            if (other.name != photonPlayer.NickName)
            {
                Debug.Log("hit " + other.name);
                // destroy bullet
                Destroy(other.gameObject);

                if (!photonView.IsMine)
                {
                    return;
                }

                if (this.target.IsAlive())
                    TakeDamage(5);

                if (this.target.IsAlive())
                {
                    StartCoroutine(PlayerColorChange(4)); // change color to red
                }
                else
                {
                    Debug.Log("dead");
                    StartCoroutine(PlayerColorChange(5)); // change color to black
                }
            }
        } else if (other.tag.Equals("wall"))
        {
            Debug.LogWarning("wall enter");
            if (other.transform.name.Equals("UpWall"))
            {
                canUp = false;
            }
            else if (other.transform.name.Equals("DownWall"))
            {
                canDown = false;
            }
            else if (other.transform.name.Equals("LeftWall"))
            {
                canLeft = false;
            }
            else if (other.transform.name.Equals("RightWall"))
            {
                canRight = false;
            }
              
        }
    }

    // On collision exit
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "wall")
        {
            if (other.transform.name.Equals("UpWall"))
            {
                canUp = true;
            }
            else if (other.transform.name.Equals("DownWall"))
            {
                canDown = true;
            }
            else if (other.transform.name.Equals("LeftWall"))
            {
                canLeft = true;
            }
            else if (other.transform.name.Equals("RightWall"))
            {
                canRight = true;
            }
            Debug.LogWarning("wall exit");
        }
    }
    private IEnumerator PlayerColorChange(int colorID)
    {
        // change this player color for everyother player
        Debug.Log("PlayerColorChange " + colors[colorID]);
        photonView.RPC("ChangeColor", RpcTarget.AllBuffered, colorID);

        // Is player that has been hit by bullet is alive
        if (this.target.IsAlive())
        {
            yield return new WaitForSeconds(1f);
            photonView.RPC("ChangeColor", RpcTarget.AllBuffered, id - 1);
        }

    }

    [PunRPC]
    void ChangeColor(int colorID)
    {
        this.gameObject.GetComponent<MeshRenderer>().material.color = colors[colorID];
    }


    private void TakeDamage(int damage) // this is called by the player that is hit by the bullet
    {
        this.target.TakeDamage(damage);
    }

    // [PunRPC]
    // void PlayerDead(int id)
    // {
    //     Debug.Log("Player " + id + " is dead");
    //     Debug.Log("GameManager.instance.players[id - 1] " + GameManager.instance.players[id - 1].playerNickName + " is dead");
    //     GameManager.instance.players[id - 1].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    // }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // stream.SendNext(this.gameObject.GetComponent<MeshRenderer>().material.color);
            // stream.SendNext(transform.rotation);
        }
        else
        {
            // this.transform.position = (Vector3)stream.ReceiveNext();
            // this.transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

}