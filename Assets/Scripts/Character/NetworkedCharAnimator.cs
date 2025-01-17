﻿using UnityEngine;
using System.Collections;

public class NetworkedCharAnimator : MonoBehaviour
{
  public float _runThreshold = 0.01f;
  public float _sprintThreshold = 10f;

  Animator _animator;
  RigidbodyNetworkedPlayerController _controller;
  Rigidbody _rb;
  string[] _animationBoolParameters = { "isIdle", "isRunning", "isAttacking_Projectile" };
  int _currentStateIndex = 0;

  // Use this for initialization
  void Start()
  {
    _animator = this.GetComponent<Animator>();
    transition(0);
    _controller = this.GetComponent<RigidbodyNetworkedPlayerController>();
    _rb = this.GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update()
  {
    if (_animator)
    {
      int newStateIndex = _currentStateIndex;
      //float speed = _rb.GetPointVelocity(Vector3.zero).magnitude;
      if (_controller.playerState == PlayerControllerState.WALKING 
			    || (_controller.playerState == PlayerControllerState.ZOOM_OUT && this.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 0.1f))
      {
				Debug.Log("state is walking");
        newStateIndex = 1;
      }
	  else if (_controller.playerState == PlayerControllerState.AIMING)
      {
        newStateIndex = 2;
      }
      else
      {
        newStateIndex = 0;
      }
      if (newStateIndex != _currentStateIndex)
      {
        if (Network.isClient || Network.isServer)
        {
          GetComponent<NetworkView>().RPC("transition", RPCMode.All, newStateIndex);
        }
        else
        {
          transition(newStateIndex);
        }
        _currentStateIndex = newStateIndex;
      }
    }
  }

  [RPC]
  void transition(int parameterIndex)
  {
    _animator.SetBool(_animationBoolParameters[parameterIndex], true);
    for (int i = 0; i < _animationBoolParameters.Length; ++i)
    {
      if (i != parameterIndex)
      {
        _animator.SetBool(_animationBoolParameters[i], false);
      }
    }
  }
}
