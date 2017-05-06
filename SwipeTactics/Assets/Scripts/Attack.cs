using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
	//private string myTag;
	private string otherTag;
	private float liveTime;

	public void Initialize(Vector2 dir, Vector2 startPos, float atkForce, float atkMass, int playerType){
		transform.position = startPos;
		transform.rotation = Quaternion.FromToRotation(Vector2.down, dir);
		transform.GetComponent<Rigidbody2D>().mass = atkMass;
		
		// initial force
		// velocity in unity is v = (F/m)*t, where t is usually 0.02s
		transform.GetComponent<Rigidbody2D>().AddForce(dir.normalized * -atkForce);
		
		// live time: slower attacks live longer
		float vel = (atkForce/atkMass)*0.02f;
		liveTime = 35f/vel;
		
		if (playerType == 0){
			// is a player, else is an AI
			transform.tag = "UserAttack";
			//myTag = "UserAttack";
			otherTag = "AIAttack";
			// layer 8 for user stuff
			transform.gameObject.layer = 8;
		}
		else{
			transform.tag = "AIAttack";
			//myTag = "AIAttack";
			otherTag = "UserAttack";
			// layer 9 for ai stuff
			transform.gameObject.layer = 9;
		}
		
	}
	
	// changed so attacks dont deal damage to players
	/*
	void OnTriggerEnter2D(Collider2D obj){
		if (obj.gameObject.tag == "Player"){
			Player player = obj.transform.GetComponent<Player>();
			if ((myTag == "UserAttack" && player.isPlayerAI()) || (myTag == "AIAttack" && !player.isPlayerAI())){
				player.TakeDamage(transform.GetComponent<Rigidbody2D>().mass * 20f);
				Destroy(this.gameObject);
			}
		}
	}*/
	
	
	void OnCollisionEnter2D (Collision2D collisionInfo) {
		if (collisionInfo.gameObject.tag == otherTag) {
			Destroy(collisionInfo.gameObject);
			Destroy(this.gameObject);
		}
	}
	
	void Update(){
		liveTime -= Time.deltaTime;
		if (liveTime <= 0f){
			Destroy(this.gameObject);
		}
	}
}