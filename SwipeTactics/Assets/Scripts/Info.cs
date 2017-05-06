using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Stores the information, but also handles music since it doesnt not get destroyed */
public class Info : MonoBehaviour {
	
	private int selectedLevel;
	private int selectedCharacter;
	
	private float volumeMusicLevel;
	private float volumeSFXLevel;
	
	public AudioClip menuMusic;
	public AudioClip combatGrass;
	public AudioClip combatDirt;
	public AudioClip combatDesert;
	public AudioClip combatTiles;
	public AudioClip combatBoss;
	
	public AudioSource soundPlayer;
	
	public GameObject settingsMenu;
	
	private bool isAutofireOn = false;
	
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		// if there is already an audio player then destroy it
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
		else{
			soundPlayer = GetComponent<AudioSource>();
			changeToMusicClip(0);
			changeVolumeLevel(0.5f);
			changeVolumeSFXLevel(0.5f);
		}
	}
	
	public void SetInfo(int level, int character){
		selectedLevel = level;
		selectedCharacter = character;
	}
	
	public int getSelectedLevel(){
		return selectedLevel;
	}
	
	public int getSelectedCharacter(){
		return selectedCharacter;
	}
	
	public void changeVolumeLevel(float vol){
		soundPlayer.volume = vol;
		volumeMusicLevel = vol;
	}
	
	public void changeVolumeSFXLevel(float vol){
		volumeSFXLevel = vol;
	}
	
	public float getVolumeMusicLevel(){
		return volumeMusicLevel;
	}
	
	public float getVolumeSFXLevel(){
		return volumeSFXLevel;
	}
	
	public void changeToMusicClip(int clip){
		switch(clip){
			case 0:
				soundPlayer.clip = menuMusic;
				soundPlayer.Play();
				break;
			case 1:
				soundPlayer.clip = combatGrass;
				soundPlayer.Play();
				break;
			case 2:
				soundPlayer.clip = combatDirt;
				soundPlayer.Play();
				break;
			case 3:
				soundPlayer.clip = combatDesert;
				soundPlayer.Play();
				break;
			case 4:
				soundPlayer.clip = combatTiles;
				soundPlayer.Play();
				break;
			case 5:
				soundPlayer.clip = combatBoss;
				soundPlayer.Play();
				break;
		}
	}
	
	public void InstantiateSettingsMenu(){
		GameObject newMenu = Instantiate(settingsMenu);
		newMenu.transform.Find("SliderMusic").transform.GetComponent<Slider>().value = volumeMusicLevel;
		newMenu.transform.Find("SliderSFX").transform.GetComponent<Slider>().value = volumeSFXLevel;
		newMenu.transform.Find("AutofireToggle").transform.GetComponent<Toggle>().isOn = isAutofireOn;
	}

	public GameObject GetSettingsMenu(){
		return settingsMenu;
	}
	
	public void ChangeAutofireSetting(bool newValue){
		isAutofireOn = newValue;
	}
	
	public bool GetAutofireSetting(){
		return isAutofireOn;
	}
}
