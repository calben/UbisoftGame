﻿using UnityEngine;
using System.Collections;

public class Killer_Mover : AI_Mover
{

  protected StatusUpdate myStatus;

  public float killSpeed;

  enum State
  {
    roaming,
    chasing
  }

  State currState;

  // Use this for initialization
  void Start()
  {
	
    StartCoroutine(changeState(State.roaming));
	
    //setting agent
    this.agent = GetComponent<NavMeshAgent>();

    myStatus = GetComponentInChildren<StatusUpdate>();

    this.prevWaypoint = this.waypoint;

  }


  // Update is called once per frame
  void Update()
  {

    move();


    if (this.health <= 0 || this.gameObject.transform.position.y <= -15f)
    {

		kill ();

    }

    if (gameObject.GetComponent<Rigidbody>().velocity.magnitude >= 2.0f)
    {
      gameObject.GetComponent<Rigidbody>().velocity = gameObject.GetComponent<Rigidbody>().velocity * 0.5f;

    }


    RaycastHit hit;

    if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0f))
    {

      if (hit.distance <= 0.5f)
      {

        transform.position = new Vector3(transform.position.x, 0.9f, transform.position.z);

      }

    }

  }


  public void kill()
  {
	
		Destroy (this.gameObject);

  }

  protected void OnCollisionEnter(Collision other)
  {

    if (other.rigidbody == null)
    {
      return;

    }

    if (other.gameObject.tag.Equals("Player"))
    {
      GameObject.Find("GameManager").GetComponent<GameManager>().decreaseHealth("Killer");
      myStatus.updateText(true);

    }

  }


  public void damage()
  {
    this.GetComponent<NetworkView>().RPC("RPCDamage", RPCMode.All, damageTaken);
    //StartCoroutine(flashRed());
  }

  [RPC]
  public void RPCDamage(int damageAmount)
  {
    health = health - damageAmount;
    StartCoroutine(damageText());
  }

  IEnumerator damageText()
  {
    myStatus.hitText();
    yield return new WaitForSeconds(0.2f);
    myStatus.updateText(false);
  }

  protected override void react()
  {

    updateWaypoint(GameObject.FindGameObjectWithTag("Player").gameObject.transform);

  }


  public override void isInterested()
  {
    StartCoroutine(changeState(State.chasing));

    this.interested = true;

    this.myStatus.updateText(true);

    react();

  }


  public void notInterested()
  {
    StartCoroutine(changeState(State.roaming));

    this.interested = false;

    this.myStatus.updateText(false);

    updateWaypoint(this.prevWaypoint);

  }


  IEnumerator flashRed()
  {

    gameObject.GetComponent<Renderer>().material.color = Color.red;

    yield return new WaitForSeconds(0.2f);

    gameObject.GetComponent<Renderer>().material.color = Color.black;

  }


  IEnumerator changeState(State newState)
  {

    yield return new WaitForSeconds(1.0f);

    this.currState = newState;

  }


  #region Networking
  [RPC]
  public void UpdateFullKillerState(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, NetworkViewID id)
  {
    if (this.GetComponent<NetworkView>().viewID == id)
    {
      this.GetComponent<Rigidbody>().position = position;
      this.GetComponent<Rigidbody>().rotation = rotation;
      this.GetComponent<Rigidbody>().velocity = velocity;
      this.GetComponent<Rigidbody>().angularVelocity = angularVelocity;
    }
  }
  #endregion

  public void FixedUpdate()
  {
    #region NetworkUpdate
    if (Network.isServer)
    {
      Rigidbody rigidbody = this.GetComponent<Rigidbody>();
      PlayerFields fields = this.GetComponent<PlayerFields>();
      this.GetComponent<NetworkView>().RPC("UpdateFullKillerState", RPCMode.Others, rigidbody.position, rigidbody.rotation, rigidbody.velocity, rigidbody.angularVelocity, this.GetComponent<NetworkView>().viewID);
    }
    #endregion
  }
}
