using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] public string levelOne;

    private void Start()
    {
        Cursor.visible = true;
    }

    public void LoadLevelOne()
    {
        SceneManager.LoadScene(levelOne);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
