using System;
using PureFunctions;
using Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    public static RawImage TransitionalFadeImage;
    private static readonly Color[] PlayButtonColorSwaps = {Color.white, Color.green};
    [SerializeField]
    private IntroductionMenu introductionMenu;
    [SerializeField]
    private InGameMenu inGameMenu;
    [SerializeField]
    private PauseMenu pauseMenu;
    [SerializeField]
    private GameOverMenu gameOverMenu;
    [SerializeField]
    private SettingsMenu settingsMenu;

    private void Start()
    {
        Initialise();
        
        EnableStartMenu();
    }

    private void Initialise()
    {
        ResolveDependencies();

        InitialiseMenus();
    }

    private void InitialiseMenus()
    {
        introductionMenu.Initialise(()=>EnableAllNonPermanentCanvases(false), PlayButtonColorSwaps);
        gameOverMenu.Initialise(PlayButtonColorSwaps);
        pauseMenu.Initialise(PauseButtonPressed);
        settingsMenu.Initialise(()=> EnablePauseButton(), () => EnablePauseButton(false),()=>EnablePauseMenu());
    }

    private void ResolveDependencies()
    {
        TransitionalFadeImage = GetComponent<RawImage>();
    }

    public void EnableStartMenu()
    {
        introductionMenu.Enable();
    }

    public void EnableInGameUserInterface(bool state = true)
    {
        inGameMenu.Enable(state);
    }

    private void PauseButtonPressed()
    {
        EnablePauseMenu(PauseManager.IsPaused);
    }

    private void EnablePauseMenu(bool state = true)
    {
        pauseMenu.Enable(state);
    }
    
    public void EnableGameOverMenu(bool state = true)
    {
        gameOverMenu.Enable(state);
    }
    
    public void EnablePauseButton(bool state = true)
    {
        pauseMenu.EnablePauseButtons(state);
    }
    
    private void EnableAllNonPermanentCanvases(bool state = true)
    {
        introductionMenu.display.enabled = state;
        gameOverMenu.display.enabled = state;
        pauseMenu.display.enabled = state;
        settingsMenu.display.enabled = state;
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
public class IntroductionMenu : Menu, IMenu
{
    [SerializeField]
    private Button startButton;
    private TMP_Text _startButtonText;
    private static Color[] _playButtonColorSwaps;

    public void Initialise(Action disableAllCanvases, Color[] playButtonColorSwaps)
    {
        ResolveDependencies(playButtonColorSwaps, disableAllCanvases);
        
        AddButtonEvents();
    }
    
    private void ResolveDependencies(Color[] playButtonColorSwaps, Action disableAllCanvases)
    {
        _startButtonText = startButton.GetComponent<TMP_Text>();
        _playButtonColorSwaps = playButtonColorSwaps;
        DisableAllCanvases = disableAllCanvases;
    }

    private void AddButtonEvents()
    {
        startButton.onClick.AddListener(() => Enable(false));
        startButton.onClick.AddListener(ProjectManager.Instance.LoadMainGame);
        startButton.onClick.AddListener(ProjectManager.Instance.audioManager.PlayButtonClickSound);
        startButton.onClick.AddListener(SequentiallyChangeTextColour.StopChangeTextColorSequence);
    }

    public void Enable(bool state = true)
    {
        DisableAllCanvases();
        ProjectManager.Instance.userInterface.EnablePauseButton();
        
        display.enabled = state;
        
        SequentiallyChangeTextColour.Change(_startButtonText, _playButtonColorSwaps[0], _playButtonColorSwaps[1], ProjectManager.Instance);
    }
}

[Serializable]
public class InGameMenu : Menu, IMenu
{
    public TMP_Text timeText;
    public TMP_Text scoreText;

    public void Enable(bool state = true)
    {
        display.enabled = state;
    }
}

[Serializable]
public class PauseMenu : Menu, IMenu
{
    [SerializeField] 
    private Sprite pauseSprite;
    [SerializeField] 
    private Sprite playSprite;
    [SerializeField] 
    private Image pauseButtonImage;
    private Button _pauseButton;

    public void Initialise(Action enablePauseCanvas)
    {
        ResolveDependencies();
        AddButtonEvents(enablePauseCanvas);
    }
    
    private void ResolveDependencies()
    {
        _pauseButton = pauseButtonImage.GetComponent<Button>();
    }

    private void AddButtonEvents(Action enablePauseCanvas)
    {
        _pauseButton.onClick.AddListener(PauseManager.PauseButtonPressed);
        _pauseButton.onClick.AddListener(()=>enablePauseCanvas());
        _pauseButton.onClick.AddListener(ProjectManager.Instance.audioManager.PlayButtonClickSound);
    }

    public void EnablePauseButtons(bool state = true)
    {
        pauseButtonImage.enabled = state;
        _pauseButton.enabled = state;
    }

    public void Enable(bool state = true)
    {
        DisableAllCanvases();
        
        display.enabled = state;
        pauseButtonImage.sprite = ReturnCurrentPauseButtonSprite(state);
    }

    public Sprite ReturnCurrentPauseButtonSprite(bool state = true)
    {
        return state ? playSprite : pauseSprite;
    }
}

[Serializable]
public class GameOverMenu : Menu, IMenu
{
    private static Color[] _restartButtonColorSwaps;
    private const string FinalScorePrefix = "FINAL " + ScoreTracker.ScoreDisplayPrefix;
    private const string HighScorePrefix = "HIGH" + ScoreTracker.ScoreDisplayPrefix;
    private const string FinalTimePrefix = "FINAL " + TimeTracker.TimeDisplayPrefix;
    private static readonly Color NewHighScoreTextColour = Color.green;
    private static Color _noHighScoreColor = Color.white;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private TMP_Text finalTimeText;
    [SerializeField]
    private TMP_Text finalScoreText;
    [SerializeField]
    private TMP_Text highScoreText;

    public void Initialise(Color[] restartButtonColorSwaps)
    {
        SetNoHighScoreColor(restartButtonColorSwaps);
        AddButtonEvents();
    }

    private void SetNoHighScoreColor(Color[] restartButtonColorSwaps)
    {
        _restartButtonColorSwaps = restartButtonColorSwaps;
        _noHighScoreColor = highScoreText.color;
    }

    private void AddButtonEvents()
    {
        restartButton.onClick.AddListener(ProjectManager.Instance.LoadMainMenu);
        restartButton.onClick.AddListener(ProjectManager.Instance.audioManager.PlayButtonClickSound);
        restartButton.onClick.AddListener(SequentiallyChangeTextColour.StopChangeTextColorSequence);
    }

    public void Enable(bool state = true)
    {
        DisableAllCanvases();
        ProjectManager.Instance.userInterface.EnableInGameUserInterface(false);
        display.enabled = state;

        if (!state) return;
        
        SequentiallyChangeTextColour.Change(restartButton.GetComponent<TMP_Text>(), _restartButtonColorSwaps[0], _restartButtonColorSwaps[1], ProjectManager.Instance);
        SequentiallyChangeTextColour.Change(restartButton.GetComponent<TMP_Text>(), _restartButtonColorSwaps[0], _restartButtonColorSwaps[1], ProjectManager.Instance);
        
        ProjectManager.Instance.userInterface.EnablePauseButton(false);
        SetFinalStatisticsDisplay();
    }

    private void SetFinalStatisticsDisplay()
    {
        var score = ScoreTracker.Score;
        var highScore = HighScores.ReturnHighScore();
        var finalTime = TimeTracker.ReturnCurrentTimeAsFormattedString();
        
        finalScoreText.text = FinalScorePrefix + score;
        finalTimeText.text = FinalTimePrefix + finalTime;
        highScoreText.text = HighScorePrefix + highScore;

        if (score >= highScore)
        {
            SetHighScoreColours();
            return;
        }
        SetHighScoreColours(false);
    }

    private void SetHighScoreColours(bool highScoreAchieved = true)
    {
        if (highScoreAchieved)
        {
            ChangeTextColors.Change(new [] {highScoreText, finalScoreText}, NewHighScoreTextColour);
            return;
        }
        ChangeTextColors.Change(new [] {highScoreText, finalScoreText}, _noHighScoreColor);
    }
}

[Serializable]
public class SettingsMenu : Menu, IMenu
{
    private Button _settingsButton;
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private GameObject settingsButtonObject;

    public void Initialise(Action enablePauseButton, Action disablePauseButton, Action enablePauseMenu)
    {
        ResolveDependencies();
        
        AddButtonEvents(enablePauseButton, disablePauseButton, enablePauseMenu);
    }

    private void ResolveDependencies()
    {
        _settingsButton = settingsButtonObject.GetComponent<Button>();
    }

    private void AddButtonEvents(Action enablePauseButton, Action disablePauseButton, Action enablePauseMenu)
    {
        closeButton.onClick.AddListener(() => Enable(false));
        closeButton.onClick.AddListener(() => enablePauseButton());
        closeButton.onClick.AddListener(() => enablePauseMenu());
        closeButton.onClick.AddListener(ProjectManager.Instance.audioManager.PlayButtonClickSound);
        
        _settingsButton.onClick.AddListener(() => Enable());
        _settingsButton.onClick.AddListener(() => disablePauseButton());
        _settingsButton.onClick.AddListener(ProjectManager.Instance.audioManager.PlayButtonClickSound);
    }

    public void Enable(bool state = true)
    {
        DisableAllCanvases();
        
        display.enabled = state;
        
        settingsButtonObject.SetActive(!state);
    }
}

[Serializable]
public abstract class Menu
{
    public Canvas display;
    protected static Action DisableAllCanvases;
}

public interface IMenu
{
    void Enable(bool state = true);
}
