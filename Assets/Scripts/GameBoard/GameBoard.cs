using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Battlefield[] playerBattlefield;
    [SerializeField] private PlayerBoard[] playerBoards;
    [SerializeField] private PlayerInfo[] playersInfo;
    [SerializeField] Weathers weathers;
    [SerializeField] Button passButton;
    public enum Weather { Blizzard, Fog, Rain }

    private bool[] isWeatherActive = new bool[3];
    public bool IsWeatherActive(Weather weather) => isWeatherActive[(int)weather];

    //Accesors
    public Battlefield[] PlayerBattlefield => playerBattlefield;
    public PlayerBoard[] PlayerBoards => playerBoards;
    public Weathers Weathers => weathers;



    // Frontend
    public void HidePlayerBoards()
    {
        for (int i = 0; i < 2; i++)
            LeanTween.scaleX(PlayerBoards[i].gameObject, 0, 0f);
    }

    public void SetActivePlayer(Player currentPlayer, bool IsActive)
    {
        var playerBoard = PlayerBoards[(int)currentPlayer];
        if (IsActive)
            LeanTween.scaleX(playerBoard.gameObject, 1f, 1.2f).setEaseOutBounce();
        else
            LeanTween.scaleX(playerBoard.gameObject, 0f, 1.2f);
    }

    // Behavior
    public void DealCards(Player player, int n) => PlayerBoards[(int)player].DealCards(n);
    public void ResetField()
    {
        HidePlayerBoards();
        for (int i = 0; i < 2; i++) playerBattlefield[i].ResetField();
    }
    public void SetWeather(Weather weather)
    {
        isWeatherActive[(int)weather] = true;
        var affectedRow = PlayerBattlefield[0].Rows[(int)weather];
        affectedRow.SetWeather();
        affectedRow = PlayerBattlefield[1].Rows[(int)weather];
        affectedRow.SetWeather();
    }
    public void ResetWeather()
    {
        
        weathers.ClearingEffect();
        for (int i = 0; i < 3; i++)
        {
            isWeatherActive[i] = false;
            PlayerBattlefield[0].Rows[i].ResetWeather();
            PlayerBattlefield[1].Rows[i].ResetWeather();
        }
    }

    public void ConsumePlayerBattery(Player? winner)
    {
        GameObject battery = playersInfo[(int)winner].Battery[0].gameObject;
        battery.gameObject.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            LeanTween.alpha(battery, 0f, 0.5f).setOnComplete(() =>
                LeanTween.alpha(battery, 1f, 0.5f));
        }


    }
    void Update()
    {
        passButton.interactable = (GameManager.Instance.CurrentTurnPhase == TurnPhase.Play);

        for (int i = 0; i < 2; i++)
        {
            var info = playersInfo[i];
            info.CardsInHand.text = playerBoards[i].Hand.transform.childCount.ToString();
            info.PlayerPower.text = playerBattlefield[i].FieldPower.ToString();
        }

        if (playerBattlefield[0].FieldPower > playerBattlefield[1].FieldPower)
        {
            playersInfo[0].PlayerPower.color = Color.green;
            playersInfo[1].PlayerPower.color = Color.red;
        }
        else if (playerBattlefield[0].FieldPower < playerBattlefield[1].FieldPower)
        {
            playersInfo[1].PlayerPower.color = Color.green;
            playersInfo[0].PlayerPower.color = Color.red;
        }
        else
        {
            playersInfo[1].PlayerPower.color = Color.black;
            playersInfo[0].PlayerPower.color = Color.black;
        }
    }
}

