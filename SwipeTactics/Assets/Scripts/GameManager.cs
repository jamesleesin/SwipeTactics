using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	// swipe variables
	private Vector3 touchPosition;
	private float swipeResistance = 10.0f;
	
	public Player[] playerPrefabs;
	public Bomb bombPrefab;
	
	/* Attack prefabs */
	public Attack[] attackPrefabs;
	
	private Player userPlayer;
	private Player AIPlayer;
	private Bomb bomb;
	
	private Animator userAnimator;
	private Animator AIAnimator;
	private Animator bombAnimator;
	
	private bool showOutcome = false;
	private float lastTime;
	
	private int AICharacterChoice;
	private int playerCharacterChoice;
	private int AIDifficulty = 5;
	private int currentLevel;
	
	public Sprite[] backgrounds;
	
	private bool showTooltip = false;
	public Tooltip tooltipPrefab;
	
	// stump, cactus, pillar
	public Obstacle[] obstaclePrefabs;
	
	private Info info;
	
	// camera
	Camera mainCamera;
	
	// autofire setting
	private bool isAutofireOn = false;
	private Vector2 lastSwipe;
	private float lastAngle = 0f;
	
	// levels (level 0 doesnt exist so its just 0, 0, 0)
	private int[,] levelSetup = new int[,]{{0, 0, 0}, 
		{0, 1, 0}, {0, 3, 0}, {1, 3, 0}, 
		{1, 4, 1}, {0, 5, 1}, {1, 5, 1},
		{2, 3, 0}, {2, 5, 1}, {3, 3, 2},
		{3, 5, 2}, {2, 5, 2}, {3, 5, 3},
		{4, 4, 3}, {4, 5, 3}, {5, 5, 3}
		};
	
	// awake, then start
	private void Awake(){
		Screen.SetResolution(800, 1280, true);
		
		info = GameObject.Find("Info").GetComponent<Info>();
		
		// load map and AI based on selected level
		currentLevel = info.getSelectedLevel();
		AICharacterChoice = levelSetup[currentLevel, 0];
		AIDifficulty = levelSetup[currentLevel, 1];
		GameObject.Find("Background").transform.GetComponent<SpriteRenderer>().sprite = backgrounds[levelSetup[currentLevel, 2]];
		
		// different music for each background
		if (currentLevel == 15){
			info.changeToMusicClip(5);
		}
		else if (levelSetup[currentLevel, 2] == 0){
			info.changeToMusicClip(1);
		}
		else if(levelSetup[currentLevel, 2] == 1){
			info.changeToMusicClip(2);
		}
		else if(levelSetup[currentLevel, 2] == 2){
			info.changeToMusicClip(3);
		}
		else if(levelSetup[currentLevel, 2] == 3){
			info.changeToMusicClip(4);
		}
		
		// obstacles for certain levels
		Obstacle newStump;
		Obstacle newCactus;
		Obstacle newPillar;
		if (currentLevel == 3){
			newStump = Instantiate(obstaclePrefabs[0]);
			newStump.transform.position = new Vector2(-3f, 0f);
		}
		else if (currentLevel == 6){
			newStump = Instantiate(obstaclePrefabs[0]);
			newStump.transform.position = new Vector2(3f, 0f);
		}
		else if (currentLevel == 7){
			newStump = Instantiate(obstaclePrefabs[0]);
			newStump.transform.position = new Vector2(-3f, 3f);
			newStump = Instantiate(obstaclePrefabs[0]);
			newStump.transform.position = new Vector2(3f, -3f);
		}
		else if (currentLevel == 9){
			newCactus = Instantiate(obstaclePrefabs[1]);
			newCactus.transform.position = new Vector2(-3f, 3f);
			newCactus = Instantiate(obstaclePrefabs[1]);
			newCactus.transform.position = new Vector2(-3f, -3f);
		}
		else if (currentLevel == 11){
			newCactus = Instantiate(obstaclePrefabs[1]);
			newCactus.transform.position = new Vector2(1f, -4f);
		}
		else if (currentLevel == 14 || currentLevel == 15){
			for (float i = -3; i <= 3; i+=6){
				for (float j = -4; j <= 4; j+=8){
					newPillar = Instantiate(obstaclePrefabs[2]);
					newPillar.transform.position = new Vector2(i, j);
				}
			}
		}
		
		// players
		playerCharacterChoice = info.getSelectedCharacter();
		
		// camera
		mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}
	
	private void Start(){
		userPlayer = Instantiate(playerPrefabs[playerCharacterChoice]);
		userPlayer.Initialize(0, playerCharacterChoice);
		AIPlayer = Instantiate(playerPrefabs[AICharacterChoice]);
		AIPlayer.Initialize(1, AICharacterChoice);
		// the bomb
		bomb = Instantiate(bombPrefab);
		
		userAnimator = userPlayer.transform.GetComponent<Animator>();
		AIAnimator = AIPlayer.transform.GetComponent<Animator>();
		bombAnimator = bomb.transform.GetComponent<Animator>();
		
		// show tooltip for level 1
		if (currentLevel == 1){
			showTooltip = true;
			Instantiate(tooltipPrefab).transform.GetComponent<Animator>();
		}
		else{
			GameObject.Find("OKButton").transform.gameObject.SetActive(false);
		}
		
		// update volume
		ChangeSFXSoundVolume(info.getVolumeSFXLevel());
		
		// start timed actions
		StartCoroutine(TimedActions());
		
		// set autofire
		ChangeAutofireSetting(info.GetAutofireSetting());
	}

	public void BombHit(){
		StartCoroutine(BombExplode());
	}
	
	IEnumerator BombExplode(){
		bombAnimator.Play("Explode");
		bomb.transform.localScale = new Vector2(2f, 2f);
		bomb.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
		bomb.transform.GetComponent<Rigidbody2D>().mass = 100f;
		bomb.PlayExplodeSound();
		yield return new WaitForSeconds(1.0f);
		bomb.transform.GetComponent<SpriteRenderer>().enabled = false;
		bomb.transform.GetComponent<Collider2D>().enabled = false;
		yield return new WaitForSeconds(2f);
		Destroy(bomb.gameObject);
		bomb = Instantiate(bombPrefab);
		bombAnimator = bomb.transform.GetComponent<Animator>();
	}
	
	private void AIActions(){
		if (bomb){
			// shoot at the bomb, the higher difficulty the more "smart" they are about their attacks
			float attackChance;
			// higher chance of attacking if the bomb is near the AI
			if (Vector3.Distance(bomb.transform.position, AIPlayer.transform.position) <= 2f){
				attackChance = Random.Range(0f, 10f);
			}else if (Vector3.Distance(bomb.transform.position, AIPlayer.transform.position) <= 3.5f){
				attackChance = Random.Range(0f, 20f);
			}
			else{
				attackChance = Random.Range(0f, 35f);
			}
			if (attackChance < 2f * Mathf.Pow(AIDifficulty, 1f/3f)){
				if (AIPlayer.canAttack()){
					Vector2 AIPos = new Vector2(AIPlayer.transform.position.x, AIPlayer.transform.position.y);
					Vector2 bombPos = new Vector2(bomb.transform.position.x, bomb.transform.position.y);
					
					// errors (lower difficulty of AI)
					Vector2 swipeError = Random.Range(-1.5f, 1.5f)/(Mathf.Pow(AIDifficulty, 2f)) * Vector2.left;
					float travelTimeError = Random.Range(-0.3f, 0.3f)/(Mathf.Pow(AIDifficulty, 2f));
					
					// calculate how much to lead the target by
					float characterSpeedOfAttack = AIPlayer.getSpeedOfAttack();
					float distToTarget = Vector2.Distance(AIPos, bombPos);
					float travelTimeOfAttack = distToTarget/characterSpeedOfAttack;
					Vector2 leadTarget = bomb.transform.GetComponent<Rigidbody2D>().velocity * (travelTimeOfAttack + travelTimeError);
					
					// but they can somtimes lead the target to incorrectly, e.g. if the bomb would hit obstacles or the edges of the screen. We need to calculate and subtract the amount that they bounce off by. 
					// first we check to see if there is a collider between the bomb and the supposed end point
					float vecDist = leadTarget.magnitude;
					RaycastHit2D[] obstacleCheck = Physics2D.CircleCastAll(bombPos, 0.625f, leadTarget, vecDist);
					Vector2 newLeadTarget = leadTarget;
					//Debug.Log(leadTarget);
					float distFromBombToObstacle = 0;
					foreach (RaycastHit2D col in obstacleCheck){
						if (col.collider.tag == "Obstacle"){
							/*
							// if there is an obstacle in the way, calculate distance from bomb to obstacle
							distFromBombToObstacle = col.distance;
							// the bomb will bounce off the obstacle, subtract along same vector (ignore angle)
							float bounceBackDist = vecDist - distFromBombToObstacle;
							newLeadTarget = bombPos - (leadTarget * (bounceBackDist - distFromBombToObstacle));	
							*/
							newLeadTarget = (bombPos + col.point)/2f;
						}
					}
					
					Vector2 simulatedSwipe = AIPos - (bombPos + newLeadTarget) + swipeError;
					
					//Debug.DrawLine(AIPlayer.transform.position, bomb.transform.position + leadTarget, Color.white, 1f);
					//Debug.DrawLine(AIPos, AIPos-simulatedSwipe*2f, Color.white, 1f);
					StartCoroutine(FireAttack(simulatedSwipe, AIPos, AIPlayer.whichCharacter, 1));
					
					// play animation
					float angle;
					if (simulatedSwipe.x > 0f){
						angle = Vector2.Angle(Vector2.down, simulatedSwipe);
					}
					else{
						angle = -1f * Vector2.Angle(Vector2.down, simulatedSwipe);	
					}
					AIPlayer.transform.rotation = Quaternion.Euler(0, 0, angle);
					AIAnimator.Play("Attack");
					AIPlayer.attacked();
				}
			}
		}
	}
	
	IEnumerator PlaySoundAfterDelay(Player player, float delay){
		yield return new WaitForSeconds(delay);
		player.PlayAttackSound();
	}
	
	IEnumerator FireAttack(Vector2 dir, Vector2 start, int whichCharacter, int playerType){
		Player player = (playerType == 0) ? userPlayer : AIPlayer;
		Vector2 normal;
		
		switch (whichCharacter){
			case 0:
				StartCoroutine(PlaySoundAfterDelay(player, 0f));
				yield return new WaitForSeconds(0.15f);
				// single shots, 
				Attack newAttack = Instantiate(attackPrefabs[0]);
				newAttack.Initialize(dir, start, player.getAttackForce(), player.getAttackMass(), playerType);
				break;
			case 1:
				StartCoroutine(PlaySoundAfterDelay(player, 0.5f));
				yield return new WaitForSeconds(0.5f);
				// triangle shots
				normal = new Vector2(-dir.y, dir.x) / Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
				for (int i = -1; i < 2; i++){
					Attack lineAttack = Instantiate(attackPrefabs[1]);
					Vector2 vec2Pos = new Vector2(start.x, start.y) + (i/3f * normal.normalized) + (Mathf.Abs(i)/4f * dir.normalized);
					lineAttack.Initialize(dir, vec2Pos, player.getAttackForce(), player.getAttackMass(), playerType);
				}
				break;
			case 2:
				StartCoroutine(PlaySoundAfterDelay(player, 0.4f));
				yield return new WaitForSeconds(0.25f);
				// 2 shots in parallel
				normal = new Vector2(-dir.y, dir.x) / Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
				for (int i = -1; i < 2; i+=2){
					Attack lineAttack = Instantiate(attackPrefabs[2]);
					Vector2 vec2Pos = new Vector2(start.x, start.y) + (i/2f * normal.normalized);
					lineAttack.Initialize(dir, vec2Pos, player.getAttackForce(), player.getAttackMass(), playerType);
				}
				break;
			case 3:
				StartCoroutine(PlaySoundAfterDelay(player, 0.1f));
				yield return new WaitForSeconds(0.2f);
				// parallel
				// compute normal 
				normal = new Vector2(-dir.y, dir.x) / Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
				for (int i = -1; i < 2; i++){
					Attack lineAttack = Instantiate(attackPrefabs[3]);
					Vector2 vec2Pos = new Vector2(start.x, start.y) + (i/3f * normal.normalized);
					lineAttack.Initialize(dir, vec2Pos, player.getAttackForce(), player.getAttackMass(), playerType);
				}
				break;
			case 4:
				StartCoroutine(PlaySoundAfterDelay(player, 0.35f));
				yield return new WaitForSeconds(0.6f);
				// 4 shots in a line
				normal = new Vector2(-dir.y, dir.x) / Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
				for (int i = 0; i < 4; i++){
					Attack lineAttack = Instantiate(attackPrefabs[4]);
					Vector2 vec2Pos = new Vector2(start.x, start.y) + normal.normalized/3f;
					lineAttack.Initialize(dir, vec2Pos - (dir.normalized*(1.5f*i/4f)), userPlayer.getAttackForce(), userPlayer.getAttackMass(), playerType);
				}
				break;
			case 5:
				StartCoroutine(PlaySoundAfterDelay(player, 0.5f));
				yield return new WaitForSeconds(0.5f);
				// 5 shots in a ^ formation
				normal = new Vector2(-dir.y, dir.x) / Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
				for (int i = -2; i < 3; i++){
					Attack lineAttack = Instantiate(attackPrefabs[5]);
					Vector2 vec2Pos = new Vector2(start.x, start.y) + (i/3f * normal.normalized) + (Mathf.Abs(i)/4f * dir.normalized);
					lineAttack.Initialize(dir, vec2Pos, player.getAttackForce(), player.getAttackMass(), playerType);
				}
				break;
		}
		
	}
	
	public void StartOutcome(bool win){
		GameObject.Find("Outcome").GetComponent<Text>().enabled = true;
		if (win){
			GameObject.Find("Outcome").GetComponent<Text>().text = "VICTORY!";
			PlayerPrefs.SetInt("UnlockedUpToLevel", currentLevel+1);
			if (currentLevel == 3){
				PlayerPrefs.SetInt("UnlockedCharactersUpTo", 1);
			}
			else if (currentLevel == 7){
				PlayerPrefs.SetInt("UnlockedCharactersUpTo", 2);
			}
			else if (currentLevel == 9){
				PlayerPrefs.SetInt("UnlockedCharactersUpTo", 3);
			}
			else if (currentLevel == 13){
				PlayerPrefs.SetInt("UnlockedCharactersUpTo", 4);
			}
			
		}
		else{
			GameObject.Find("Outcome").GetComponent<Text>().text = "DEFEAT...";
		}
		PlayerPrefs.Save();
		showOutcome = true;
	}
	
	// timed AI actions and bomb damage
	private IEnumerator TimedActions(){
		while (!showTooltip && !showOutcome){
			// run this 10 times per second
			yield return new WaitForSeconds(0.1f);
			
			AIActions();
				
			// bomb deals DPS based on whos side it's on
			if (bomb){
				if (Vector3.Distance(bomb.transform.position, userPlayer.transform.position) <= 3f){
					userPlayer.transform.GetComponent<Player>().TakeDamage(2f);
				}
				else if (Vector3.Distance(bomb.transform.position, AIPlayer.transform.position) <= 3f){
					AIPlayer.transform.GetComponent<Player>().TakeDamage(2f);
				}
			}
		}
	}
	
	void Update() 
	{	
		if (!showTooltip){
			if (showOutcome){
				if (Time.time - lastTime > 0.03f){
					lastTime = Time.time;
					if (GameObject.Find("Outcome").GetComponent<Text>().fontSize < 100)
						GameObject.Find("Outcome").GetComponent<Text>().fontSize ++;
					else{
						GameObject.Find("Outcome").GetComponent<Text>().fontSize = 1;
						GameObject.Find("Outcome").GetComponent<Text>().enabled = false;
						SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
					}
					
					if (GameObject.Find("FadeOut").GetComponent<Image>().color.a < 1){
						float alpha = GameObject.Find("FadeOut").GetComponent<Image>().color.a + 0.03f;
						Color newCol = GameObject.Find("FadeOut").GetComponent<Image>().color;
						newCol.a = alpha;
						GameObject.Find("FadeOut").GetComponent<Image>().color = newCol;
					}
				}
			}
			else{
				if (!isAutofireOn){
					if (userPlayer.canAttack()){
						// OLD SWIPE: direction from touch to release
						// NEW SWIPE: is from user to release position
						if (Input.GetMouseButtonDown(0)){
							touchPosition = mainCamera.WorldToScreenPoint(userPlayer.transform.position);
							//touchPosition = Input.mousePosition;
						}
						if (Input.GetMouseButtonUp(0)){
							Vector2 deltaSwipe = touchPosition - Input.mousePosition;		
							if (deltaSwipe.magnitude > swipeResistance){
								// play animation
								float angle;
								if (deltaSwipe.x > 0f){
									angle = Vector2.Angle(Vector2.down, deltaSwipe);
								}
								else{
									angle = -1f * Vector2.Angle(Vector2.down, deltaSwipe);	
								}
								userPlayer.transform.rotation = Quaternion.Euler(0, 0, angle);
								userAnimator.Play("Attack");
								
								StartCoroutine(FireAttack(deltaSwipe, userPlayer.transform.position, userPlayer.whichCharacter, 0));
								userPlayer.attacked();
							}
						}
					}
				}
				else {
					// just keep track of the latest swipe, and call fire attack every update
					if (Input.GetMouseButtonDown(0)){
						touchPosition = mainCamera.WorldToScreenPoint(userPlayer.transform.position);
						//touchPosition = Input.mousePosition;
					}
					if (Input.GetMouseButtonUp(0)){
						Vector2 deltaSwipe = touchPosition - Input.mousePosition;		
						if (deltaSwipe.magnitude > swipeResistance){
							// play animation
							float angle;
							if (deltaSwipe.x > 0f){
								angle = Vector2.Angle(Vector2.down, deltaSwipe);
							}
							else{
								angle = -1f * Vector2.Angle(Vector2.down, deltaSwipe);	
							}
							lastAngle = angle;
							lastSwipe = deltaSwipe;
						}
					}
					if (userPlayer.canAttack() && lastSwipe != new Vector2(0f, 0f)){
						userPlayer.transform.rotation = Quaternion.Euler(0, 0, lastAngle);
						userAnimator.Play("Attack");
						StartCoroutine(FireAttack(lastSwipe, userPlayer.transform.position, userPlayer.whichCharacter, 0));
						userPlayer.attacked();
					}
				}
			}
		}
	}
	
	public bool isGameOver(){
		return showOutcome;
	}
	
	public void closeTooltip(){
		showTooltip = false;
		StartCoroutine(TimedActions());
	}
	
	public void ChangeSFXSoundVolume(float vol){
		userPlayer.ChangeSFXVolume(vol);
		AIPlayer.ChangeSFXVolume(vol);
		if (bomb)
			bomb.ChangeSFXVolume(vol);
	}
	
	public void ChangeAutofireSetting(bool newValue){
		isAutofireOn = newValue;
	}
}
