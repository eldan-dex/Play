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

public class NoteAreaContextMenuHandler : AbstractContextMenuHandler, INeedInjection
{
    [Inject]
    private SongMeta songMeta;

    [Inject]
    private SongEditorSceneController songEditorSceneController;

    [Inject]
    private SongMetaChangeEventStream songMetaChangeEventStream;

    [Inject]
    private NoteArea noteArea;

    [Inject]
    private AddNoteAction addNoteAction;

    protected override void FillContextMenu(ContextMenu contextMenu)
    {
        int beat = (int)noteArea.GetHorizontalMousePositionInBeats();
        int midiNote = noteArea.GetVerticalMousePositionInMidiNote();
        contextMenu.AddItem("Fit vertical", () => noteArea.FitViewportVerticalToNotes());
        contextMenu.AddItem("Add note", () => addNoteAction.ExecuteAndNotify(songMeta, beat, midiNote));
    }
}
