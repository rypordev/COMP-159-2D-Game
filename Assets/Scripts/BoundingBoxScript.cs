using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxScript : MonoBehaviour
{
    private GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        gm.ReloadLevel();
    }
}
