using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CannonController : NetworkBehaviour
{
	private float power;
	private Transform playerCamera;

	// Use this for initialization
	//初始化，找到名為FirstPersonCharacter的物件
	void Start ()
	{
		power = 800.0f;
		playerCamera = transform.Find("FirstPersonCharacter");
	}
	
	// Update is called once per frame
	//如果是本地玩家，才能觸發射擊
	void Update ()
	{
		if(isLocalPlayer)
		{
			if(Input.GetButtonDown("Fire1"))
			{
				CmdSpawnCannonball();
			}
		}
	}

	//告訴Server射擊，這樣球才能同步到所有的Client
	[Command]
	void CmdSpawnCannonball ()
	{
		//we instantiate one from Resources
		GameObject instance = Instantiate (Resources.Load ("Ball")) as GameObject;
		//Let's name it
		instance.name = "Cannonball";
		//Let's position it at the player
		instance.transform.position = playerCamera.position + playerCamera.forward * 1.5f + playerCamera.up * -.5f;
		instance.GetComponent<Rigidbody> ().AddForce (playerCamera.forward * power);
		//
		NetworkServer.Spawn (instance);
	}
}
