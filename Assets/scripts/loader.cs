using UnityEngine;
using System.Collections;
//Loads a gameManger if it doesnt exist.
public class loader : MonoBehaviour 
{
	public GameObject gameMangerPrefab;
	
	void Awake ()
	{
		if (gameManager.instance == null)
		{
			Instantiate(gameMangerPrefab);
		}
	}
}