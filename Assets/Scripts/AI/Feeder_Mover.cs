﻿using UnityEngine;
using System.Collections;

public class Feeder_Mover : AI_Mover
{

  public float killSpeed;
  public float timeUntilBored;
  public float smellingRadius;
  public float pullRadius;
  public float pullForce;
  public GameObject EnviroTile;
  public AudioManager _audioManager;

  Transform tempTarget;
  bool isFeeding;
  int resourcesEaten;
  float tempSpeed;
  StatusUpdate myStatus;

  enum State
  {
    roaming,
    eating
  }

  State currState;

  // Use this for initialization
  void Start()
  {
    StartCoroutine(changeState(State.roaming));

    //setting agent
    this.agent = GetComponent<NavMeshAgent>();

    this.tempSpeed = this.agent.speed;

    this.myStatus = GetComponentInChildren<StatusUpdate>();

    //gameObject.renderer.material.color = Color.magenta;

    this.prevWaypoint = this.waypoint;

  }


  // Update is called once per frame
  void Update()
  {

    move();

    if (health <= 0)
    {

      kill();

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

        transform.position = new Vector3(transform.position.x, 1.2f, transform.position.z);

      }

    }

  }

  public void setTempTarget(Transform nextWaypoint)
  {

    this.tempTarget = nextWaypoint;

  }



  /*
  IEnumerator falcuhnPuuull()
  {
    tempSpeed = agent.speed;
    agent.speed = 0.0f;

    foreach(Collider collider in Physics.OverlapSphere(gameObject.transform.position, pullRadius))
    {
      if(collider.gameObject.tag.Equals("EnviroTile"))
      {
        Debug.Log ("NOMNOM PULL");
				
        Vector3 directedForce = transform.position - collider.transform.position;
				
        collider.rigidbody.AddForce (directedForce.normalized * pullForce * Time.deltaTime);
      }

    }

    yield return new WaitForSeconds (4.0f);
    notInterested ();

      void FixedUpdate()
  {
    if(interested){
		
      tempSpeed = agent.speed;
      agent.speed = 0.0f;
			
      foreach(Collider collider in Physics.OverlapSphere(gameObject.transform.position, pullRadius))
      {
        if(collider.gameObject.tag.Equals("EnviroTile"))
        {

          Vector3 directedForce = transform.position - collider.transform.position;
					
          collider.rigidbody.AddForce (directedForce.normalized * pullForce * Time.deltaTime);
        }
			
      }
		
      notInterested ();
    }

  }

  }*/

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

  public void kill()
  {

    int i = 0;
    GameObject tempGameObj;
    while (i < resourcesEaten)
    {

      tempGameObj = Instantiate(EnviroTile, transform.position, Quaternion.identity) as GameObject;

    }

    Destroy(this.gameObject);

  }

  protected void OnCollisionEnter(Collision other)
  {

    if (other.gameObject.tag.Equals("EnviroTile"))
    {

      Destroy(other.gameObject);
      resourcesEaten++;
      _audioManager.Play("enemy_drinking", 1.0f, false);
      //StartCoroutine(flashGreen());
      //StartCoroutine(falconPull());
      return;

    }

    if (other.gameObject.tag.Equals("Player"))
    {
      GameObject.Find("GameManager").GetComponent<GameManager>().decreaseHealth("Feeder");
      damage();
      return;

    }

	if(other.gameObject.tag.Equals ("Feeder") || other.gameObject.tag.Equals ("Killer"))
	{

			Physics.IgnoreCollision(other.collider, GetComponent<Collider>());

		
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
		myStatus.hitText ();
		yield return new WaitForSeconds (0.2f);
		myStatus.updateText (false);
  }

  public override void isInterested()
  {
    StartCoroutine(changeState(State.eating));

    this.interested = true;

    myStatus.updateText(true);

    react();

  }


  protected override void react()
  {

    this.waypoint = this.tempTarget;

  }


  public void notInterested()
  {
    StartCoroutine(changeState(State.roaming));

    //this.agent.speed = this.tempSpeed;

    this.interested = false;

    myStatus.updateText(false);

    this.waypoint = this.prevWaypoint;

  }

  //change this to flashColour(Color tempColour) so you do all the colours!
  IEnumerator flashRed()
  {

    gameObject.GetComponent<Renderer>().material.color = Color.red;

    yield return new WaitForSeconds(0.2f);

    gameObject.GetComponent<Renderer>().material.color = Color.magenta;

  }


  IEnumerator flashGreen()
  {

    gameObject.GetComponent<Renderer>().material.color = Color.green;

    yield return new WaitForSeconds(0.2f);

    gameObject.GetComponent<Renderer>().material.color = Color.magenta;

  }

  IEnumerator falconPull()
  {
    //Debug.Log ("wait for it");
    yield return new WaitForSeconds(1.3f);

    this.tempSpeed = this.agent.speed;
    this.agent.speed = 0f;

    //Debug.Log ("a few more seconds");
    yield return new WaitForSeconds(1.0f);

    //Debug.Log ("ok go");
    notInterested();

  }


  IEnumerator changeState(State newState)
  {

    yield return new WaitForSeconds(1.0f);

    this.currState = newState;

  }

}
