﻿//Reference: http://docs.unity3d.com/ScriptReference/Object.Instantiate.html

using UnityEngine;
using System.Collections;
using GamepadInput;

/**
* Spawn a rigid body GameObject with an initial velocity when triggered. 
* Constraints: The projectile must contain a rigid body.
*/
public class ProjectilePower : MonoBehaviour
{
	//Launch properties
	public GameObject _parent;
	public GameObject _projectile;
	public GameObject _otherGun;
	public AudioManager _audioManager;
	public Vector3 _offset;
	public float _magnitude = 50;
	public bool _makeChild = false;
	
	//Rate of fire
	public float _cooldown = 1;
	float _cooldownTimer;
	
	//Controller properties
	public ProjectileTriggerButton _projectileButton = ProjectileTriggerButton.LEFT;
	GamePad.Index _padIndex = GamePad.Index.One;
	float _triggerThreshold = 0.20f;
	DeftPlayerController _controller;

	
	// Use this for initialization
	void Start()
	{
		_cooldownTimer = _cooldown;
		_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<DeftPlayerController>();
		if (_parent)
		{
			this.transform.parent = _parent.transform;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if (this.GetComponent<NetworkView>().isMine || _controller.singlePlayer)
		{
			_cooldownTimer -= Time.deltaTime;
			
			if (_parent)
			{
				this.transform.position = _parent.transform.position;
			}
			
			bool leftTriggerHeld = (GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, _padIndex) > _triggerThreshold);
			bool rightTriggerHeld = (GamePad.GetTrigger(GamePad.Trigger.RightTrigger, _padIndex) > _triggerThreshold);
			if (_cooldownTimer <= 0.0f)
			{
				//----FIRE----//
				if ((leftTriggerHeld && _projectileButton == ProjectileTriggerButton.LEFT)
				    || (rightTriggerHeld && _projectileButton == ProjectileTriggerButton.RIGHT))
				{
						if (Network.isClient || Network.isServer)
						{
							GetComponent<NetworkView>().RPC("LaunchProjectile", RPCMode.All, _offset, _magnitude, _makeChild);
						}
						else
						{
							LaunchProjectile(_offset, _magnitude, _makeChild);
						}
					}
				_cooldownTimer = _cooldown;
			}
		}
	}
	
	[RPC]
	void LaunchProjectile(Vector3 offset, float magnitude, bool makeChild)
	{
		_audioManager.QueueClip("suction", false);
		GameObject clone;
		clone = Instantiate(_projectile, transform.position + offset, transform.rotation) as GameObject;
		//clone.rigidbody.velocity = transform.TransformDirection( trajectory * magnitude );
		
		Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
		forward = forward.normalized;
		clone.GetComponent<Rigidbody>().velocity = (new Vector3(forward.x * magnitude, 0, forward.z * magnitude));
		
		if (makeChild)
		{
			clone.transform.parent = this.transform;
		}
	}
}