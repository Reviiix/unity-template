using System;
using System.Collections.Generic;
using Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    public static RawImage transitionalFadeImage;
    [SerializeField]
    private IntroductionMenu introductionMenu;
    [SerializeField]
    private InGameMenu inGameMenu;
    [SerializeField]
    private PauseMenu pauseMenu;
    [SerializeField]
    private GameOverMenu gameOverMenu;

    private void Start()
    {
        Initialise();
        introductionMenu.EnableIntroductionCanvas();
    }
    
    private void Initialise()
    {
        transitionalFadeImage = GetComponent<RawImage>();
        introductionMenu.Initialise();
        gameOverMenu.Initialise();
        inGameMenu.Initialise();
    }


    public void EnableAllNonPermanentCanvases(bool state)
    {
        introductionMenu.startCanvas.enabled = state;
        gameOverMenu.gameOverCanvas.enabled = state;
        pauseMenu.pauseCanvas.enabled = state;
    }
    
    public void EnableInGameCanvas(bool state)
    {
        inGameMenu.EnableInGameCanvas(state);
    }

    public void EnablePauseCanvas(bool state)
    {
        pauseMenu.EnablePauseCanvas(state);
    }
    
    public void EnablePauseButton(bool state)
    {
        pauseMenu.pauseButtonImage.enabled = false;
    }

    public TMP_Text ReturnScoreText()
    {
        return inGameMenu.scoreText;
    }
    
    public TMP_Text ReturnTimeText()
    {
        return inGameMenu.timeText;
    }
}

[Serializable]
public class IntroductionMenu
{
    [SerializeField]
    private Button startButton;
    public Canvas startCanvas;
    private TMP_Text _startButtonText;

    public void Initialise()
    {
        _startButtonText = startButton.GetComponent<TMP_Text>();
        
        startButton.onClick.AddListener(() => EnableIntroductionCanvas(false));
        
        startButton.onClick.AddListener(GameManager.instance.StartGamePlay);
        
        startButton.onClick.AddListener(GameManager.instance.baseAudioManager.PlayButtonClickSound);
    }

    public void EnableIntroductionCanvas(bool state = true)
    {
        GameManager.instance.userInterface.EnableAllNonPermanentCanvases(false);
        
        startCanvas.enabled = state;
        
        SequentiallyChangeTextColour.StartChangeTextColorSequence(_startButtonText, Color.white, Color.black);
    }
}

[Serializable]
public struct InGameMenu
{
    [SerializeField]
    private Canvas inGameCanvas;
    [SerializeField]
    private Button pauseButton;
    public TMP_Text timeText;
    public TMP_Text scoreText;

    public void Initialise()
    {
        pauseButton.onClick.AddListener(PauseManager.PauseButtonPressed);
        pauseButton.onClick.AddListener(GameManager.instance.baseAudioManager.PlayButtonClickSound);
    }

    public void EnableInGameCanvas(bool state)
    {
        inGameCanvas.enabled = state;
    }
}

[Serializable]
public class PauseMenu
{
    [SerializeField] 
    private Sprite pauseSprite;
    [SerializeField] 
    private Sprite playSprite;
    public Canvas pauseCanvas;
    public Image pauseButtonImage;
    
    public void EnablePauseCanvas(bool state = true)
    {
        GameManager.instance.userInterface.EnableAllNonPermanentCanvases(false);
        
        pauseCanvas.enabled = state;
        pauseButtonImage.sprite = ReturnCurrentPauseButtonSprite(PauseManager.isPaused);
    }

    public Sprite ReturnCurrentPauseButtonSprite(bool state)
    {
        return state ? playSprite : pauseSprite;
    }
}

[Serializable]
public class GameOverMenu
{
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private TMP_Text finalTimeText;
    [SerializeField]
    private TMP_Text finalScoreText;
    [SerializeField]
    private TMP_Text highScoreText;
    public Canvas gameOverCanvas;
    private const string HighScorePrefix = "HIGHSCORE: ";
    private static readonly Color HighScoreColour = Color.green;

    public void Initialise()
    {
        restartButton.onClick.AddListener(GameManager.ReloadGame);
        restartButton.onClick.AddListener(GameManager.instance.baseAudioManager.PlayButtonClickSound);
    }
    
    [ContextMenu("EnableGameOverCanvas")]
    public void EnableGameOverCanvas(bool state, string finalScore = "", string finalTime = "")
    {
        GameManager.instance.userInterface.EnableAllNonPermanentCanvases(false);
        
        gameOverCanvas.enabled = state;

        if (!state) return;

        GameManager.instance.userInterface.EnablePauseButton(false);
        
        finalScoreText.text = finalScore;
        finalTimeText.text = finalTime;
        
        var highScore = HighScores.ReturnHighScore();
        highScoreText.text = HighScorePrefix + highScore;

        if (ScoreTracker.Score >= highScore)
        {
            ChangeTextColors(new [] {highScoreText, finalScoreText}, HighScoreColour);
            return;
        }
        ChangeTextColors(new [] {highScoreText, finalScoreText}, Color.white);
    }
    
    private static void ChangeTextColors(IEnumerable<TMP_Text> textsToChange, Color newColor)
    {
        foreach (var text in textsToChange)
        {
            text.color = newColor;
        }
    }
}
