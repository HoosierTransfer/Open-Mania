using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class Main: MonoBehaviour {
  public List < HitCircle > HitCircleList = new List < HitCircle > ();
  public AudioSource hitSounds;
  public AudioSource Song;
  public string[][] hitObjects;
  public int bpm = -1;
  public GameObject[] NoteEmitters;
  public string title;
  public string artist;
  public string creator;
  public string HPDrainRate = null;
  public string OD = null;
  public int currentNote = 0;
  public int currentNoteOld = 0;
  // Start is called before the first frame update
  void Start() {
    readMap("Assets/Songs/t+pazolite & Camellia feat. Nanahira - Boku no Yume, Mecha Kuso Mugen Waki (Rhezie) [Normal].osu");
  }

  List < string > readFile(string dir) {
    string line;

    StreamReader sr = new StreamReader(dir);

    line = sr.ReadLine();
    List < string > file = new List < string > ();
    int i = 0;

    while (line != null) {
      line = sr.ReadLine();
      file.Add(line);
      i++;
    }

    sr.Close();

    return file;
  }

  void readMap(string dir) {
    string tmp = null;
    List < string > map = readFile(dir);
    List < string > timingPoints = new List < string > ();
    List < string > hitObjects = new List < string > ();
    title = null;
    artist = null;
    creator = null;

    for (int i = 0; i < map.Count; i++) {
      tmp = map[i];
      if (tmp.Contains("Title:")) {
        title = tmp;

        title = title.Split(':')[1];
        if (title.StartsWith(" ")) {
          title = title.Trim();
        }
        break;
      }

    }

    for (int i = 0; i < map.Count; i++) {
      tmp = map[i];
      if (tmp.Contains("Artist:")) {
        artist = tmp;

        artist = artist.Split(':')[1];
        if (artist.StartsWith(" ")) {
          artist = artist.Trim();
        }
        break;
      }

    }

    for (int i = 0; i < map.Count; i++) {
      tmp = map[i];
      if (tmp.Contains("Creator:")) {
        creator = tmp;

        creator = creator.Split(':')[1];
        if (creator.StartsWith(" ")) {
          creator = creator.Trim();
        }
        break;
      }

    }

    for (int i = 0; i < map.Count; i++) {
      tmp = map[i];
      if (tmp.Contains("HPDrainRate:")) {
        HPDrainRate = tmp;

        HPDrainRate = HPDrainRate.Split(':')[1];
        if (HPDrainRate.StartsWith(" ")) {
          HPDrainRate = HPDrainRate.Trim();
        }
        break;
      }

    }

    for (int i = 0; i < map.Count; i++) {
      tmp = map[i];
      if (tmp.Contains("OverallDifficulty:")) {
        OD = tmp;

        OD = OD.Split(':')[1];
        if (OD.StartsWith(" ")) {
          OD = OD.Trim();
        }
        break;
      }

    }

    int timingStart = 0;
    int timingEnd = 0;
    //add multiple bpm
    for (int i = 0; i < map.Count; i++) {
      tmp = map[i];

      if (tmp.Contains("[TimingPoints]")) {
        timingStart = i + 1;
        break;
      }

    }

    for (int i = 0; i < map.Count; i++) {
      tmp = map[i];

      if (tmp.Contains("[HitObjects]")) {
        timingEnd = i;
        break;
      }

    }

    for (int i = 0; i < timingEnd - timingStart; i++) {
      if (!string.IsNullOrEmpty(map[i + timingStart])) {
        timingPoints.Add(map[i + timingStart]);
      }
    }
    List < timingPoint > timingPointList = new List < timingPoint > ();

    for (int i = 0; i < timingPoints.Count; i++) {
      string[] temp = timingPoints[i].Split(',');
      int time = int.Parse(temp[0]);
      float beatLength = float.Parse(temp[1]);
      int meter = int.Parse(temp[2]);
      int sampleSet = int.Parse(temp[3]);
      int sampleIndex = int.Parse(temp[4]);
      int volume = int.Parse(temp[5]);
      int uninherited = int.Parse(temp[6]);
      int effects = int.Parse(temp[7]);
      timingPoint currentTimingPoint = new timingPoint(time, beatLength, meter, sampleSet, sampleIndex, volume, uninherited, effects);
      timingPointList.Add(currentTimingPoint);
    }

    for (int i = 0; i < map.Count - timingEnd; i++) {
      if (!string.IsNullOrEmpty(map[i + timingEnd])) {
        hitObjects.Add(map[i + timingEnd]);
      }
    }
    //List<HitCircle> HitCircleList = new List<HitCircle>();

    for (int i = 1; i < hitObjects.Count; i++) {
      string[] temp = hitObjects[i].Split(',');
      int x = int.Parse(temp[0]);
      int y = int.Parse(temp[1]);
      int time = int.Parse(temp[2]);
      int type = int.Parse(temp[3]);
      int hitSound = int.Parse(temp[4]);
      HitCircle currentHitCircle = new HitCircle(x, y, time, type, hitSound);
      HitCircleList.Add(currentHitCircle);
    }

    //HitCircleList = this.HitCircleList;

    Debug.Log(title);
    Debug.Log(artist);
    Debug.Log(creator);
    Debug.Log(HPDrainRate);
    Debug.Log(OD);
    for (int i = 0; i < HitCircleList.Count; i++) {
      Debug.Log(HitCircleList[i].x);
      Debug.Log(HitCircleList[i].time);
      Debug.Log(HitCircleList[i].type);
    }
    for (int i = 0; i < timingPointList.Count; i++) {
      Debug.Log(timingPointList[i].time);
      Debug.Log(timingPointList[i].beatLength);
      Debug.Log(timingPointList[i].effects);
    }
  }
  public int timer = 0;
  public bool played = false;
  // Update is called once per frame
  void Update() {
    if (Input.GetKeyDown(KeyCode.D) == true || Input.GetKeyDown(KeyCode.F) == true || Input.GetKeyDown(KeyCode.J) == true || Input.GetKeyDown(KeyCode.K) == true) {
      hitSounds.Play();
    }
    List < int > avg = new List < int > ();
    //if (Time.frameCount == 1)
    //{
    //    Song.Play();
    //}

    int timeBefore = Mathf.RoundToInt(Time.time * 1000) - Mathf.RoundToInt(Time.deltaTime * 1000);
    int time = Mathf.RoundToInt(Time.time * 1000) + Mathf.RoundToInt(Time.deltaTime * 1000);
    if (Time.time > 3.179656f && !played) {
      Song.Play();
      played = true;
    }

    if (HitCircleList[currentNote].time > timeBefore && HitCircleList[currentNote].time < time) {
      if (HitCircleList[currentNote].time == HitCircleList[currentNote + 1].time) {
        for (int i = 0; i < 2; i++) {
          if (HitCircleList[currentNote].x == 64) {
            NoteEmitters[0].GetComponent < Notes > ().NoteEvent();

          } else if (HitCircleList[currentNote].x == 192) {
            NoteEmitters[1].GetComponent < Notes > ().NoteEvent();
          } else if (HitCircleList[currentNote].x == 320) {
            NoteEmitters[2].GetComponent < Notes > ().NoteEvent();
          } else if (HitCircleList[currentNote].x == 448) {
            NoteEmitters[3].GetComponent < Notes > ().NoteEvent();
          }
          currentNote++;
        }
      } else if (HitCircleList[currentNote].time == HitCircleList[currentNote + 1].time && HitCircleList[currentNote].time == HitCircleList[currentNote + 2].time) {
        for (int i = 0; i < 3; i++) {
          if (HitCircleList[currentNote].x == 64) {
            NoteEmitters[0].GetComponent < Notes > ().NoteEvent();

          } else if (HitCircleList[currentNote].x == 192) {
            NoteEmitters[1].GetComponent < Notes > ().NoteEvent();
          } else if (HitCircleList[currentNote].x == 320) {
            NoteEmitters[2].GetComponent < Notes > ().NoteEvent();
          } else if (HitCircleList[currentNote].x == 448) {
            NoteEmitters[3].GetComponent < Notes > ().NoteEvent();
          }
        }
      } else if (HitCircleList[currentNote].time == HitCircleList[currentNote + 1].time && HitCircleList[currentNote].time == HitCircleList[currentNote + 2].time && HitCircleList[currentNote].time == HitCircleList[currentNote + 3].time) {
        for (int i = 0; i < 4; i++) {
          if (HitCircleList[currentNote].x == 64) {
            NoteEmitters[0].GetComponent < Notes > ().NoteEvent();

          } else if (HitCircleList[currentNote].x == 192) {
            NoteEmitters[1].GetComponent < Notes > ().NoteEvent();
          } else if (HitCircleList[currentNote].x == 320) {
            NoteEmitters[2].GetComponent < Notes > ().NoteEvent();
          } else if (HitCircleList[currentNote].x == 448) {
            NoteEmitters[3].GetComponent < Notes > ().NoteEvent();
          }
        }
      }
  
      if (HitCircleList[currentNote].x == 64) {
        NoteEmitters[0].GetComponent < Notes > ().NoteEvent();

      } else if (HitCircleList[currentNote].x == 192) {
        NoteEmitters[1].GetComponent < Notes > ().NoteEvent();
      } else if (HitCircleList[currentNote].x == 320) {
        NoteEmitters[2].GetComponent < Notes > ().NoteEvent();
      } else if (HitCircleList[currentNote].x == 448) {
        NoteEmitters[3].GetComponent < Notes > ().NoteEvent();
      }
      currentNote++;
    }
  }

}

public class timingPoint {
  public int time;
  public float beatLength;
  public int meter;
  public int sampleSet;
  public int sampleIndex;
  public int volume;
  public int uninherited;
  public int effects;
  public timingPoint(int time, float beatLength, int meter, int sampleSet, int sampleIndex, int volume, int uninherited, int effects) {
    this.time = time;
    this.beatLength = beatLength;
    this.meter = meter;
    this.sampleSet = sampleSet;
    this.sampleIndex = sampleIndex;
    this.volume = volume;
    this.uninherited = uninherited;
    this.effects = effects;
  }

}

public class HitCircle {
  public int x;
  public int y;
  public int time;
  public int type;
  public int hitSound;
  //public int[] hitSample;
  public HitCircle(int x, int y, int time, int type, int hitSound /*int[] hitSample*/ ) {
    this.x = x;
    this.y = y;
    this.time = time;
    this.type = type;
    this.hitSound = hitSound;
    //this.hitSample = hitSample;
  }
}