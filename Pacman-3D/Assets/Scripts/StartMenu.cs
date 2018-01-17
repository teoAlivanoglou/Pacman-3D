using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public TMPro.TextMeshProUGUI player2GhostText;
    public GhostController[] ghosts;

    private List<string> ghostNames;
    private int selected = 0;

    void Awake()
    {
        Time.timeScale = 0;
        ghostNames = new List<string>() {
            "<color=#FD4425>Blinky",
            "<color=#F0A6EE>Pinky",
            "<color=#93C9F4>Inky",
            "<color=#F2C06D>Clyde"};
    }

    public void CycleNext()
    {
        selected = ghostNames.Next(selected);
        SetText();
    }

    public void CyclePrevious()
    {
        selected = ghostNames.Previous(selected);
        SetText();
    }

    private void SetText ()
    {
        player2GhostText.text = ghostNames[selected];
    }

    public void StartGame()
    {

        Time.timeScale = 1;
        ghosts[selected].ai = false;
        gameObject.SetActive(false);
        GameManager.Instance.ResetBoard();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
