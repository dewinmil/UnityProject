﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunInBackground : MonoBehaviour
{

	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
	}
}
