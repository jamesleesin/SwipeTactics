using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
	private int myLevel;
	private bool unlocked = false;
	
	public void UnlockMe(){
		unlocked = true;
	}
	
	public void SelectedMe(){
		if (unlocked){
			myLevel = int.Parse(transform.GetChild(0).GetComponent<Text>().text);
			GameObject.Find("Start").GetComponent<LevelSelection>().UpdateSelectedLevel(myLevel);
		}
	}
}
