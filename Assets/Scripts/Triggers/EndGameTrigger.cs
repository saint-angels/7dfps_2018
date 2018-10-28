using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour {

    [SerializeField] GameObject endGamePanel;
    [SerializeField] bool showedGameOver;


    private void Update()
    {
        if (showedGameOver && Input.anyKeyDown)
        {
            Application.Quit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            showedGameOver = true;
            endGamePanel.SetActive(true);
        }
    }
}
