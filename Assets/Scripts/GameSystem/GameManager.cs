using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static LeanTween;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using System.Linq;

public enum GameState { Start, Round, RoundEnd, Victory }
public enum TurnPhase { Draw, Play, Summon, SelectRow, SelectCard, TurnEnd }
public enum Player { Player_One, Player_Two }

public class GameManager : MonoBehaviour
{
    // Singleton Pattern GameManger Instance
    public static GameManager Instance { get; private set; }

    // Frontend Components
    [SerializeField] GameBoard gameBoard;
    [SerializeField] TextMeshProUGUI GameStatusInfo;
    [SerializeField] GameObject EventDialogBox;
    [SerializeField] TextMeshProUGUI EventText;
    [SerializeField] GameObject VictoryPanel;
    [SerializeField] TextMeshProUGUI VictoryText;
    [SerializeField] InitialHandPanel InitPanel;
    public InitialHandPanel InitialHandPanel => InitPanel;


    // Logic fields
    public GameState GameState { get; private set; }
    public TurnPhase CurrentTurnPhase { get; private set; }
    public Player currentPlayer { get; private set; }
    int RoundCount = 0;
    int[] VictoryPoints = { 2, 2 };
    [SerializeField] bool[] PlayersHasPassed = new bool[2];

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Start()
    {
        UpdateGameState(GameState.Start);
    }

    // State Management
    public void UpdateGameState(GameState newState)
    {
        this.GameState = newState;
        HandleState(newState);
    }

    private void HandleState(GameState newGameState)
    {
        Debug.Log(newGameState);
        switch (newGameState)
        {
            case GameState.Start:
                GameState = GameState.Start;
                HandleStart();
                break;
            case GameState.Round:
                GameState = GameState.Round;
                Round();
                break;
            case GameState.RoundEnd:
                GameState = GameState.Round;
                EndRound();
                break;
            case GameState.Victory:
                GameState = GameState.Round;
                Victory();
                break;
        }
        Debug.Log(GameState);
    }

    void DisplayDialogMessage(string text)
    {
        EventDialogBox.SetActive(true);
        EventDialogBox.transform.localScale = new Vector2(0, 0);
        scale(EventDialogBox, new Vector2(1f, 1f), 1.5f).setEaseOutQuad();
        delayedCall(2.4f, () => scale(EventDialogBox, new Vector2(0f, 0f), 1.5f))
            .setEaseInBounce()
            .setOnComplete(() => EventDialogBox.SetActive(false));
        EventText.text = text;
    }

    void HandleStart()
    {
        InitPanel.gameObject.SetActive(true);
        int firstPlayer = UnityEngine.Random.Range(0, 2);
        currentPlayer = (Player)firstPlayer;
        Debug.Log($"FirstPlayer is {currentPlayer}");
    }

    void Round()
    {
        if (RoundCount == 0)
            gameBoard.HidePlayerBoards();

        DisplayDialogMessage($"{currentPlayer} Starts the Round");

        if (RoundCount != 0)
        {
            UpdateTurnPhase(TurnPhase.Draw);
            gameBoard.DealCards(currentPlayer, 2);
            int enemyPlayer = ((int)currentPlayer + 1) % 2;
            gameBoard.DealCards((Player)enemyPlayer, 2);
        }
        UpdateTurnPhase(TurnPhase.Play);
    }

    void EndRound()
    {
        var winner = DetermineRoundWinner();
        if (winner is not null)
        {
            var loser = ((int)winner + 1) % 2;
            VictoryPoints[loser]--;
            gameBoard.ConsumePlayerBattery((Player)loser);
            currentPlayer = (Player)winner;
        }
        else
            foreach (Player player in Enum.GetValues(typeof(Player)))
            {
                gameBoard.ConsumePlayerBattery(player);
                VictoryPoints[(int)player]--;
            }

        if (VictoryPoints[0] == 0 || VictoryPoints[1] == 0)
            delayedCall(2f,()=>
                UpdateGameState(GameState.Victory));

        RoundCount++;
        CardManager.Instance.ResetField();
        gameBoard.ResetField();
        PlayersHasPassed[0] = false;
        PlayersHasPassed[1] = false;

        Debug.Log(RoundCount);
        delayedCall(2.5f, () =>
        {
            if (VictoryPoints[0] == 0 || VictoryPoints[1] == 0)
                UpdateGameState(GameState.Victory);
            else
                UpdateGameState(GameState.Round);
        });
    }

    public Player? DetermineRoundWinner()
    {
        Player? winner;

        Debug.Log($"{gameBoard.PlayerBattlefield[0].FieldPower} vs {gameBoard.PlayerBattlefield[1].FieldPower}");
        if (gameBoard.PlayerBattlefield[0].FieldPower > gameBoard.PlayerBattlefield[1].FieldPower)
        {
            winner = Player.Player_One;
            DisplayDialogMessage($"{winner.ToString().Replace('_', ' ')} has won the Round");
        }
        else if (gameBoard.PlayerBattlefield[0].FieldPower < gameBoard.PlayerBattlefield[1].FieldPower)
        {
            winner = Player.Player_Two;
            DisplayDialogMessage($"{winner.ToString().Replace('_', ' ')} has won the Round");
        }
        else
        {
            winner = null;
            DisplayDialogMessage($"The Round ended in DRAW");
        }
        Debug.Log($"{winner}");

        return winner;
    }

    public void Victory()
    {
        VictoryPanel.SetActive(true);
        // scale(VictoryPanel, Vector3.zero, 2f)
        // .setOnComplete(() =>
        scale(VictoryPanel, Vector3.one, 2f);

        if (VictoryPoints.Sum() == 0)
            VictoryText.text = $"Game is Draw";
        else if (VictoryPoints[0] != 0)
            VictoryText.text = $"Player One Has Won the Game";
        else if (VictoryPoints[1] != 0)
            VictoryText.text = $"Player Two Has Won the Game";

        delayedCall(3f, () =>
        SceneManager.LoadScene("MainMenu"));
    }






    public void UpdateTurnPhase(TurnPhase newPhase)
    {
        CurrentTurnPhase = newPhase;
        HandleTurnPhase(newPhase);
    }

    private void HandleTurnPhase(TurnPhase newPhase)
    {
        Debug.Log($"{newPhase}");
        switch (newPhase)
        {
            case TurnPhase.Draw:
                CurrentTurnPhase = TurnPhase.Draw;
                break;
            case TurnPhase.Play:
                CurrentTurnPhase = TurnPhase.Play;
                GameStatusInfo.text = $"{currentPlayer.ToString().Replace('_', ' ')} turn, make a play!";
                gameBoard.SetActivePlayer(currentPlayer, true);
                break;
            case TurnPhase.Summon:
                CurrentTurnPhase = TurnPhase.Summon;
                break;
            case TurnPhase.SelectRow:
                GameStatusInfo.text = $"Select a Row to summon your card";
                CurrentTurnPhase = TurnPhase.SelectRow;
                break;
            case TurnPhase.SelectCard:
                GameStatusInfo.text = $"Select a card";
                CurrentTurnPhase = TurnPhase.SelectCard;
                break;
            case TurnPhase.TurnEnd:
                CurrentTurnPhase = TurnPhase.TurnEnd;
                EndPlayerTurn();
                break;
        }
        Debug.Log($"{CurrentTurnPhase}");

    }
    public void WaitForRowSelection() => UpdateTurnPhase(TurnPhase.SelectRow);
    public void WaitForCardSelection() => UpdateTurnPhase(TurnPhase.SelectCard);
    void EndPlayerTurn()
    {
        int enemyPlayer = ((int)currentPlayer + 1) % 2;
        if (!PlayersHasPassed[enemyPlayer])
        {
            gameBoard.SetActivePlayer(currentPlayer, false);
            currentPlayer = (Player)enemyPlayer;
            delayedCall(1.5f, () => UpdateTurnPhase(TurnPhase.Play));
        }
        else if (PlayersHasPassed[0] && PlayersHasPassed[1])
            UpdateGameState(GameState.RoundEnd);
        else
            UpdateTurnPhase(TurnPhase.Play);
    }

    public void Pass()
    {
        PlayersHasPassed[(int)currentPlayer] = true;
        UpdateTurnPhase(TurnPhase.TurnEnd);
    }
}
