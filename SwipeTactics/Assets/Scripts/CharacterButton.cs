using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterButton : MonoBehaviour {
	private int myChar;

	// Use this for initialization
	void Start () {
		string name = transform.name;
		if (name == "Peasant"){
			myChar = 0;
		}
		else if (name == "Archer"){
			myChar = 1;
		}
		else if (name == "Knight"){
			myChar = 2;
		}
		else if (name == "Ninja"){
			myChar = 3;
		}
		else if (name == "Mage"){
			myChar = 4;
		}
	}
	
	public void SelectedMe(){
		GameObject.Find("Start").GetComponent<LevelSelection>().UpdateSelectedChar(myChar);
	}
}
