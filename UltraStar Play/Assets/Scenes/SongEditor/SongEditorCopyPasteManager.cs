﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniInject;
using UniRx;

// Disable warning about fields that are never assigned, their values are injected.
#pragma warning disable CS0649

public class SongEditorCopyPasteManager : MonoBehaviour, INeedInjection
{
    [Inject]
    private SongMeta songMeta;

    [Inject]
    private SongAudioPlayer songAudioPlayer;

    [Inject]
    private EditorNoteDisplayer editorNoteDisplayer;

    [Inject]
    private SongEditorSelectionController selectionController;

    [Inject]
    private SongEditorLayerManager layerManager;

    [Inject]
    private SongMetaChangeEventStream songMetaChangeEventStream;

    private Voice copiedVoice;

    private List<Note> CopiedNotes
    {
        get
        {
            return layerManager.GetNotes(ESongEditorLayer.CopyPaste);
        }
    }

    void Start()
    {
        songAudioPlayer.PositionInSongEventStream.Subscribe(newMillis => MoveCopiedNotesToMillisInSong(newMillis));
    }

    void Update()
    {
        EKeyboardModifier modifier = InputUtils.GetCurrentKeyboardModifier();
        if (modifier == EKeyboardModifier.Ctrl)
        {
            if (Input.GetKeyUp(KeyCode.C) && !InputFieldHasFocus())
            {
                CopySelectedNotes();
            }
            else if (Input.GetKeyUp(KeyCode.V) && !InputFieldHasFocus())
            {
                PasteCopiedNotes();
            }
        }
    }

    private bool InputFieldHasFocus()
    {
        GameObject selectedGameObject = GameObjectUtils.GetSelectedGameObject();
        return selectedGameObject != null && selectedGameObject.GetComponentInChildren<InputField>() != null;
    }

    private void MoveCopiedNotesToMillisInSong(double newMillis)
    {
        if (CopiedNotes.IsNullOrEmpty() || songAudioPlayer.IsPlaying)
        {
            return;
        }

        int newBeat = (int)Math.Round(BpmUtils.MillisecondInSongToBeat(songMeta, newMillis));
        int minBeat = CopiedNotes.Select(it => it.StartBeat).Min();
        int distanceInBeats = newBeat - minBeat;
        if (distanceInBeats == 0)
        {
            return;
        }

        foreach (Note note in CopiedNotes)
        {
            note.MoveHorizontal(distanceInBeats);
        }

        editorNoteDisplayer.UpdateNotes();
    }

    private void ClearCopiedNotes()
    {
        layerManager.ClearLayer(ESongEditorLayer.CopyPaste);
    }

    private void PasteCopiedNotes()
    {
        int minBeat = CopiedNotes.Select(it => it.StartBeat).Min();
        Sentence sentenceAtBeatWithVoice = SongMetaUtils.GetSentencesAtBeat(songMeta, minBeat)
            .Where(it => it.Voice != null).FirstOrDefault();

        // Find voice to insert the notes into
        Voice voice;
        if (copiedVoice != null)
        {
            voice = copiedVoice;
        }
        else if (sentenceAtBeatWithVoice != null)
        {
            voice = sentenceAtBeatWithVoice.Voice;
        }
        else
        {
            voice = songMeta.GetVoices().FirstOrDefault();
        }

        // Add the notes to the voice
        foreach (Note note in CopiedNotes)
        {
            InsertNote(note, voice);
        }
        ClearCopiedNotes();

        songMetaChangeEventStream.OnNext(new NotesAddedEvent());
    }

    private void InsertNote(Note note, Voice voice)
    {
        Sentence sentenceAtBeatOfVoice = SongMetaUtils.GetSentencesAtBeat(songMeta, note.StartBeat)
            .Where(sentence => sentence.Voice == voice).FirstOrDefault();
        if (sentenceAtBeatOfVoice == null)
        {
            // Add sentence with note
            Sentence newSentence = new Sentence(new List<Note> { note }, note.EndBeat);
            newSentence.SetVoice(voice);
        }
        else
        {
            // Add note to existing sentence
            note.SetSentence(sentenceAtBeatOfVoice);
        }
    }

    private void CopySelectedNotes()
    {
        // Remove any old copied notes from the Ui.
        foreach (Note note in CopiedNotes)
        {
            editorNoteDisplayer.DeleteNote(note);
        }

        ClearCopiedNotes();

        List<Note> selectedNotes = selectionController.GetSelectedNotes();
        foreach (Note note in selectedNotes)
        {
            copiedVoice = note.Sentence?.Voice;
            Note noteCopy = note.Clone();
            noteCopy.SetSentence(null);
            CopiedNotes.Add(noteCopy);
            layerManager.AddNoteToLayer(ESongEditorLayer.CopyPaste, noteCopy);
        }

        selectionController.SetSelection(CopiedNotes);

        editorNoteDisplayer.UpdateNotes();
    }
}
