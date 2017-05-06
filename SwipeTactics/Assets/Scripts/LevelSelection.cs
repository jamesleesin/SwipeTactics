using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelSelection : MonoBehaviour {
	private float animationSpeed = 0.05f;
	public Sprite[] peasantSprites;
	public Sprite[] archerSprites;
	public Sprite[] knightSprites;
	public Sprite[] ninjaSprites;
	public Sprite[] mageSprites;
	
	private bool animating = false;
	
	// selected level and character
	private int selectedLevel = 1;
	private int selectedCharacter = 0;
	
	private int unlockedUpToLevel;
	private int unlockedUpToCharacter;
	public Sprite unlockedImage;
	public Sprite unlockedBossImage;
	
	// Use this for initialization
	void Start () {
		// find out what levels are unlocked for the user
		unlockedUpToLevel = PlayerPrefs.GetInt("UnlockedUpToLevel");
		unlockedUpToCharacter = PlayerPrefs.GetInt("UnlockedCharactersUpTo");
		
		// if unlocked up to 16 then beat the game
		if (unlockedUpToLevel == 16){
			GameObject.Find("AllClear").transform.GetComponent<Image>().enabled = true;
		}
		
		// and for each unlocked level switch the image and add text
		for (int i = 1; i <= unlockedUpToLevel; i++){
			if (i < 16){
				string levelName = "Level " + i;
				if (i < 15)
					GameObject.Find(levelName).transform.GetComponent<Image>().sprite = unlockedImage;
				else if (i == 15)
					GameObject.Find(levelName).transform.GetComponent<Image>().sprite = unlockedBossImage;
				
				GameObject.Find(levelName).transform.GetChild(0).GetComponent<Text>().text = ""+i;
				GameObject.Find(levelName).transform.GetComponent<LevelButton>().UnlockMe();
			}
		}
		
		// for each unlocked character display it
		if (unlockedUpToCharacter < 4){
			GameObject.Find("Mage").SetActive(false);
		}
		if (unlockedUpToCharacter < 3){
			GameObject.Find("Ninja").SetActive(false);
			
		}
		if (unlockedUpToCharacter < 2){
			GameObject.Find("Knight").SetActive(false);
		}
		if (unlockedUpToCharacter < 1){
			GameObject.Find("Archer").SetActive(false);
		}
		
		// set selected level to the newest unlocked one
		int currentSelectedLevel = (unlockedUpToLevel == 16) ? 15 : unlockedUpToLevel; 
		if (currentSelectedLevel == 0){
			currentSelectedLevel = 1;
		}
		UpdateSelectedLevel(currentSelectedLevel);
	}
	
	public IEnumerator animateCharacters()
	{
		animating = true;
		for (int i=0; i < 15; i++)
		{
			if (i < peasantSprites.Length)
				GameObject.Find("Peasant").transform.GetComponent<Image>().sprite = peasantSprites[i];
			if (unlockedUpToCharacter >= 1 && i < archerSprites.Length)
				GameObject.Find("Archer").transform.GetComponent<Image>().sprite = archerSprites[i];
			if (unlockedUpToCharacter >= 2 && i < knightSprites.Length)
				GameObject.Find("Knight").transform.GetComponent<Image>().sprite = knightSprites[i];
			if (unlockedUpToCharacter >= 3 && i < ninjaSprites.Length)
				GameObject.Find("Ninja").transform.GetComponent<Image>().sprite = ninjaSprites[i];
			if (unlockedUpToCharacter >= 4 && i < mageSprites.Length)
				GameObject.Find("Mage").transform.GetComponent<Image>().sprite = mageSprites[i];
			yield return new WaitForSeconds(animationSpeed);
		}
		animating = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (animating == false){
			StartCoroutine(animateCharacters());
		}
	}

    public void UpdateSelectedLevel(int level)
    {
		string levelName = "Level " +  level;
		Vector3 levelButtonPos = GameObject.Find(levelName).transform.position;
		GameObject.Find("LevelHighlight").transform.position = levelButtonPos;
		selectedLevel = level;
    }
	
	public void UpdateSelectedChar(int charac)
    {
		string characterName;
		if (charac == 0){
			characterName = "Peasant";
		}
		else if (charac == 1){
			characterName = "Archer";
		}
		else if (charac == 2){
			characterName = "Knight";
		}
		else if (charac == 3){
			characterName = "Ninja";
		}
		else{
			characterName = "Mage";
		}
		Vector3 characterPos = GameObject.Find(characterName).transform.position;
		GameObject.Find("CharacterHighlight").transform.position = characterPos;
		selectedCharacter = charac;
    }
	
	// start the game
	public void StartGame(){
		GameObject.Find("Info").GetComponent<Info>().SetInfo(selectedLevel, selectedCharacter);
		SceneManager.LoadScene("Game", LoadSceneMode.Single);
	}
	
}
