﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public enum PlayerCharacter { Syphen, Blitz };

public class GameManager : MonoBehaviour
{
  public GameObject DeftClientServer;
  public int playerCurrentHealth;
  public int playerTotalHealth;
  public int numbersOfDepots;
  public int[] depotCapacity;
  public int[] depotCurrentStock;
  public int[] depotResourceValue;
  public GameObject[] depots;
  public int killerAttackDamage;
  public int feederAttackDamage;
  public GameObject[] depotUI_objects;
  public GameObject eventSystemObject;
  public GameObject gameOverFirstSelected;
  public GameObject gameOverWindow;
  public GameObject winText;
  public GameObject loseText;
  public GameObject SyphenPowerUnlock;
  public GameObject BlitzPowerUnlock;
  public GameObject NewObjectiveWindow;
  public bool longRangeUnlocked;
  
  private string playerName;
  private bool secondTutorial;
  private int depotsFull;
  private EventSystem eventSystem;

  void Start()
  {
    longRangeUnlocked = false;
    gameOverWindow.SetActive(false);
    SyphenPowerUnlock.SetActive(false);
    BlitzPowerUnlock.SetActive(false);
    NewObjectiveWindow.SetActive(false);
    winText.SetActive(false);
    loseText.SetActive(false);
    eventSystem = eventSystemObject.GetComponent<EventSystem>();
    depotsFull = 0;
    //Disable all depots except the first one
    foreach (GameObject d in depots)
    {
      d.SetActive(false);
    }
    foreach (GameObject d in depotUI_objects)
    {
      d.SetActive(false);
    }
    depots[0].SetActive(true);
    depotUI_objects[0].SetActive(true);
  }

  [RPC]
  public void StartHealthBar(string name)
  {
	playerName = name;
    GameObject hb = GameObject.Find("HealthBar");
    GameObject hs = GameObject.Find("HealthStats");
    if (name.Contains("Syphen"))
    {
      hb.GetComponent<HealthBar>().StartHealthBar(0);
      hs.GetComponent<HealthStats>().StartStats(0);
    }
    else
    {
      hb.GetComponent<HealthBar>().StartHealthBar(1);
      hs.GetComponent<HealthStats>().StartStats(1);
    }
  }

  [RPC]
  public void StartGrenadeBar()
  {
    GameObject.Find("GrenadeBar").GetComponent<GrenadeBar>().StartGrenadeUI();
  }

  [RPC]
  public void decreaseHealth(string attackerName)
  {
    int damage;
    if (attackerName.Contains("Feeder"))
    {
      damage = feederAttackDamage;
    }
    else
    {
      damage = killerAttackDamage;
    }

    playerCurrentHealth -= damage;

    if (playerCurrentHealth <= 0)
    {
      gameOver();
    }
  }

  [RPC]
  public void increaseResourceCount(int depotNumber)
  {
    if (depotCurrentStock[depotNumber] < depotCapacity[depotNumber])
    {
      depotCurrentStock[depotNumber] += depotResourceValue[depotNumber];
      if (depotCurrentStock[depotNumber] >= depotCapacity[depotNumber])
      {
        depotFull();
        Debug.Log("called DepotFull()");
      }
    }
  }

  [RPC]
  public void depotFull()
  {
    Debug.Log("A Depot is FULL");
    depotsFull++;
    if (depotsFull == numbersOfDepots)
    {
      lastDepotFull();
    }
    else
    {
      //Activate second power
      longRangeUnlocked = true;
      //Activate the Power Unlock window 
//      string name = DeftClientServer.GetComponent<PlayerSelect>().selectedPlayer.name;
//      if (playerName.Contains("Blitz"))
//      {
//        Debug.Log("Activating tutorial for blitz");
//        BlitzPowerUnlock.SetActive(true);
//      }
//      else
//      {
//        SyphenPowerUnlock.SetActive(true);
//        Debug.Log("Activating tutorial for syphen");
//        try
//        {
//          GameObject button = SyphenPowerUnlock.transform.FindChild("Button").gameObject;
//          GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(button);
//        }
//        catch (System.NullReferenceException e)
//        {
//        }
//      }
    }
  }
  void Update() {
	//Check if first depot is full, if so display tutorial
		if (!secondTutorial && depotCurrentStock[0] >= depotCapacity[0]) {
			secondTutorial = true;
			if (playerName.Contains("Blitz")) {
        		BlitzPowerUnlock.SetActive(true);
			} else {
				SyphenPowerUnlock.SetActive(true);
				try
				{
					GameObject button = SyphenPowerUnlock.transform.FindChild("Button").gameObject;
					GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(button);
				}
				catch (System.NullReferenceException e)
				{
				}
			}
		}
	}
	
	[RPC]
	public void activateNewObjective()
  {
    NewObjectiveWindow.SetActive(true);
    GameObject.Find("Layer10SyncManager").GetComponent<DeftLayerSyncManager>().SetLastSavedState();
    activateNextDepot();
  }

  [RPC]
  public void activateNextDepot()
  {
    //Activate next depot
    depots[depotsFull].SetActive(true);
    depotUI_objects[depotsFull].SetActive(true);
  }

  [RPC]
  public void lastDepotFull()
  {
    Debug.Log("YOU WIN.");
    gameOverWindow.SetActive(true);
    winText.SetActive(true);
    eventSystem.SetSelectedGameObject(gameOverFirstSelected);
  }

  [RPC]
  public void gameOver()
  {
    Debug.Log("YOU DIED.");
    gameOverWindow.SetActive(true);
    loseText.SetActive(true);
    eventSystem.SetSelectedGameObject(gameOverFirstSelected);
  }

}
