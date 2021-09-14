using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private Transform cursorObject;
    [SerializeField] private GameObject WinText;
    [SerializeField] private string nextLevel;

    private Camera cam;
    private bool won;
    // Update is called once per frame

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }

    private void Start()
    {
        cam = Camera.main;
        Cursor.visible = false;
        won = false;
    }

    void Update()
    {
        //Vector3 cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
        //cursorObject.position = new Vector3(cursorPos.x, cursorPos.y, 0);
        if (won && Input.anyKeyDown)
            SceneManager.LoadScene(nextLevel);
        
        cursorObject.position = Input.mousePosition;
        if(Input.GetKeyDown(KeyCode.RightBracket))
            SceneManager.LoadScene(0);
    }

    public void ReloadLevel()
    {
        if (won)
            return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Win()
    {
        won = true;
        WinText.SetActive(true);
    }
}
