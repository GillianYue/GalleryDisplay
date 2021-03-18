using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Slider initialZoom, panSpeed, zoomSpeed;
    public PanZoom panZoom;
    public GameObject settingsPanel;

    void Awake()
    {
        InitializeSliderValues();
    }

    void Start()
    {
        closePanel();
    }


    void Update()
    {
        
    }

    public void openPanel()
    {
        settingsPanel.SetActive(true);
        panZoom.checkForPan = false;
    }

    public void closePanel()
    {
        settingsPanel.SetActive(false);
        panZoom.checkForPan = true;
    }

    void InitializeSliderValues()
    {
        print("pp: "+PlayerPrefs.GetFloat("initialZoom", panZoom.zMin));
        var val = PlayerPrefs.GetFloat("initialZoom", panZoom.zMin); 
        initialZoom.minValue = panZoom.zMin;
        initialZoom.maxValue = panZoom.zMax;

        initialZoom.value = val;
        print("initial zoom: " + val);
        panZoom.setZ(val);



    }

    public void savePrefs()
    {
        PlayerPrefs.SetFloat("initialZoom", initialZoom.value);
        print("initial zoom set to " + initialZoom.value);
    }

    public void onChangeSliderValue(int sliderIndex)
    {
        switch (sliderIndex)
        {
            case 0:
                panZoom.setZ(initialZoom.value);
                break;
            case 1:

                break;
            case 2:

                break;
           
        }
    }
}
