using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveMarkingImage : MonoBehaviour
{
	public RawImage myMarkImage;

	public void AboveThisTransform()
	{
		Vector3 newPos = new Vector3(this.transform.position.x,
		                             this.transform.position.y + 1.75f,
		                             this.transform.position.z);

		myMarkImage.rectTransform.position = newPos;
	}
}
