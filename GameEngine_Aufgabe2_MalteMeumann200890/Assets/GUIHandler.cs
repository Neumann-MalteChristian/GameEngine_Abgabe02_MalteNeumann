using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject LifeBar;
    [SerializeField]
    private GameObject dashIconBlur;
    [SerializeField]
    private GameObject grapplingHookIconBlur;
    [SerializeField]
    private GameObject score;

    private Image dashBlurImage;
    private Image grapplinghookBlurImage;
    private Image lifeBarImage;
    private Text scoreText;

    private int kills = 0;

     void Start()
    {
        updateScoreText();
    }
    void Awake()
    {
        dashBlurImage = dashIconBlur.GetComponent<Image>();
        grapplinghookBlurImage = grapplingHookIconBlur.GetComponent<Image>();
        lifeBarImage = LifeBar.GetComponent<Image>();
        scoreText = score.GetComponent<Text>();
    }

    

    public void updatedashBlur(float pAmount)
    {
        dashBlurImage.fillAmount=pAmount;
    }
    public void updateGrapplinghookBlur(float pAmount)
    {
       grapplinghookBlurImage.fillAmount = pAmount;
    }

    public void updateLifeBar(float pAmount)
    {
        lifeBarImage.fillAmount=pAmount;
    }
    public void setHealthBarColor(Color pColor)
    {
        lifeBarImage.color = pColor;
    }
    public void increaseScore()
    {
        kills++;
        updateScoreText();
    }
    private void updateScoreText()
    {
        string s = kills + "/18";
        scoreText.text = s;
    }
}
