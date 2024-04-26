using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class PlayersInput : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip startGame;
    [SerializeField] Image MenuBG;
    [SerializeField] Color FadedColor;
    [SerializeField] MenuButtons MenuButtons;
    [SerializeField] TextMeshProUGUI[] PlayersName;
    [SerializeField] TextMeshProUGUI LoadingText;
    [SerializeField] GameObject PlayersInfoPanel;
    bool[] PlayersAreReady = new bool[2];


    public void SetPlayersInfo()
    {
        MenuButtons.HideButtons();

        LeanTween.delayedCall(1.5f, () =>
        LeanTween.moveLocalY(PlayersInfoPanel, 0f, 1.5f).setEaseOutBack());
        LeanTween.delayedCall(3f, () => audioSource.Play());

        LeanTween.delayedCall(3f, () =>
        MenuBG.color = FadedColor);
        FadedColor.a = 1f;
    }
    public void SetPlayerOneFaction(int FactionID) => PlayerPrefs.SetInt("P1Faction", FactionID);

    public void SetPlayerTwoFaction(int FactionID) => PlayerPrefs.SetInt("P2Faction", FactionID);

    public void PlayerOneReady()
    {
        PlayersAreReady[0] = true;
        if (PlayersAreReady[0] && PlayersAreReady[1])
        {
            this.gameObject.GetComponent<AudioSource>()?.PlayOneShot(startGame);
            StartGame();
        }

    }
    public void PlayerTwoReady()
    {
        PlayersAreReady[1] = true;
        if (PlayersAreReady[0] && PlayersAreReady[1])
        {
            this.gameObject.GetComponent<AudioSource>()?.PlayOneShot(startGame);
            StartGame();
        }

    }

    public void StartGame()
    {
        PlayerPrefs.SetString("PlayerOneNick", PlayersName[0].text);
        PlayerPrefs.SetString("PlayerTwoNick", PlayersName[1].text);
        LeanTween.moveLocalY(PlayersInfoPanel.gameObject, -1200f, 2f).setEaseInBack()
        .setOnComplete(() => LoadingText.gameObject.SetActive(true));
        MenuBG.color = Color.white;
        LeanTween.delayedCall(2.1f, () =>
        MenuButtons.StartGame());
    }

}