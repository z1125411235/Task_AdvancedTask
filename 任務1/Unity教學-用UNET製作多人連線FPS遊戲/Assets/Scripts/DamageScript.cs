using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DamageScript : NetworkBehaviour {

	AudioSource damageAudio;

	void Awake()
	{
		//Set current lives and get audio component reference
		damageAudio = GetComponent<AudioSource>();
	}

	//如果被球打中
	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Ball") {
			
			//damageAudio.Stop ();
			CmdHitPlayer(this.transform.gameObject);
			//damageAudio.Play ();
		}
		
	}

	//通知伺服器我被打中了
	[Command]
	void CmdHitPlayer(GameObject hit)
	{
		hit.GetComponent<DamageScript>().RpcResolveHit();
	}

	//通知所有的玩家這個玩家被打中了
	[ClientRpc]
	public void RpcResolveHit()
	{
		if (isLocalPlayer)
		{
			Transform spawn = NetworkManager.singleton.GetStartPosition();
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;
			damageAudio.Play ();

		}
	}
}
