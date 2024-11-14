using UnityEngine;
using System.Collections;

public class Fps : MonoBehaviour
{

	string label = "";
	float count;
	private GUIStyle guiStyle = new GUIStyle();



	IEnumerator Start()
	{

		guiStyle.fontSize = 32;

		GUI.depth = 2;
		while (true)
		{
			if (Time.timeScale == 1)
			{
				yield return new WaitForSeconds(0.1f);
				count = (1 / Time.deltaTime);
				label = "FPS :" + (Mathf.Round(count));
			}
			else
			{
				label = "Pause";
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	void OnGUI()
	{
		GUI.Label(new Rect(5, 40, 200, 50), label, guiStyle);
	}
}