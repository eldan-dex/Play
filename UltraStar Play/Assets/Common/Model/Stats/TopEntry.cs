<<<<<<< HEAD
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds data about one of the "top" songs
[Serializable]
public class TopEntry
{
    public string SongName { get; private set; }
    public string SongArtist { get; private set; }
    public SongStatistic SongStatistic { get; private set; }

    public TopEntry(string songName, string songArtist, SongStatistic songStatistic)
    {
        this.SongName = songName;
        this.SongArtist = songArtist;
        this.SongStatistic = songStatistic;
    }
}

//Comparer for score sorting
public class CompareByTopEntryScore : IComparer<TopEntry>
{
    public int Compare(TopEntry x, TopEntry y)
    {
        if (x.SongStatistic == null || y.SongStatistic == null)
        {
            return 0;
        }

        return x.SongStatistic.Score.CompareTo(y.SongStatistic.Score);
    }
}
=======
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds data about one of the "top" songs
[Serializable]
public class TopEntry
{
    public string songName { get; private set; }
    public string songArtist { get; private set; }
    public SongStatistic songStatistic { get; private set; }

    public TopEntry(string songName, string songArtist, SongStatistic songStatistic)
    {
        this.songName = songName;
        this.songArtist = songArtist;
        this.songStatistic = songStatistic;
    }
}

//Comparer for score sorting
public class CompareByTopEntryScore : IComparer<TopEntry>
{
    public int Compare(TopEntry x, TopEntry y)
    {
        if (x.songStatistic == null || y.songStatistic == null)
        {
            return 0;
        }

        return x.songStatistic.Score.CompareTo(y.songStatistic.Score);
    }
}
>>>>>>> parent of 5e3c645... Improved highscore display in the song select
