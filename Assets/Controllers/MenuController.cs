using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    GameObject playButton;
    Button playButtonScript;

    GameObject multiButton;
    Button multiButtonScript;

    GameObject helpButton;
    Button helpButtonScript;

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

        playButton = GameObject.Find("SoloButton");
        playButtonScript = playButton.GetComponent<Button>();
        playButtonScript.onClick.AddListener(delegate { PlayButton(); });

        multiButton = GameObject.Find("MultiButton");
        multiButtonScript = multiButton.GetComponent<Button>();
        multiButtonScript.onClick.AddListener(delegate { MultiButton(); });

        helpButton = GameObject.Find("HelpButton");
        helpButtonScript = helpButton.GetComponent<Button>();
        helpButtonScript.onClick.AddListener(delegate { ShowGuide1(); });

        guide1Sprite = GameObject.Find("Guide1Sprite");
        guide1Script = guide1Sprite.GetComponent<ButtonScript>();
        guide1Script.ButtonClicked = ShowGuide2;
        //guide1Sprite.SetActive(false);

        guide2Sprite = GameObject.Find("Guide2Sprite");
        guide2Script = guide2Sprite.GetComponent<ButtonScript>();
        guide2Script.ButtonClicked = ShowGuide3;
        //guide2Sprite.SetActive(false);

        guide3Sprite = GameObject.Find("Guide3Sprite");
        guide3Script = guide3Sprite.GetComponent<ButtonScript>();
        guide3Script.ButtonClicked = ShowGuide4;
        //guide3Sprite.SetActive(false);

        guide4Sprite = GameObject.Find("Guide4Sprite");
        guide4Script = guide4Sprite.GetComponent<ButtonScript>();
        guide4Script.ButtonClicked = HideGuides;
        //guide4Sprite.SetActive(false);
    }

    public void PlayButton()
    {
        isMulti = false;
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
    }

    public void MultiButton()
    {
        isMulti = true;
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);        
    }

    public void ShowGuide1()
    {
        guide1Sprite.GetComponent<SpriteRenderer>().enabled = true;
        guide1Sprite.GetComponent<BoxCollider2D>().enabled = true;
        playButton.SetActive(false);
        multiButton.SetActive(false);
        helpButton.SetActive(false);
    }

    public void ShowGuide2(ButtonScript button)
    {
        guide1Sprite.GetComponent<SpriteRenderer>().enabled = false;
        guide1Sprite.GetComponent<BoxCollider2D>().enabled = false;
        guide2Sprite.GetComponent<SpriteRenderer>().enabled = true;
        guide2Sprite.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ShowGuide3(ButtonScript button)
    {
        guide2Sprite.GetComponent<SpriteRenderer>().enabled = false;
        guide2Sprite.GetComponent<BoxCollider2D>().enabled = false;
        guide3Sprite.GetComponent<SpriteRenderer>().enabled = true;
        guide3Sprite.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ShowGuide4(ButtonScript button)
    {
        guide3Sprite.GetComponent<SpriteRenderer>().enabled = false;
        guide3Sprite.GetComponent<BoxCollider2D>().enabled = false;
        guide4Sprite.GetComponent<SpriteRenderer>().enabled = true;
        guide4Sprite.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void HideGuides(ButtonScript button)
    {
        guide4Sprite.GetComponent<SpriteRenderer>().enabled = false;
        guide4Sprite.GetComponent<BoxCollider2D>().enabled = false;
        playButton.SetActive(true);
        multiButton.SetActive(true);
        helpButton.SetActive(true);
    }
}
