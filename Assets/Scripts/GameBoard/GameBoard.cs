using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Weather { Blizzard, Fog, Rain }

public class GameBoard : MonoBehaviour
{
    // Singleton Pattern
    public static GameBoard Instance { get; private set; }

    [SerializeField] private Battlefield[] playerBattlefields;
    [SerializeField] private PlayerBoard[] playerBoards;
    [SerializeField] private PlayerInfo[] playerInfos;
    [SerializeField] WeathersRow weathersRow;
    [SerializeField] Button passButton;

    private bool[] isWeatherActive = new bool[3];
    public bool IsWeatherActive(Weather weather) => isWeatherActive[(int)weather];

    //Accesors
    public Battlefield[] PlayerBattlefields => playerBattlefields;
    public PlayerBoard[] PlayerBoards => playerBoards;
    public WeathersRow Weathers => weathersRow;

    // Frontend
    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        for (int i = 0; i < 2; i++)
            playerBoards[i].SetPlayerFaction(i);
    }

    void Start()
    {
        playerInfos[0].playerNick.text = PlayerPrefs.GetString("PlayerOneNick", "Player One").ToUpper();
        playerInfos[1].playerNick.text = PlayerPrefs.GetString("PlayerTwoNick", "Player Two").ToUpper();
    }
    public void HidePlayerHands()
    {
        for (int i = 0; i < 2; i++)
            LeanTween.scaleY(PlayerBoards[i].Hand.gameObject, 0, 0f);
    }

    public void SetActivePlayer(Player currentPlayer, bool IsActive)
    {
        var playerBoard = PlayerBoards[(int)currentPlayer];
        if (IsActive)
        {
            LeanTween.scaleY(playerBoard.Hand.gameObject, 1f, 1f).setEaseOutBack();
            playerBoard.Hand.transform.SetParent(this.transform);

        }
        else
            LeanTween.scaleY(playerBoard.Hand.gameObject, 0f, 1.2f);
    }

    // Behavior
    public void DealCards(Player player, int n) => PlayerBoards[(int)player].DealCards(n);
    public void ResetField()
    {
        for (int i = 0; i < 2; i++)
        {
            playerBattlefields[i].ResetField();
            playerBoards[i].ResetLeaderEffect();
        }
        ResetWeather();

    }
    public void SetWeather(Weather weather)
    {
        isWeatherActive[(int)weather] = true;
        var affectedRow = PlayerBattlefields[0].Rows[(int)weather];
        affectedRow.SetWeather();
        affectedRow = PlayerBattlefields[1].Rows[(int)weather];
        affectedRow.SetWeather();
    }
    public void ResetWeather()
    {
        weathersRow.ClearingAnimation();
        for (int i = 0; i < 3; i++)
        {
            isWeatherActive[i] = false;
            PlayerBattlefields[0].Rows[i].ResetWeather();
            PlayerBattlefields[1].Rows[i].ResetWeather();
        }
    }

    public void ConsumePlayerBattery(Player? winner)
    {
        GameObject battery = playerInfos[(int)winner].Battery[0].gameObject;
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
            var info = playerInfos[i];
            info.CardsInHand.text = playerBoards[i].Hand.transform.childCount.ToString();
            info.PlayerPower.text = playerBattlefields[i].FieldPower.ToString();
        }

        if (playerBattlefields[0].FieldPower > playerBattlefields[1].FieldPower)
        {
            playerInfos[0].PlayerPower.color = Color.green;
            playerInfos[1].PlayerPower.color = Color.red;
        }
        else if (playerBattlefields[0].FieldPower < playerBattlefields[1].FieldPower)
        {
            playerInfos[1].PlayerPower.color = Color.green;
            playerInfos[0].PlayerPower.color = Color.red;
        }
        else
        {
            playerInfos[1].PlayerPower.color = Color.white;
            playerInfos[0].PlayerPower.color = Color.white;
        }
    }
}

