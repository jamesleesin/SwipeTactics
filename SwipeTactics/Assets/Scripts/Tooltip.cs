using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour {
	
	
	public void CloseTooltip(){
		GameObject.Find("GameManager").transform.GetComponent<GameManager>().closeTooltip();
		transform.gameObject.SetActive(false);
		Destroy(GameObject.Find("Tooltip(Clone)"));
	}
}
