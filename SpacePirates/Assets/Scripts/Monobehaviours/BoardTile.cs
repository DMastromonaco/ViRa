using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class BoardTile : MonoBehaviour, iClickable, iBuildingReceiver
{
	//Does not need to be assigned in inspector
	public GameObject GO_tile;
	public int ID = -1;
	public Vector2 v2_boardRowCol = new Vector2(-1,-1);

	//Inspector, LOCS
	public GameObject loc_childPlacement;

	//Inspector, tile gfx
	public GameObject GO_rend;
	private Renderer rend_GFX;

	//FLAG for if this tile is painted or not
	public bool isMarked = false;

	// mats for tile gfx
	public Material mat_default;
	public Material mat_red;
	
	//Inspector, highlight Image
	public SpriteRenderer sprite_highlight;

	// colors for highlight
	public Color color_highlight_off;
	public Color color_highlight_on;

	// Game Objects to locate Neighbors
	public List<GameObject> locs_neighbors = new List<GameObject>();

	//Neighbors
	public List<BoardTile> tileNeighbors = new List<BoardTile>();

	//Private, unused
	private inputTracker _myInput;

	#region Public Methods

	//INIT MUST BE CALLED ! when this prefab is spawned
	public void Init(int index, Vector2 v2_rowColumn, Vector2 v2_neighborSpacing)
	{
		GO_tile = this.gameObject;

		//Store indentifying information
		ID = index;
		v2_boardRowCol = v2_rowColumn;

		//starts not painted
		rend_GFX = GO_rend.GetComponent<Renderer>();
		isMarked = false;

		//gfx
		SetHighlight(color_highlight_off);

		//Set locs at correct spacing, in correct directions
		FixNeighborLocators(v2_neighborSpacing);
	}

	#endregion
	
	#region iClickable interfaces
	public void ClickStart(inputTracker input)
	{
		if(BrushType.Off != (BrushType)Painting.instance.paintBrush.currentBrush)
		{
			//flag on
			Painting.instance.StartPainting();

			//Process this tile as clicked for painting
			PaintClick((BrushType)Painting.instance.paintBrush.currentBrush);
		}

		if(BuildingType.off != (BuildingType)Buildings.instance.buildingPlacement.currentBuilding)
		{
			//Only if we don't have a building start the placement
			if(canReceiveBuilding())
			{
				//flag on
				Buildings.instance.StartPlacement();

				Buildings.instance.PlaceTempBuilding(this as iBuildingReceiver);
			}
		}
	}
	
	public void ClickEnd(inputTracker input)
	{
		if(Painting.instance.paintBrush.isOn)
		{
			//flag off
			Painting.instance.StopPainting();
		}

		if(Buildings.instance.buildingPlacement.isOn)
		{
			//clear temp building display
			Buildings.instance.ClearTempBuilding();

			//and place a newly spawned building on this tile
			Buildings.instance.PlaceBuildingOnCurrent();

			//flag off
			Buildings.instance.StopPlacement();
		}
	}
	
	public void HoverStart(inputTracker input)
	{
		SetHighlight(color_highlight_on);

		if(Painting.instance.paintBrush.isOn)
		{
			if(BrushType.Off != (BrushType)Painting.instance.paintBrush.currentBrush)
			{
				//Process this tile as clicked for painting
				PaintClick((BrushType)Painting.instance.paintBrush.currentBrush);
			}
		}

		if(Buildings.instance.buildingPlacement.isOn)
		{
			if(BuildingType.off != (BuildingType)Buildings.instance.buildingPlacement.currentBuilding)
			{
				//Only if we don't have a building display the hover
				if(canReceiveBuilding())
				{
					Buildings.instance.PlaceTempBuilding(this as iBuildingReceiver);
				}
			}
		}
	}
	
	public void HoverEnd(inputTracker input)
	{
		SetHighlight(color_highlight_off);
	}
	#endregion

	private iBuildingPlacer _myBuildingUnit = null;

	#region iBuildingReceiver interfaces

	public void ReceiveBuilding(iBuildingPlacer building)
	{
		_myBuildingUnit = building;
	}

	public bool canReceiveBuilding()
	{
		return (_myBuildingUnit == null);
	}

	public Vector3 GetPlacementLocation()
	{
		return loc_childPlacement.transform.position;
	}

	#endregion



	// INTERNAL

	private void FixNeighborLocators(Vector2 v2_neighborSpacing)
	{
		if(locs_neighbors.Count == 4)
		{
			locs_neighbors[0].transform.localPosition = new Vector3(-1f * v2_neighborSpacing.x, 0f, 0f);
			locs_neighbors[1].transform.localPosition = new Vector3(0f, 0f, v2_neighborSpacing.y);
			locs_neighbors[2].transform.localPosition = new Vector3(v2_neighborSpacing.x, 0f, 0f);
			locs_neighbors[3].transform.localPosition = new Vector3(0f, 0f, -1f * v2_neighborSpacing.y);
		}
		else
		{
			Debug.LogError("Neighbor locators not assigned");
		}
	}


	// MORE INTERNAL

	// Painting

	private void PaintClick(BrushType brushType)
	{
		switch(brushType)
		{
			////
		case BrushType.Off:
			break;

			////
		case BrushType.DrawSingle:
			Mark();
			break;

		case BrushType.DrawMed:
			Mark();
			MarkAllNeighbors();
			break;
			
		case BrushType.DrawWide:
			Mark();
			MarkAllNeighbors();
			for(int x = 0; x < tileNeighbors.Count; x++)
			{
				if(null != tileNeighbors[x])
				{
					tileNeighbors[x].MarkAllNeighbors();
				}
			}
			break;

			////
		case BrushType.EraseSingle:
			Erase();
			break;
		
		case BrushType.EraseMed:
			Erase();
			EraseAllNeighbors();
			break;
			
		case BrushType.EraseWide:
			Erase();
			EraseAllNeighbors();
			for(int x = 0; x < tileNeighbors.Count; x++)
			{
				if(null != tileNeighbors[x])
				{
					tileNeighbors[x].EraseAllNeighbors();
				}
			}
			break;
		}
	}

	// PAINTING

	public void Mark()
	{
		SetMat(mat_red);
		isMarked = true;
	}

	public void Erase()
	{
		SetMat(mat_default);
		isMarked = false;
	}

	public void MarkAllNeighbors()
	{
		for(int x = 0; x < tileNeighbors.Count; x++)
		{
			if(null != tileNeighbors[x])
			{
				tileNeighbors[x].Mark();
			}
		}
	}

	public void EraseAllNeighbors()
	{
		for(int x = 0; x < tileNeighbors.Count; x++)
		{
			if(null != tileNeighbors[x])
			{
				tileNeighbors[x].Erase();
			}
		}
	}

	//for painting
	private void SetMat(Material whatMat)
	{
		Material[] mats = rend_GFX.materials;
		mats[0] = whatMat;
		rend_GFX.materials = mats;
	}
	
	private void SetHighlight(Color whatColor)
	{
		sprite_highlight.color = whatColor;
	}

	////////////////////////////////////////////

	public BoardTile GetNeighbor(NeighborDirection which)
	{
		//return null for no neighbors
		if(tileNeighbors.Count < 4)
		{
			return null;
		}

		//Order is : North, East, South, West
		return tileNeighbors[(int)which];
	}

	public void SetNeighbors_FromLocators()
	{
		if(locs_neighbors.Count == 4)
		{
			BoardTile tempTile = null;
			int i = 0;
			bool tileFound = false;

			tileNeighbors = new List<BoardTile>();

			//Order is : North, East, South, West
			for(int x = 0; x < 4; x++)
			{
				//get all colliders for current locator
				Collider[] hitColliders = Physics.OverlapSphere(locs_neighbors[x].transform.position, 0.1f);

				//reset counters
				i = 0;
				tileFound = false;

				//Loop them and check for board tiles, or assign null
				while (i < hitColliders.Length && !tileFound) 
				{
					tempTile = hitColliders[i].gameObject.GetComponent<BoardTile>();
					if(tempTile != null)
					{
						//Tile found, assign
						tileNeighbors.Add(tempTile);
						//and drop out
						tileFound = true;
					}
					i++;
				}

				if(!tileFound)
				{
					//If this tile was not found, null will be that neighbor
					tileNeighbors.Add(null);
				}
			}
		}
	}
}
