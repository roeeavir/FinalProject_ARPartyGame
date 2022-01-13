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
            if (photonPlayer.IsLocal && target != null & target.isAlive())
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
            if ((horizontal > 0 && vertical > 0) || (hori > 0 && verti > 0))
            {
                transform.localEulerAngles = new Vector3(0, 45, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if ((horizontal > 0 && vertical < 0) || (hori > 0 && verti < 0))
            {
                transform.localEulerAngles = new Vector3(0, 135, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if ((horizontal < 0 && vertical < 0) || (hori < 0 && verti < 0))
            {
                transform.localEulerAngles = new Vector3(0, -135, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if ((horizontal < 0 && vertical > 0) || (hori < 0 && verti > 0))
            {
                transform.localEulerAngles = new Vector3(0, -45, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if ((horizontal > 0 && vertical == 0) || (hori > 0 && verti == 0))
            {
                transform.localEulerAngles = new Vector3(0, 90, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if ((horizontal < 0 && vertical == 0) || (hori < 0 && verti == 0))
            {
                transform.localEulerAngles = new Vector3(0, -90, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if ((vertical > 0 && horizontal == 0) || (verti > 0 && hori == 0))
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else if ((vertical < 0 && horizontal == 0) || (verti < 0 && hori == 0))
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

                if (this.target.isAlive())
                    TakeDamage(5);

                if (this.target.isAlive())
                {
                    StartCoroutine(PlayerColorChange(4)); // change color to red
                }
                else
                {
                    Debug.Log("dead");
                    StartCoroutine(PlayerColorChange(5)); // change color to black
                }
            }
        }
    }
    private IEnumerator PlayerColorChange(int colorID)
    {
        // change this player color for everyother player
        Debug.Log("PlayerColorChange " + colors[colorID]);
        photonView.RPC("ChangeColor", RpcTarget.AllBuffered, colorID);

        // Is player that has been hit by bullet is alive
        if (this.target.isAlive())
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


    // IEnumerator PlayerColorChange()
    // {
    //     this.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    //     yield return new WaitForSeconds(2);
    //     this.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
    // }
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