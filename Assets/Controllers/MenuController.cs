using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    GameObject playButton;
    ButtonScript playButtonScript;

    GameObject multiButton;
    ButtonScript multiButtonScript;

    GameObject helpButton;
    ButtonScript helpButtonScript;

    GameObject guide1Sprite;
    ButtonScript guide1Script;
    GameObject guide2Sprite;
    ButtonScript guide2Script;
    GameObject guide3Sprite;
    ButtonScript guide3Script;
    GameObject guide4Sprite;
    ButtonScript guide4Script;

    public bool isMulti;

    // Start is called before the first frame update
    void Start()
    {
        #if !UNITY_WEBGL
            Application.targetFrameRate = 30;
        #endif

        playButton = GameObject.Find("PlayButton");
        playButtonScript = playButton.GetComponent<ButtonScript>();
        playButtonScript.ButtonClicked = PlayButton;

        playButton = GameObject.Find("MultiButton");
        playButtonScript = playButton.GetComponent<ButtonScript>();
        playButtonScript.ButtonClicked = MultiButton;

        helpButton = GameObject.Find("HelpButton");
        helpButtonScript = helpButton.GetComponent<ButtonScript>();
        helpButtonScript.ButtonClicked = ShowGuide1;

        guide1Sprite = GameObject.Find("Guide1Sprite");
        guide1Script = guide1Sprite.GetComponent<ButtonScript>();
        guide1Script.ButtonClicked = ShowGuide2;
        guide1Sprite.SetActive(false);

        guide2Sprite = GameObject.Find("Guide2Sprite");
        guide2Script = guide2Sprite.GetComponent<ButtonScript>();
        guide2Script.ButtonClicked = ShowGuide3;
        guide2Sprite.SetActive(false);

        guide3Sprite = GameObject.Find("Guide3Sprite");
        guide3Script = guide3Sprite.GetComponent<ButtonScript>();
        guide3Script.ButtonClicked = ShowGuide4;
        guide3Sprite.SetActive(false);

        guide4Sprite = GameObject.Find("Guide4Sprite");
        guide4Script = guide4Sprite.GetComponent<ButtonScript>();
        guide4Script.ButtonClicked = HideGuides;
        guide4Sprite.SetActive(false);
    }

    public void PlayButton(ButtonScript button)
    {
        isMulti = false;
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
    }

    public void MultiButton(ButtonScript button)
    {
        isMulti = true;
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);        
    }

    public void ShowGuide1(ButtonScript button)
    {
        guide1Sprite.SetActive(true);
    }

    public void ShowGuide2(ButtonScript button)
    {
        guide1Sprite.SetActive(false);
        guide2Sprite.SetActive(true);
    }

    public void ShowGuide3(ButtonScript button)
    {
        guide2Sprite.SetActive(false);
        guide3Sprite.SetActive(true);
    }

    public void ShowGuide4(ButtonScript button)
    {
        guide3Sprite.SetActive(false);
        guide4Sprite.SetActive(true);
    }

    public void HideGuides(ButtonScript button)
    {
        guide4Sprite.SetActive(false);
    }
}
