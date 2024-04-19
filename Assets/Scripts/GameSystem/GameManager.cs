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
using UnityEngine.SceneManagement;
using System.Linq;

public enum GameState { Start, Round, RoundEnd, Victory }
public enum Player { PlayerOne, PlayerTwo }

public class GameManager : MonoBehaviour
{
    // Singleton Pattern GameManger Instance
    public static GameManager Instance { get; private set; }

    // Frontend Components
    [SerializeField] GameBoard gameBoard;
    [SerializeField] GameObject EventDialogBox;
    [SerializeField] TextMeshProUGUI EventText;
    [SerializeField] GameObject VictoryPanel;
    [SerializeField] TextMeshProUGUI VictoryText;
    [SerializeField] TurnManager turnManager;


    // Logic fields
    [field: SerializeField] public GameState GameState { get; private set; }
    [field: SerializeField] public Player currentPlayer { get; private set; }
    [SerializeField] public TurnPhase CurrentTurnPhase => turnManager.CurrentTurnPhase;
    int RoundCount = 0;
    int[] VictoryPoints = { 2, 2 };
    string[] PlayerNames;
    public string currentPlayerName => PlayerNames[(int)currentPlayer];


    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        PlayerNames = new string[2]{PlayerPrefs.GetString("PlayerOneNick", "Player One"),
                            PlayerPrefs.GetString("PlayerTwoNick", "Player Two")};

        UpdateGameState(GameState.Start);
    }
    void Start()
    {
        gameBoard = GameBoard.Instance;
        if (gameBoard is null) Debug.Log($"Problem with gameboard");
    }

    void DisplayDialogMessage(string text)
    {
        EventDialogBox.SetActive(true);
        EventDialogBox.transform.localScale = new Vector2(0, 0);
        scale(EventDialogBox, new Vector2(1f, 1f), 1.5f).setEaseOutQuad();
        delayedCall(2f, () => scale(EventDialogBox, new Vector2(0f, 0f), 1.5f))
            .setEaseInBounce()
            .setOnComplete(() => EventDialogBox.SetActive(false));
        EventText.text = text;
    }

    public void SetNextPlayer()
    {
        int enemyPlayer = ((int)currentPlayer + 1) % 2;
        currentPlayer = (Player)enemyPlayer;
    }

    public void UpdateTurnPhase(TurnPhase newTurnPhase, float timeDelay = 0f) =>
         turnManager.UpdateTurnPhase(newTurnPhase, timeDelay);

    public void WaitForRowSelection() => UpdateTurnPhase(TurnPhase.SelectRow);
    public void WaitForCardSelection() => UpdateTurnPhase(TurnPhase.SelectCard);


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

    void HandleStart()
    {
        int firstPlayer = UnityEngine.Random.Range(0, 2);
        currentPlayer = (Player)firstPlayer;
        Debug.Log($"FirstPlayer is {currentPlayerName}");
    }

    void Round()
    {
        gameBoard.HidePlayerHands();
        DisplayDialogMessage($"{currentPlayerName} Starts the Round");

        if (RoundCount != 0)
            UpdateTurnPhase(TurnPhase.Draw, 1.5f);

        UpdateTurnPhase(TurnPhase.Play, 1.6f);
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

        RoundCount++;
        CardManager.Instance.ResetField();
        gameBoard.ResetField();
        turnManager.ResetPass();

        Debug.Log(RoundCount);
        delayedCall(2.5f, () =>
        {
            if (VictoryPoints[0] == 0 || VictoryPoints[1] == 0)
                UpdateGameState(GameState.Victory);
            else
                UpdateGameState(GameState.Round);
        });
    }

    Player? DetermineRoundWinner()
    {
        Player? winner;

        Debug.Log($"{gameBoard.PlayerBattlefields[0].FieldPower} vs {gameBoard.PlayerBattlefields[1].FieldPower}");
        if (gameBoard.PlayerBattlefields[0].FieldPower > gameBoard.PlayerBattlefields[1].FieldPower)
        {
            winner = Player.PlayerOne;
            DisplayDialogMessage($"{PlayerNames[0]} has won the Round");
        }
        else if (gameBoard.PlayerBattlefields[0].FieldPower < gameBoard.PlayerBattlefields[1].FieldPower)
        {
            winner = Player.PlayerTwo;
            DisplayDialogMessage($"{PlayerNames[1]} has won the Round");
        }
        else
        {
            winner = null;
            DisplayDialogMessage($"The Round ended in DRAW");
        }
        Debug.Log($"{winner}");

        return winner;
    }

    void Victory()
    {
        VictoryPanel.SetActive(true);
        scale(VictoryPanel, new Vector3(1f, 1f, 1f), 1f);

        if (VictoryPoints.Sum() == 0)
            VictoryText.text = $"Game is Draw";
        else if (VictoryPoints[0] != 0)
            VictoryText.text = $"{PlayerNames[0]} Has Won the Game";
        else if (VictoryPoints[1] != 0)
            VictoryText.text = $"{PlayerNames[1]} Has Won the Game";

        delayedCall(3.5f, () =>
        SceneManager.LoadScene("MainMenu"));
    }
}