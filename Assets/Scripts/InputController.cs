using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class InputController : MonoBehaviour
{
    [SerializeField] private Text SolText = null;

    [SerializeField] private List<int> SolutionList = new List<int>();
    [SerializeField] private Transform PngParent = null;

    [SerializeField] private GameObject GameCanvas = null;
    [SerializeField] private GameObject VictoryCanvas = null;
    [SerializeField] private GameObject MainMenuCanvas = null;
    [SerializeField] private GameObject LevelMenuCanvas = null;
    [SerializeField] private GameObject FinalVictoryCanvas = null;
    [SerializeField] private GameObject AboutCanvas = null;

    [SerializeField] private AudioSource TapSound = null;
    [SerializeField] private AudioSource RightSound = null;
    [SerializeField] private AudioSource WrongSound = null;

    [SerializeField] private Camera Cam = null;

    [SerializeField] private float CameraShakeDuration = 0.1f;
    [SerializeField] private float CameraShakeAmount = 0.7f;
    
    private int CurrentLevel = 0;
    private int EnteredInput = 0;
    private bool VictoryPageActive;
    private float CurrentShakeDuration = 0f;
    private Vector3 OriPos = new Vector3();
    
    private void Start()
    {
        CurrentLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        UpdateLevelUI();
        VictoryPageActive = false;
        OriPos = Cam.transform.position;
    }

    private void Update()
    {
        if(CurrentShakeDuration > 0f)
        {
            Cam.transform.position = OriPos + Random.insideUnitSphere * CameraShakeAmount;
            CurrentShakeDuration -= Time.deltaTime;
        }
    }

    private void UpdateLevelUI()
    {
        if (PlayerPrefs.GetInt("UnlockedLevel") > 17)
            PlayerPrefs.SetInt("UnlockedLevel", 17);

        for( int i = 0; i < PngParent.childCount; i++)
        {
            LevelMenuCanvas.transform.GetChild(1).transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);

            var col = LevelMenuCanvas.transform.GetChild(0).transform.GetChild(i).GetComponent<Image>().color;
            col.a = 0.2f;
            LevelMenuCanvas.transform.GetChild(0).transform.GetChild(i).GetComponent<Image>().color = col;
        }

        for(int i = 0; i < PlayerPrefs.GetInt("UnlockedLevel", CurrentLevel); i++)
        {
            LevelMenuCanvas.transform.GetChild(1).transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(true);

            var col = LevelMenuCanvas.transform.GetChild(0).transform.GetChild(i).GetComponent<Image>().color;
            col.a = 1f;
            LevelMenuCanvas.transform.GetChild(0).transform.GetChild(i).GetComponent<Image>().color = col;
        }
    }

    private void AddUnlockedLevelUI(int level)
    {
        LevelMenuCanvas.transform.GetChild(1).transform.GetChild(0).transform.GetChild(level).gameObject.SetActive(true);

        var col = LevelMenuCanvas.transform.GetChild(0).transform.GetChild(level).GetComponent<Image>().color;
        col.a = 1f;
        LevelMenuCanvas.transform.GetChild(0).transform.GetChild(level).GetComponent<Image>().color = col;
    }

    public void CheckSol()
    {
        if (!VictoryPageActive)
        {
            if (EnteredInput == SolutionList[CurrentLevel - 1])
            {
                RightSound.Play();
                if (PngParent.childCount > 0)
                {
                    Color temp = PngParent.GetChild(CurrentLevel - 1).GetComponent<Image>().color;
                    temp.a = 0.2f;
                    PngParent.GetChild(CurrentLevel - 1).GetComponent<Image>().color = temp;
                    GameCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                }

                SolText.text = "";
                
                if(CurrentLevel == PngParent.childCount)
                {
                    VictoryPageActive = true;
                    FinalVictoryCanvas.SetActive(true);
                }
                else
                {
                    VictoryPageActive = true;
                    VictoryCanvas.SetActive(true);
                }

                if (CurrentLevel >= PlayerPrefs.GetInt("UnlockedLevel", 1) && CurrentLevel <= PngParent.childCount)
                {
                    PlayerPrefs.SetInt("UnlockedLevel", CurrentLevel + 1);
                    UpdateLevelUI();
                }
            }
            else
            {
                WrongSound.Play();
                EnteredInput = 0;
                SolText.text = "";
                CurrentShakeDuration = CameraShakeDuration;
                StartCoroutine(StopShaking());
            }
        }
    }

    public void PlayNewLevel()
    {
        CurrentLevel++;
        VictoryPageActive = false;
        VictoryCanvas.SetActive(false);

        Color temp = PngParent.GetChild(CurrentLevel - 1).GetComponent<Image>().color;
        temp.a = 1f;
        PngParent.GetChild(CurrentLevel - 2).GetComponent<Image>().color = temp;
        PngParent.GetChild(CurrentLevel - 2).gameObject.SetActive(false);

        EnteredInput = 0;
        SolText.text = "";
        GameCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        GameCanvas.SetActive(true);
        PngParent.GetChild(CurrentLevel - 1).gameObject.SetActive(true);
    }

    private void PlayParticularLevel(int level)
    {
        CurrentLevel = level;
        LevelMenuCanvas.SetActive(false);
        VictoryPageActive = false;
        VictoryCanvas.SetActive(false);

        Color temp = PngParent.GetChild(CurrentLevel - 1).GetComponent<Image>().color;
        temp.a = 1f;

        EnteredInput = 0;
        SolText.text = "";
        GameCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        GameCanvas.SetActive(true);
        PngParent.GetChild(CurrentLevel - 1).gameObject.SetActive(true);
    }

    public void ContinueLatestLevel()
    {
        GameCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        TapSound.Play();
        VictoryPageActive = false;
        MainMenuCanvas.SetActive(false);
        VictoryCanvas.SetActive(false);
        FinalVictoryCanvas.SetActive(false);
        EnteredInput = 0;
        SolText.text = "";
        GameCanvas.SetActive(true);
        CurrentLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (PngParent.childCount > 0)
            PngParent.GetChild(CurrentLevel - 1).gameObject.SetActive(true);
    }
    
    public void MainMenuBack()
    {
        TapSound.Play();
        Color temp = PngParent.GetChild(CurrentLevel - 1).GetComponent<Image>().color;
        temp.a = 255;
        PngParent.GetChild(CurrentLevel - 1).GetComponent<Image>().color = temp;
        PngParent.GetChild(CurrentLevel - 1).gameObject.SetActive(false);

        VictoryPageActive = false;
        GameCanvas.SetActive(false);
        VictoryCanvas.SetActive(false);
        FinalVictoryCanvas.SetActive(false);
        AboutCanvas.SetActive(false);
        LevelMenuCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
        if (PngParent.childCount > 0)
            PngParent.GetChild(CurrentLevel - 1).gameObject.SetActive(false);
    }
    
    public void GameMenuBack()
    {
        if(!VictoryPageActive)
        {
            TapSound.Play();
            Color temp = PngParent.GetChild(CurrentLevel - 1).GetComponent<Image>().color;
            temp.a = 255;
            PngParent.GetChild(CurrentLevel - 1).GetComponent<Image>().color = temp;
            PngParent.GetChild(CurrentLevel - 1).gameObject.SetActive(false);

            VictoryPageActive = false;
            GameCanvas.SetActive(false);
            VictoryCanvas.SetActive(false);
            FinalVictoryCanvas.SetActive(false);
            LevelMenuCanvas.SetActive(false);
            MainMenuCanvas.SetActive(true);
            if (PngParent.childCount > 0)
                PngParent.GetChild(CurrentLevel - 1).gameObject.SetActive(false);
        }
    }

    public void LoadLevelMenu()
    {
        TapSound.Play();
        VictoryPageActive = false;
        MainMenuCanvas.SetActive(false);
        EnteredInput = 0;
        SolText.text = "";
        LevelMenuCanvas.SetActive(true);
        UpdateLevelUI();
    }

    public void LoadAbout()
    {
        TapSound.Play();
        MainMenuCanvas.SetActive(false);
        AboutCanvas.SetActive(true);
    }

    public void ResetLevelButton()
    {
        TapSound.Play();
        CurrentLevel = 1;
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        UpdateLevelUI();
    }

    public void PlayLevel1()
    {
        TapSound.Play();
        PlayParticularLevel(1);
    }

    public void PlayLevel2()
    {
        TapSound.Play();
        PlayParticularLevel(2);
    }

    public void PlayLevel3()
    {
        TapSound.Play();
        PlayParticularLevel(3);
    }

    public void PlayLevel4()
    {
        TapSound.Play();
        PlayParticularLevel(4);
    }

    public void PlayLevel5()
    {
        TapSound.Play();
        PlayParticularLevel(5);
    }

    public void PlayLevel6()
    {
        TapSound.Play();
        PlayParticularLevel(6);
    }

    public void PlayLevel7()
    {
        TapSound.Play();
        PlayParticularLevel(7);
    }

    public void PlayLevel8()
    {
        TapSound.Play();
        PlayParticularLevel(8);
    }

    public void PlayLevel9()
    {
        TapSound.Play();
        PlayParticularLevel(9);
    }

    public void PlayLevel10()
    {
        TapSound.Play();
        PlayParticularLevel(10);
    }

    public void PlayLevel11()
    {
        TapSound.Play();
        PlayParticularLevel(11);
    }

    public void PlayLevel12()
    {
        TapSound.Play();
        PlayParticularLevel(12);
    }

    public void PlayLevel13()
    {
        TapSound.Play();
        PlayParticularLevel(13);
    }

    public void PlayLevel14()
    {
        TapSound.Play();
        PlayParticularLevel(14);
    }

    public void PlayLevel15()
    {
        TapSound.Play();
        PlayParticularLevel(15);
    }

    public void PlayLevel16()
    {
        TapSound.Play();
        PlayParticularLevel(16);
    }

    public void PlayLevel17()
    {
        TapSound.Play();
        PlayParticularLevel(17);
    }

    public void Pressed1()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput * 10 + 1;
            if (EnteredInput > 999999)
                EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    public void Pressed2()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput * 10 + 2;
            if (EnteredInput > 999999)
                EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    public void Pressed3()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput * 10 + 3;
            if (EnteredInput > 999999)
                EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    public void Pressed4()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput * 10 + 4;
            if (EnteredInput > 999999)
                EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    public void Pressed5()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput * 10 + 5;
            if (EnteredInput > 999999)
                EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    public void Pressed6()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput * 10 + 6;
            if (EnteredInput > 999999)
                EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    public void Pressed7()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput * 10 + 7;
            if (EnteredInput > 999999)
                EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    public void Pressed8()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput * 10 + 8;
            if (EnteredInput > 999999)
                EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    public void Pressed9()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput * 10 + 9;
            if (EnteredInput > 999999)
                EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    public void Pressed0()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput * 10;
            if (EnteredInput > 999999)
                EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    public void BackSpace()
    {
        if (!VictoryPageActive)
        {
            TapSound.Play();
            EnteredInput = EnteredInput / 10;
            UpdateSolText();
        }
    }

    private void UpdateSolText()
    {
        SolText.text = EnteredInput.ToString();
    }

    IEnumerator StopShaking()
    {
        yield return new WaitForSeconds(CameraShakeDuration + 0.01f);
        Cam.transform.position = OriPos;
    }
}