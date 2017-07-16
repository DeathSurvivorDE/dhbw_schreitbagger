using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamerawechsel : MonoBehaviour {

	[Header("Cameras on Keys 1-6")]
	public Camera[] Kamera = new Camera[6];
	//public Camera Kamera2, Kamera3, Kamera4, Kamera5, Kamera6;

	string guiString;
	GUIStyle style;

	// Use this for initialization
	void Start () {
		guiString = "Kamera: 1  2  3  4  5  6";

		style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = (int)(Screen.height * 0.03f);
		style.normal.textColor = new Color (255.0f, 255.0f, 200.5f, 1.0f);

	}
	
	// Update is called once per frame
	void Update ()
	{
		// Key 1
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			ActivateCameras (1);
		// Key 2
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			ActivateCameras (2);
		// Key 3
		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			ActivateCameras (3);
		// Key 4
		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			ActivateCameras (4);
		// Key 5
		} else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			ActivateCameras (5);
		// Key 6
		} else if (Input.GetKeyDown (KeyCode.Alpha6)) {
			ActivateCameras (6);
		}
	}

	void ActivateCameras(int number)
	{
		guiString = "Kamera: ";
		if (number > 0 && number <= Kamera.Length)
			for (int id = 0; id < Kamera.Length; id++) {
				if (Kamera [id] != null) {
					Kamera [id].enabled = (id + 1) == number;
					guiString += string.Format ("{1}{0}{2} ", (id + 1), Kamera[id].enabled ? ">>" : " ", Kamera[id].enabled ? "<<" : " ");
				}
			}
	}

	void OnGUI()
	{
		int w = Screen.width;
		int h = Screen.height;
		Rect rect = new Rect(w * 0.6f, h * 0.97f, w * 0.05f, h * 0.03f);

		GUI.Label (rect, guiString, style);
	}
}
