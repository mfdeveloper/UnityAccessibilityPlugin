﻿using UnityEngine;

public class Instructions : MonoBehaviour
{
	//////////////////////////////////////////////////////////////////////////

	public void OnBackButtonPressed()
	{
		Instantiate(Resources.Load("Main Menu"));
		DestroyImmediate(gameObject);
	}

	//////////////////////////////////////////////////////////////////////////
}