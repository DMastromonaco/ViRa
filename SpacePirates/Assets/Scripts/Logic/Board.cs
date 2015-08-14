using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : Singleton<Board>
{
	//== PUBLIC inspector vars
	public GameObject GO_Root;

	public GameObject prefab_BoardTile;

	//== PRIVATE member vars
	private Vector2 v2_DefaultBoardSize = new Vector2(40, 40);
	private Vector2 v2_BoardSpacing = new Vector2(0.5f, 0.5f);

	//internal, to track spawned prefabs
	private List<BoardTile> boardTiles = new List<BoardTile>();

	public void Spawn_Default()
	{
		Spawn(v2_DefaultBoardSize);
	}

	public void Spawn(Vector2 size)
	{
		//Clear current tiles
		Despawn ();

		GameObject tempBoardTileGO = null;
		BoardTile tempTile = null;
		Vector2 curOffset = new Vector2(0.0f, 0.0f);

		int tileIndex = 0;

		for(int x = 0; x < size.x; x++)
		{
			for(int y = 0; y < size.y; y++)
			{
				//Spawn here
				tempBoardTileGO = GameObject.Instantiate(prefab_BoardTile);

				//Parent new tile to root
				tempBoardTileGO.transform.parent = GO_Root.transform;
				//Set local position of tile to current placement offset
				tempBoardTileGO.transform.localPosition = new Vector3(curOffset.x,
				                                                      0.0f,
				                                                      curOffset.y);

				//Setup the board tile class object to represent this tile
				tempTile = tempBoardTileGO.GetComponent<BoardTile>();
				tempTile.Init(tileIndex, new Vector2(x, y), v2_BoardSpacing);

				//Add the board tile object to our internal collection
				boardTiles.Add(tempTile);

				//Increment Y
				curOffset.y += v2_BoardSpacing.y;
				//Increment index for each tile
				tileIndex++;
			}

			//Reset Y and increment X
			curOffset.y = 0f;
			curOffset.x += v2_BoardSpacing.x;
		}

		//Once all tiles are spawned, have them find their neighbors
		StartCoroutine(Do_FindNeighbors());
	}

	public void Spawn(List<Vector2> tileLocations)
	{
		//Clear current tiles
		Despawn ();

		//Reset temp vars
		GameObject tempBoardTileGO = null;
		BoardTile tempTile = null;
		Vector2 curOffset = new Vector2(0.0f, 0.0f);
		
		int tileIndex = 0;

		//find the largest X and Y row and column
		float maxX = 0f;
		float maxY = 0f;
		for(int i = 0; i < tileLocations.Count; i++)
		{
			if(tileLocations[i].x > maxX)
			{
				maxX = tileLocations[i].x;
			}

			if(tileLocations[i].y > maxY)
			{
				maxY = tileLocations[i].y;
			}
		}
		//And set the size to spawn as those loop limits
		Vector2 size = new Vector2(maxX, maxY);

		for(float x = 0.0f; x <= size.x; x++)
		{
			for(float y = 0.0f; y <= size.y; y++)
			{
				//Don't loop past the end of the tile locations
				if(tileIndex < tileLocations.Count)
				{
					//Don't spawn until we match the new one in the locations tile list
					if(tileLocations[tileIndex].x == x && tileLocations[tileIndex].y == y)
					{
						//Spawn here
						tempBoardTileGO = GameObject.Instantiate(prefab_BoardTile);
						
						//Parent new tile to root
						tempBoardTileGO.transform.parent = GO_Root.transform;
						//Set local position of tile to current placement offset
						tempBoardTileGO.transform.localPosition = new Vector3(curOffset.x,
						                                                      0.0f,
						                                                      curOffset.y);
						
						//Setup the board tile class object to represent this tile
						tempTile = tempBoardTileGO.GetComponent<BoardTile>();
						tempTile.Init(tileIndex, new Vector2(x, y), v2_BoardSpacing);
						
						//Add the board tile object to our internal collection
						boardTiles.Add(tempTile);

						//Increment index for each tile ACTUALLY SPAWNED
						tileIndex++;
					}
				}

				//Increment Y
				curOffset.y += v2_BoardSpacing.y;
			}
			
			//Reset Y and increment X
			curOffset.y = 0f;
			curOffset.x += v2_BoardSpacing.x;
		}
		
		//Once all tiles are spawned, have them find their neighbors
		StartCoroutine(Do_FindNeighbors());
	}

	IEnumerator Do_FindNeighbors()
	{
		yield return new WaitForFixedUpdate();

		//Once all tiles are spawned, have them find their neighbors
		for(int j = 0; j < boardTiles.Count; j++)
		{
			boardTiles[j].SetNeighbors_FromLocators();
		}
	}

	///////////////////////////////////

	public void Despawn()
	{
		BoardTile tempTile = null;
		GameObject tempGO = null;

		//delete all spawned tiles and clear internal list
		while(boardTiles.Count > 0)
		{
			tempTile = boardTiles[0] as BoardTile;
			tempGO = tempTile.GO_tile;

			boardTiles.RemoveAt(0);
			Destroy(tempGO);
		}

		boardTiles.Clear();
	}

	/// Marked Tile Handling

	public List<Vector2> GetMarkedTiles()
	{
		List<Vector2> markedList = new List<Vector2>();

		//Once all tiles are spawned, have them find their neighbors
		for(int j = 0; j < boardTiles.Count; j++)
		{
			if(boardTiles[j].isMarked)
			{
				markedList.Add(boardTiles[j].v2_boardRowCol);
			}
		}

		return markedList;
	}

	public void BoardTiles_MarkFromList(List<Vector2> markedList)
	{
		int markIndex = 0;

		for(int j = 0; j < boardTiles.Count; j++)
		{
			if(markIndex < markedList.Count)
			{
				if(boardTiles[j].v2_boardRowCol == markedList[markIndex])
				{
					//this board tile is in the list, mark it
					boardTiles[j].Mark();

					//and increment the index of the next V2 to look for match
					markIndex++;
				}
				else
				{
					//If the board tile we are on is already past the marked one we are looking for, we must advance the index until we catch up
					while(markIndex < markedList.Count &&
						(boardTiles[j].v2_boardRowCol.x > markedList[markIndex].x ||
					      (boardTiles[j].v2_boardRowCol.x == markedList[markIndex].x &&
					       boardTiles[j].v2_boardRowCol.y > markedList[markIndex].y)))
					{
						markIndex++;
					}

					//See if we have advanced to the one we are on
					if(markIndex < markedList.Count)
					{
						if(boardTiles[j].v2_boardRowCol == markedList[markIndex])
						{
							//this board tile is in the list, mark it
							boardTiles[j].Mark();
							
							//and increment the index of the next V2 to look for match
							markIndex++;
						}
					}
				}
			}
		}
	}

	public void BoardTiles_MarkAll()
	{
		for(int j = 0; j < boardTiles.Count; j++)
		{
			boardTiles[j].Mark();
		}
	}

	public void BoardTiles_ClearAllMarks()
	{
		for(int j = 0; j < boardTiles.Count; j++)
		{
			boardTiles[j].Erase ();
		}
	}
}
