using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Player {
    public Image panel;
    public TMP_Text text;
    public Button button;
}

[System.Serializable]
public class PlayerColor {
    public Color panelColor;
    public Color textColor;
}
public class GameController : MonoBehaviour {
    public TMP_Text[] buttonList;
    
    private string playerSide;
    private string computerSide;
    public bool playerMove;
    public float delay;
    private int value;
    
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    
    private int moveCount;
    
    public GameObject restartButton;
    
    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;
    public GameObject StartInfo;
    
    void Awake() {
      SetGameControllerReferenceOnButtons();
      gameOverPanel.SetActive(false);
      moveCount = 0;
      restartButton.SetActive(false);
      playerMove = true;
    }
 
    public void EndTurn() {
    moveCount++;
    if (
    // Rows
    (buttonList[0].text == computerSide && buttonList[1].text == computerSide && buttonList[2].text == computerSide) ||
    (buttonList[3].text == computerSide && buttonList[4].text == computerSide && buttonList[5].text == computerSide) ||
    (buttonList[6].text == computerSide && buttonList[7].text == computerSide && buttonList[8].text == computerSide) ||

    // Columns
    (buttonList[0].text == computerSide && buttonList[3].text == computerSide && buttonList[6].text == computerSide) ||
    (buttonList[1].text == computerSide && buttonList[4].text == computerSide && buttonList[7].text == computerSide) ||
    (buttonList[2].text == computerSide && buttonList[5].text == computerSide && buttonList[8].text == computerSide) ||

    // Diagonals
    (buttonList[0].text == computerSide && buttonList[4].text == computerSide && buttonList[8].text == computerSide) ||
    (buttonList[2].text == computerSide && buttonList[4].text == computerSide && buttonList[6].text == computerSide)
) {
        GameOver(computerSide);
        return;
    }
       
if (
    // Rows
    (buttonList[0].text == playerSide && buttonList[1].text == playerSide && buttonList[2].text == playerSide) ||
    (buttonList[3].text == playerSide && buttonList[4].text == playerSide && buttonList[5].text == playerSide) ||
    (buttonList[6].text == playerSide && buttonList[7].text == playerSide && buttonList[8].text == playerSide) ||

    // Columns
    (buttonList[0].text == playerSide && buttonList[3].text == playerSide && buttonList[6].text == playerSide) ||
    (buttonList[1].text == playerSide && buttonList[4].text == playerSide && buttonList[7].text == playerSide) ||
    (buttonList[2].text == playerSide && buttonList[5].text == playerSide && buttonList[8].text == playerSide) ||

    // Diagonals
    (buttonList[0].text == playerSide && buttonList[4].text == playerSide && buttonList[8].text == playerSide) ||
    (buttonList[2].text == playerSide && buttonList[4].text == playerSide && buttonList[6].text == playerSide)
) {
        GameOver(playerSide);
        return;
    }   
    if(moveCount >= 9) {
      GameOver("draw");
      return;
    }
    
    ChangeSides();
}
    
    
    void SetPlayersColors(Player newPlayer, Player oldPlayer) {
      newPlayer.panel.color = activePlayerColor.panelColor;
      newPlayer.text.color = activePlayerColor.textColor;
      oldPlayer.panel.color = inactivePlayerColor.panelColor;
      oldPlayer.text.color = inactivePlayerColor.textColor;
    }
    void SetBoardInteractable(bool toggle) {
    
      for(int i = 0; i < buttonList.Length; i++) {
        buttonList[i].GetComponentInParent<Button>().interactable = toggle;
        }
    }
    void GameOver(string winningPlayer) {
      SetBoardInteractable(false);
    
      if(winningPlayer == "draw") {
        SetGameOverText("It's a draw!");
        SetPlayersColorsInactive();
      }
      else {
        SetGameOverText("    " + winningPlayer + " Wins!");
      }
      
      restartButton.SetActive(true);
     }
    
    void ChangeSides() {
      //playerSide = (playerSide == "X") ? "O" : "X";
      playerMove = (playerMove == true) ? false : true;
      if(playerMove == true) {
        SetPlayersColors(playerX, playerO);
      }
      else {
        SetPlayersColors(playerO, playerX);
      }
    }
    
    void SetGameOverText(string value) {
      gameOverPanel.SetActive(true);
      gameOverText.text = value;
    }
    void Update () {
    	if(playerMove == false) {
    		delay += delay * Time.deltaTime;
    		if(delay >= 100) {
    			value = Random.Range(0,8);
    			if(buttonList[value].GetComponentInParent<Button> ().interactable == true) {
    				buttonList[value].text = GetComputerSide ();
    				buttonList[value].GetComponentInParent<Button> ().interactable = false;
    				EndTurn();
    			}    		}
    	}
    }
    void SetGameControllerReferenceOnButtons() {
      for(int i = 0; i < buttonList.Length; i++) {
        buttonList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
      }
    }
    public void SetStartingSide(string startingSide) {
      playerSide = startingSide;
      if(playerSide == "X") {
      computerSide = "O";
        SetPlayersColors(playerX, playerO);
        }
      else {
      	computerSide = "X";
        SetPlayersColors(playerO, playerX);
      }
      StartGame();
    }
    public string GetPlayerSide() {
      return playerSide;
    }
    public string GetComputerSide() {
    	return computerSide;
    }
    void StartGame() {
      SetBoardInteractable(true);
      SetPlayerButtons(false);
      StartInfo.SetActive(false);
    }
    public void RestartGame() {
      moveCount = 0;
      playerSide = "";
      gameOverPanel.SetActive(false);
      restartButton.SetActive(false);
      SetPlayerButtons(true);
      SetPlayersColorsInactive();
      StartInfo.SetActive(true);
      
      for(int i = 0; i < buttonList.Length; i++) {
          buttonList[i].text = "";
      }
      restartButton.SetActive(false);
    }
   
    void SetPlayerButtons(bool toggle){
      playerX.button.interactable = toggle;
      playerO.button.interactable = toggle;
    }
    
      void SetPlayersColorsInactive() {
    playerX.panel.color = inactivePlayerColor.panelColor;
    playerX.text.color = inactivePlayerColor.textColor;
    playerO.panel.color = inactivePlayerColor.panelColor;
    playerO.text.color = inactivePlayerColor.textColor;
    }   
}
