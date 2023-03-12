using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Minigame : MonoBehaviour
{
    [SerializeField]
    GameObject[] cubes;

    [SerializeField]
    GameObject arrow;

    [SerializeField]
    Hacker hacker;
    
    float progress = 0f;
    float turnsPerSecond = 0.6f;
    
    bool[] isSetCorrectly = new bool[6];
    bool isMovingForward = true;

    public void InitializeGame()
    {
        for (var i = 0; i < 6; i++)
        {
            isSetCorrectly[i] = false;
        }
        UpdateCubeColors();
    }

    void CheckIfGameIsWon()
    {
        for (var i = 0; i < 6; i++)
        {
            if (!isSetCorrectly[i])
            {
                return;
            }
        }

        StartCoroutine(hacker.FinishMinigame());
    }
    
    void UpdateCubeColors()
    {
        for (var i = 0; i < 6; i++)
        {
            cubes[i].GetComponent<Renderer>().material.color = isSetCorrectly[i] ? Color.green : Color.red;
        }
    }
    
    void Start()
    {
        InitializeGame();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isMovingForward)
        {
            progress += Time.deltaTime;
        }
        else
        {
            progress -= Time.deltaTime;
        }

        if (progress >= 1f)
        {
            progress -= 1f;
        }
        else if (progress <= 0f)
        {
            progress += 1f;
        }
        
        arrow.transform.localRotation = Quaternion.Euler(
            arrow.transform.localRotation.eulerAngles.x, 
            progress * 360.0f, 
            arrow.transform.localRotation.eulerAngles.z
            );

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            var currentIndex = (int)Mathf.Floor(progress * 6);
            isSetCorrectly[currentIndex] = !isSetCorrectly[currentIndex];
            cubes[currentIndex].GetComponent<Renderer>().material.color = isSetCorrectly[currentIndex] ? Color.green : Color.red;

            if (Random.Range(0, 10) < 2)
            {
                isMovingForward = !isMovingForward;
            }
        }

        CheckIfGameIsWon();
    }
}
