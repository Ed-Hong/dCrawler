﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour {

	public Texture2D textureToGenerateFrom;
	public GameObject tilePrefab;

	private int xTileSize;
	private int yTileSize;

	void Start() 
	{
  	    xTileSize = gameManager.xTileSize;
		yTileSize = gameManager.yTileSize;
        GameObject.FindWithTag("Player").transform.position = new Vector2(
			Mathf.Round(xTileSize*3), 
			Mathf.Round(yTileSize*3)
		);
    
		GenerateLevel();
	}

	void GenerateLevel()
	{
	
		Color32[] pixels = textureToGenerateFrom.GetPixels32();
		int texWidth = textureToGenerateFrom.width;

		for(int i = 0; i < pixels.Length; i++)
		{
			if(pixels[i] == Color.black)
			{
                //finds position for tile based on pixel (multiplies by tile size
                //if u want me to explain this, just ask
                float x = i % texWidth;
                float y = (i - x) / texWidth;
                Vector2 pos = new Vector2((x * xTileSize), (y * yTileSize));

				//creates tile at the new position with a zero rotation (Quaternion.identity)
				GameObject newTile = Instantiate(tilePrefab, pos, Quaternion.identity);
				newTile.GetComponent<SpriteRenderer>().sortingOrder = -(int)(y);

				newTile.transform.parent = transform;

			}
		}
	}	
}

