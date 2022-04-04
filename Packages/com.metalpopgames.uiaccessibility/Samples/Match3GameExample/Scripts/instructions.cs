using System.Collections;
using UnityEngine;
using UAP.Core;

public class instructions : MonoBehaviour
{
	//////////////////////////////////////////////////////////////////////////

	public void OnBackButtonPressed()
	{
		Instantiate(Resources.Load("Main Menu"));
		DestroyImmediate(gameObject);
	}

	//////////////////////////////////////////////////////////////////////////
}
