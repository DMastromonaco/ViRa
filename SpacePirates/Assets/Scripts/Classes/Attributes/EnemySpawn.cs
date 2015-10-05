using UnityEngine;
using System.Collections;

public class EnemySpawn : TileAttribute
{
	private static TileAttributeType _type = TileAttributeType.enemySpawn;

	public int index = -1;

	public override TileAttributeType getType()
	{
		return _type;
	}

	public override string getParameters()
	{
		Debug.Log("EnemySpawn : getParams");
		
		//Store relavent trigger parameters (inspector set during runtime) into a CSS string and return
		string css = "";

		css += index.ToString() + ",";
		
		return css;
	}
	
	public override void setParameters(string[] args)
	{
		Debug.Log("EnemySpawn : setParams");

		//Parse args back into their appropriate variables (happens during map spawn)

		int tempIndex = -1;

		if(int.TryParse(args[3], out tempIndex))
		{
			index = tempIndex;
		}
		else
		{
			Debug.LogError("EnemySpawn : Failed to parse index");
		}
	}
}
