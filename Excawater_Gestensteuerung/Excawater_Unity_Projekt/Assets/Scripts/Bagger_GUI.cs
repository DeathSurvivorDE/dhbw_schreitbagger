using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bagger_GUI
{
	GUIStyle style;
	float deltaTime = 0.0f;

	internal Bagger_GUI()
	{
		style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = (int)(Screen.height * 0.03f);
		style.normal.textColor = new Color (255.0f, 255.0f, 200.5f, 1.0f);

	}

	internal void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	internal void ZeigeHandStatus(int nHand, string message)
	{

		GUI.Label (new Rect (Screen.width - 400, 10, 200, 200), string.Format ("Hände:\t{0}\n{1}",
			nHand, message), style);
	}

	internal void ZeigeBaggerWinkel(float W_Rotation, float W_Ausleger, float W_Löffelstiel, float W_Löffel)
	{
		GUI.Label (new Rect (10, 10, 200, 200), string.Format ("Rotation:\t{0:N2}\nAusleger:\t{1:N2}\nStiel:\t{2:N2}\nLöffel:\t{3:N2}",
			W_Rotation, W_Ausleger, W_Löffelstiel, W_Löffel), style);
	}

	internal void ZeigeFPS()
	{
		int w = Screen.width, h = Screen.height;

		Rect rect = new Rect(0, h * 0.97f, w * 0.05f, h * 0.03f);

		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}
