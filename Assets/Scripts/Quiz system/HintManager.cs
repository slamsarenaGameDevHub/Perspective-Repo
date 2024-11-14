using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
	[TextArea(4,9)]
	public List<string> hints=new List<string>();
	
	public string GiveHint(int hintNumber) 
	{
		return hints[hintNumber];
	}
   
}
