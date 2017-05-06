using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
	public AudioClip explodeSound;
	public AudioSource soundPlayer;
	
	// Use this for initialization
	void Start () {
		transform.position = new Vector2(0f, 0f);
		soundPlayer = GetComponent<AudioSource>();
	}
	
	void OnTriggerEnter2D(Collider2D obj){
		if(obj.gameObject.tag == "Player"){
			obj.transform.GetComponent<Player>().TakeDamage(200f);
			// send a message to the game manager saying that the bomb has been destroyed
			GameObject.Find("GameManager").GetComponent<GameManager>().BombHit();
		}
	}
	
	public void PlayExplodeSound(){
		soundPlayer.pitch = Random.Range(0.95f, 1.05f);
		soundPlayer.PlayOneShot(explodeSound, 3.0f);
	}
	
	void OnCollisionEnter2D(Collision2D collisionInfo){
		if (collisionInfo.gameObject.tag == "UserAttack" || collisionInfo.gameObject.tag == "AIAttack"){
			Destroy(collisionInfo.gameObject);
		}
	}
	
	public void ChangeSFXVolume(float vol){
		soundPlayer.volume = vol;
	}
}
