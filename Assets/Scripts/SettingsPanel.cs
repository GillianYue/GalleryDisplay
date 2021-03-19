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
        var val = PlayerPrefs.GetFloat("initialZoom", panZoom.zMin); 
        initialZoom.minValue = panZoom.zMin;
        initialZoom.maxValue = panZoom.zMax;

        initialZoom.value = val;
        panZoom.setZ(val);


        panSpeed.minValue = 0.5f;
        panSpeed.maxValue = 3f;

        var pan = PlayerPrefs.GetFloat("panSpeed", 1);
        panSpeed.value = pan;
        panZoom.panSpeed = pan;


        zoomSpeed.minValue = 0.5f;
        zoomSpeed.maxValue = 2f;

        var zoom = PlayerPrefs.GetFloat("zoomSpeed", 1);
        zoomSpeed.value = zoom;
        panZoom.zoomSpeed = zoom;
    }

    public void savePrefs()
    {
        PlayerPrefs.SetFloat("initialZoom", initialZoom.value);
        print("initial zoom set to " + initialZoom.value);

        PlayerPrefs.SetFloat("panSpeed", panSpeed.value);
        PlayerPrefs.SetFloat("zoomSpeed", zoomSpeed.value);
    }

    public void onChangeSliderValue(int sliderIndex)
    {
        switch (sliderIndex)
        {
            case 0:
                panZoom.setZ(initialZoom.value);
                break;
            case 1:
                panZoom.panSpeed = panSpeed.value;
                break;
            case 2:
                panZoom.zoomSpeed = zoomSpeed.value;
                break;
           
        }
    }
}
