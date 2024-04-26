using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI loading;
    public void StartGame()=>
        SceneManager.LoadScene("Game");
    
    public void MainMenu() => SceneManager.LoadScene("MainMenu");
    public void QuitGame() => Application.Quit();
    public void CardCollection()
    {
        HideButtons();
        loading.gameObject.SetActive(true);
        LeanTween.delayedCall(1.5f, () =>
        SceneManager.LoadScene("CardEditor"));
    }
    public void HideButtons() => LeanTween.moveY(this.gameObject, -1000f, 1.5f).setEaseInBack();
}
