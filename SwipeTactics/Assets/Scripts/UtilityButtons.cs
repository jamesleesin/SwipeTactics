using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UtilityButtons : MonoBehaviour {
	//private GameObject settingsMenu;
	private bool inGame = false;
	
	void Start(){
		if (transform.name == "SettingsButton"){
			inGame = true;
		}
	}
	
	public void Back(){
		SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
	}
	
	public void Close(){
		PlayerPrefs.Save();
		Application.Quit();
	}
	
	public void Play(){
		SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
	}
	
	public void Settings(){
		if ((inGame && !GameObject.Find("GameManager").transform.GetComponent<GameManager>().isGameOver()) || !inGame){
			GameObject.Find("Info").transform.GetComponent<Info>().InstantiateSettingsMenu();

			//settingsMenu.SetActive(true);
			// open settings UI
			Slider volumeMusicSlider = GameObject.Find("SliderMusic").transform.GetComponent<Slider>();
			volumeMusicSlider.onValueChanged.AddListener(VolumeChange);
			VolumeChange(volumeMusicSlider.value);
			// sound effects slider
			Slider volumeSFXSlider = GameObject.Find("SliderSFX").transform.GetComponent<Slider>();
			volumeSFXSlider.onValueChanged.AddListener(VolumeSFXChange);
			VolumeSFXChange(volumeSFXSlider.value);
			// toggle button
			Toggle autofireToggle = GameObject.Find("AutofireToggle").transform.GetComponent<Toggle>();
			autofireToggle.onValueChanged.AddListener(AutofireToggleChange);
			AutofireToggleChange(autofireToggle.isOn);
		}
	}
	
	public void BackFromSettings(){
		Destroy(transform.parent.gameObject);
	}
	
	// slider music value changed
	public void VolumeChange(float vol)
    {
        GameObject.Find("Info").transform.GetComponent<Info>().changeVolumeLevel(vol);
    }
	
	// slider SFX value changed
	public void VolumeSFXChange(float vol){
		GameObject.Find("Info").transform.GetComponent<Info>().changeVolumeSFXLevel(vol);
		if (inGame){
			GameObject.Find("GameManager").transform.GetComponent<GameManager>().ChangeSFXSoundVolume(vol);
		}
	}
	
	// toggle autofire on off
	public void AutofireToggleChange(bool value){
		GameObject.Find("Info").transform.GetComponent<Info>().ChangeAutofireSetting(value);
		if (inGame){
			GameObject.Find("GameManager").transform.GetComponent<GameManager>().ChangeAutofireSetting(value);
		}
	}
}
