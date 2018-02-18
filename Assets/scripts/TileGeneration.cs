using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour {

	public Texture2D textureToGenerateFrom;
	public GameObject tilePrefab;
	private int xMargin = -77;
	private int yMargin = -144;

	private int xTileSize;
	private int yTileSize;

	void Start() 
	{
		xTileSize = gameManager.instance.xTileSize;
		yTileSize = gameManager.instance.yTileSize;
		GameObject.FindWithTag("Player").transform.position = new Vector2((xTileSize*3) + xMargin, (yTileSize*3) + yMargin);
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
				Vector2 pos = new Vector2((i % texWidth * xTileSize) + xMargin, (((i - (i % texWidth))/ texWidth) * yTileSize) + yMargin);

				//creates tile at the new position with a zero rotation (Quaternion.identity)
				GameObject newTile = Instantiate(tilePrefab, pos, Quaternion.identity);
				newTile.GetComponent<SpriteRenderer>().sortingOrder = -i;

				newTile.transform.parent = transform;

			}
		}
	}	
}

