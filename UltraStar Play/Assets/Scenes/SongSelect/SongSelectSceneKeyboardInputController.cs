﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSelectSceneKeyboardInputController : MonoBehaviour
{
    private const KeyCode NextSongShortcut = KeyCode.RightArrow;
    private const KeyCode PreviousSongShortcut = KeyCode.LeftArrow;
    private const KeyCode StartSingSceneShortcut = KeyCode.Return;

    private const KeyCode QuickSearchSong = KeyCode.LeftControl;
    private const KeyCode QuickSearchArtist = KeyCode.LeftAlt;

    void Update()
    {
        SongSelectSceneController songSelectSceneController = SongSelectSceneController.Instance;
        // Open / close search
        if (Input.GetKeyDown(QuickSearchArtist))
        {
            songSelectSceneController.EnableSearch(SearchInputField.ESearchMode.ByArtist);
        }
        if (Input.GetKeyDown(QuickSearchSong))
        {
            songSelectSceneController.EnableSearch(SearchInputField.ESearchMode.BySongTitle);
        }

        if (songSelectSceneController.IsSearchEnabled())
        {
            // When the search is enabled, then close it via Escape
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                songSelectSceneController.DisableSearch();
            }
        }
        else
        {
            // When the search is not enabled, then open the main menu via Escape or Backspace
            if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace))
            {
                SceneNavigator.Instance.LoadScene(EScene.MainScene);
            }
        }

        if (Input.GetKeyUp(NextSongShortcut))
        {
            songSelectSceneController.OnNextSong();
        }

        if (Input.GetKeyUp(PreviousSongShortcut))
        {
            songSelectSceneController.OnPreviousSong();
        }

        if (Input.GetKeyUp(StartSingSceneShortcut))
        {
            GameObject focusedControl = GameObjectUtils.GetSelectedGameObject();
            bool focusedControlIsSongButton = (focusedControl != null && focusedControl.GetComponent<SongRouletteItem>() != null);
            bool focusedControlIsSearchField = (focusedControl != null && focusedControl.GetComponent<SearchInputField>() != null);
            if (focusedControl == null || focusedControlIsSongButton || focusedControlIsSearchField)
            {
                songSelectSceneController.StartSingScene();
            }
        }
    }
}
