using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	private float HP = 395f;
	private GameObject healthDisplayScale;
	
	private float attackCooldownMax;
	private float currentAttackCooldown = 0f;
	private float attackForce;
	private float attackMass;
	
	public int whichCharacter;
	public string characterType = "";
	private bool isAI;
	
	public AudioClip[] attackSounds;
	public AudioSource soundPlayer;
	
	public void Initialize(int type, int which){
		soundPlayer = GetComponent<AudioSource>();
		
		// user type
		if (type == 0){
			transform.position = new Vector2(0f, -7f);
			healthDisplayScale = GameObject.Find("PlayerHealthCurrent");
			transform.gameObject.layer = 8;
			isAI = false;
		}
		// ai type
		else if (type == 1){
			transform.position = new Vector2(0f, 7f);
			healthDisplayScale = GameObject.Find("AIHealthCurrent");
			transform.GetComponent<SpriteRenderer>().flipY = true;
			transform.gameObject.layer = 9;
			isAI = true;
			
			// AI max hp based on level
			Info info = GameObject.Find("Info").GetComponent<Info>();
			HP = 170 + (15*info.getSelectedLevel());
		}
		
		whichCharacter = which;
		switch(which){
			case 0:
				// single shots: velocity of 3.5
				attackCooldownMax = 0.6f;
				attackForce = 43.75f;
				attackMass = 0.25f;
				characterType = "Peasant";
				break;
			case 1:
				// triangle shots, weak power, velocity of 5
				attackCooldownMax = 0.75f;
				attackForce = 20f;
				attackMass = 0.08f;
				characterType = "Archer";
				break;
			case 2:
				// 2 shots in parallel, velocity of 3.5
				attackCooldownMax = 0.7f;
				attackForce = 29.75f;
				attackMass = 0.17f;
				characterType = "Knight";
				break;
			case 3:
				// 3 shots in parallel, velocity of 5
				attackCooldownMax = 0.8f;
				attackForce = 25f;
				attackMass = 0.10f;
				characterType = "Ninja";
				break;
			case 4:
				// 4 shots in a line, velocity of 4
				attackCooldownMax = 0.9f;
				attackForce = 20f;
				attackMass = 0.10f;
				characterType = "Mage";
				break;
			case 5:
				// 5 shots in a ^ formation, velocity of 4
				attackCooldownMax = 1.2f;
				attackForce = 23f;
				attackMass = 0.12f;
				characterType = "King";
				break;
		}
	}
	
	// take damage, if hp <= 0 then death, but only if the game is not yet over
	public void TakeDamage(float damage){
		HP -= damage;
		if (HP <= 0f){
			if (!GameObject.Find("GameManager").transform.GetComponent<GameManager>().isGameOver()){
				//death
				HP = 0f;
				GameObject.Find("GameManager").GetComponent<GameManager>().StartOutcome(isAI);
			}
		}
	}
	
	public void PlayAttackSound(){
		soundPlayer.pitch = Random.Range(0.95f, 1.05f);
		soundPlayer.PlayOneShot(attackSounds[whichCharacter], 0.7f);
	}
	
	public bool canAttack(){
		if (currentAttackCooldown <= 0f){
			return true;
		}
		return false;
	}
	
	public void attacked(){
		currentAttackCooldown = attackCooldownMax;
		// flip it back after first attack
		if (transform.GetComponent<SpriteRenderer>().flipY){
			transform.GetComponent<SpriteRenderer>().flipY = false;
		}
	}
	
	void Update() 
	{
		healthDisplayScale.transform.localScale = new Vector2((2f*HP)/800f, 1f);
		if (currentAttackCooldown > 0){
			currentAttackCooldown -= Time.deltaTime;
		}
	}
	
	// return functions
	public float getAttackForce(){
		return attackForce;
	}
	
	public float getAttackMass(){
		return attackMass;
	}
	
	public bool isPlayerAI(){
		return isAI;
	}
	
	// force of 25 and mass of 0.2 results in speed of 5
	public float getSpeedOfAttack(){
		return attackForce*attackMass;
	}
	
	public void ChangeSFXVolume(float vol){
		soundPlayer.volume = vol;
	}
}
