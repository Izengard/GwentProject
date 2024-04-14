using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static LeanTween;

public class TurnManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI GameStatusInfo;
    GameBoard gameBoard;
    public TurnPhase CurrentTurnPhase { get; private set; }
    public Player currentPlayer => GameManager.Instance.currentPlayer;
    bool[] PlayersHasPassed = new bool[2];

    void Start()
    {
        gameBoard = GameBoard.Instance;
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
    
    void EndPlayerTurn()
    {
        int enemyPlayer = ((int)currentPlayer + 1) % 2;
        if (!PlayersHasPassed[enemyPlayer])
        {
            gameBoard.SetActivePlayer(currentPlayer, false);
            GameManager.Instance.SetNextPlayer();
            delayedCall(1.5f, () => UpdateTurnPhase(TurnPhase.Play));
        }
        else if (PlayersHasPassed[0] && PlayersHasPassed[1])
            GameManager.Instance.UpdateGameState(GameState.RoundEnd);
        else
            UpdateTurnPhase(TurnPhase.Play);
    }

    public void Pass()
    {
        PlayersHasPassed[(int)currentPlayer] = true;
        UpdateTurnPhase(TurnPhase.TurnEnd);
    }
    public void ResetPass()
    {
        PlayersHasPassed[0] = false;
        PlayersHasPassed[1] = false;
    }
}
