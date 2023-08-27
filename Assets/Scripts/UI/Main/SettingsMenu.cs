using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Main.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private Toggle vsyncToggle;

        [SerializeField]
        private TMP_Dropdown resolutionDropdown;

        [SerializeField]
        private GameObject FPSCounter;

        private Resolution[] resolutions;
        private List<Resolution> filteredResolutions;
        private float currentRefreshRate;
        private int currentResolutionIndex = 0;

        private void Start()
        {
            // Load the previous value of VSync from PlayerPrefs and set the toggle accordingly
            vsyncToggle.isOn = PlayerPrefs.GetInt("VSyncEnabled", 0) == 1;

            InitResolutions();
        }

        public void OnVSyncToggle()
        {
            // Store the value of VSync in PlayerPrefs
            int vsyncValue = vsyncToggle.isOn ? 1 : 0;
            PlayerPrefs.SetInt("VSyncEnabled", vsyncValue);
            PlayerPrefs.Save();
        }

        private void InitResolutions()
        {
            resolutions = Screen.resolutions;
            filteredResolutions = new List<Resolution>();

            resolutionDropdown.ClearOptions();
            currentRefreshRate = Screen.currentResolution.refreshRate;

            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].refreshRate == currentRefreshRate)
                {
                    filteredResolutions.Add(resolutions[i]);
                }
            }

            List<string> options = new List<string>();

            for (int i = 0; i < filteredResolutions.Count; i++)
            {
                string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRate + " Hz";
                options.Add(resolutionOption);
                if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = filteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, true);
        }

        public void SetFullscreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
        }

        public void ToggleFPSCounter(bool setFPS)
        {
            FPSCounter.SetActive(setFPS);
        }
    }
}
