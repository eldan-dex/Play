﻿using System;
using NUnit.Framework;
using System.Collections.Generic;

public class MutableVoiceTests
{
    private MutableVoice mv;

    [SetUp]
    public void TestInit()
    {
        mv = new MutableVoice();
    }

    [Test]
    public void TestCreation()
    {
        Assert.AreEqual(0, mv.GetSentences().Count);
    }

    [Test]
    public void TestOneSentence()
    {
        Note testNote = new Note(ENoteType.Normal, 0, 2, 0, "");
        MutableSentence ms = new MutableSentence();
        ms.Add(testNote);
        mv.Add((Sentence)ms);

        List<Sentence> sentences = mv.GetSentences();
        Assert.AreEqual(1, sentences.Count);
        List<Note> notes = sentences[0].Notes;
        Assert.AreEqual(1, notes.Count);
        Assert.AreEqual(testNote, notes[0]);
    }

    [Test]
    public void TestInterferingLinebreakThrowsException()
    {
        MutableSentence ms = new MutableSentence();
        ms.Add(new Note(ENoteType.Normal, 0, 2, 0, ""));
        ms.SetLinebreakBeat(5);
        mv.Add((Sentence)ms);

        MutableSentence ms2 = new MutableSentence();
        ms2.Add(new Note(ENoteType.Normal, 4, 2, 0, ""));

        VoicesBuilderException vbe = Assert.Throws<VoicesBuilderException>(delegate { mv.Add((Sentence)ms2); });
        Assert.AreEqual("Sentence conflicts with linebreak of previous sentence", vbe.Message);
    }
}
