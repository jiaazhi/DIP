using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
	public int cost;
	public string itemname;

	public void bought()
	{
		if(GetComponentInParent<shop>().money >= cost)
        {
			GetComponentInParent<shop>().money -= cost;
			GetComponentInParent<shop>().additem(itemname);
        }
	}
}
