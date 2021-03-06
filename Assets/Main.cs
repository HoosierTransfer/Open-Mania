using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

public class Main: MonoBehaviour {
  public float difficultyRating = 0;
  public int maxScore = 0;
  public int avarageTimeBetweenNotes = 0;
  public int modMultiplier = 1;
  public int countPerfect = 0;
  public int countGreat = 0;
  public int countOk = 0;
  public int countMeh = 0;
  public int countMiss = 0;
  public int difficultyMultiplier = 0;
  public int combo = 0;
  public float health = 1f;
  public int hitError = 0;
  public int score = 0;
  public List <float> acc = new List<float>();
  public int accuracy = 100;
  public List < HitCircle > HitCircleList = new List < HitCircle > ();
  public AudioSource hitSounds;
  public AudioClip songSource;
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
  public string AudioFilename;
  public bool isAudioLoaded = false;
  public float loadingTime = -1;
  // Start is called before the first frame update
  void Start() {
    readMap(sceneData.mapDir, sceneData.osuFile);
  }

  List < string > readFile(string dir) {
    string line;

    Debug.Log(dir);
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

  void readMap(string dir, string osuFile) {
    string tmp = null;
    List < string > map = readFile(dir);
    List < string > timingPoints = new List < string > ();
    List < string > hitObjects = new List < string > ();
    AudioFilename = null;
    title = null;
    artist = null;
    creator = null;

   

      for (int i = 0; i < map.Count; i++) {
      tmp = map[i];
      if (tmp.Contains("AudioFilename:")) {
        AudioFilename = tmp;

        AudioFilename = AudioFilename.Split(':')[1];
        if (AudioFilename.StartsWith(" ")) {
          AudioFilename = AudioFilename.Trim();
        }
        break;
      }

    }

    StartCoroutine(wait(dir, osuFile));

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

    for(int i = HitCircleList.Count; i > -1; i--) {
      avarageTimeBetweenNotes+=HitCircleList[i]-HitCircleList[i-1];
    }
    avarageTimeBetweenNotes = avarageTimeBetweenNotes / HitCircleList.Count;

    float notesPerSecond = avarageTimeBetweenNotes / 1000;

    int diffValue = OD + HPDrainRate;

    if(diffValue <= 5) {
      difficultyMultiplier = 2;
    } else if(diffValue !< 6 && diffValue <= 12) {
      difficultyMultiplier = 3;
    } else if(diffValue !< 13 && diffValue <= 17) {
      difficultyMultiplier = 4;
    } else if(diffValue !< 19 && diffValue <= 24) {
      difficultyMultiplier = 5;
    }

    for(int i = 0; i < HitCircleList.Count; i++) {
      maxScore+= 315 + (315 * ((i * difficultyMultiplier * modMultiplier) / 25));
    }

    difficultyRating = (notesPerSecond + (HitCircleList.Count / 500)) * (difficultyMultiplier / 5)

    Debug.Log(title);
    Debug.Log(artist);
    Debug.Log(creator);
    Debug.Log(HPDrainRate);
    Debug.Log(OD);
    Debug.Log(("file://" + Application.dataPath + "/" + dir + "/" + AudioFilename).Replace(osuFile + "/", "").Replace("Assets/Assets/", "Assets/").Replace("\\", "/"));
  }
  public bool played = false;
  void Update() {
    if(isAudioLoaded) {
    health -= ((HPDrainRate * 0.0025f) * Time.deltaTime) * 10;
    if(health>1) {
      health = 1;
    }
    if (Input.GetKeyDown(KeyCode.D) == true || Input.GetKeyDown(KeyCode.F) == true || Input.GetKeyDown(KeyCode.J) == true || Input.GetKeyDown(KeyCode.K) == true) {
      hitSounds.Play();
    }
    List < int > avg = new List < int > ();
    int timeBefore = Mathf.RoundToInt((Time.time - loadingTime) * 1000) - Mathf.RoundToInt(Time.deltaTime * 1000);
    int time = Mathf.RoundToInt((Time.time - loadingTime) * 1000) + Mathf.RoundToInt(Time.deltaTime * 1000);
    if (Time.time - loadingTime > 3.179656f && !played) {
      Song.Play();
      played = true;
    }
        Debug.Log(time);
    if (HitCircleList[currentNote].time > timeBefore && HitCircleList[currentNote].time < time) {
      if (HitCircleList[currentNote].time == HitCircleList[currentNote + 1].time) {

        for (int i = 0; i < 2; i++) {
          if (HitCircleList[currentNote].x == 64) {
            NoteEmitters[0].GetComponent < Notes > ().NoteEvent();
            StartCoroutine(noteHitTiming(KeyCode.D, HitCircleList[currentNote].time));
            judgements(hitError);

          } else if (HitCircleList[currentNote].x == 192) {
            NoteEmitters[1].GetComponent < Notes > ().NoteEvent();
            StartCoroutine(noteHitTiming(KeyCode.F, HitCircleList[currentNote].time));
            judgements(hitError);

          } else if (HitCircleList[currentNote].x == 320) {
            NoteEmitters[2].GetComponent < Notes > ().NoteEvent();
            StartCoroutine(noteHitTiming(KeyCode.J, HitCircleList[currentNote].time));
            judgements(hitError);

          } else if (HitCircleList[currentNote].x == 448) {
            NoteEmitters[3].GetComponent < Notes > ().NoteEvent();
            StartCoroutine(noteHitTiming(KeyCode.K, HitCircleList[currentNote].time));
            judgements(hitError);
            
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

  private IEnumerator wait(string dir, string osuFile) {
     UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(("file://" + Application.dataPath + "/" + dir + "/" + AudioFilename).Replace(osuFile + "/", "").Replace("Assets/Assets/", "Assets/").Replace("\\", "/"), AudioType.MPEG);
     yield return request.SendWebRequest();
     Song.clip = DownloadHandlerAudioClip.GetContent(request);
     isAudioLoaded = true;
     loadingTime = Time.time;

}


  private IEnumerator noteHitTiming(KeyCode key, int noteTime) {
    float hitTime = 0f;
    float timeOld = Time.time;
    while(!Input.GetKey(key) || Time.time - timeOld > 3.179656f){
      hitTime = Mathf.RoundToInt((Time.time - loadingTime) * 1000)
    }
    hitError = hitTime - noteTime;
    yield return null;
  }


  void judgements(int hitDelay) {
    int hitScore = 0;
    if(hitDelay = 0 && !hitDelay < 0) {
      countPerfect++;
      combo++;
      hitScore = 315 + (315 * ((combo * difficultyMultiplier * modMultiplier) / 25));
      if(health < 0.96f) {
        health+=0.0525
      }

    } else if(hitDelay <= (80-(OD*6)) && !hitDelay < 0) {
      countGreat++;
      combo++;
      hitScore = 300 + (300 * ((combo * difficultyMultiplier * modMultiplier) / 25));
      acc.Add(100f);
      if(health < 0.96f) {
        health+=0.025
      }
    } else if(!hitdelay <= (80-(OD*6)) && hitdelay <= (140-(OD*8)) && !hitdelay < 0) {
      countOk++;
      combo++;
      hitScore = 100 + (100 * ((combo * difficultyMultiplier * modMultiplier) / 25));
      acc.add(33.33f);
      if(health < 0.96f) {
        health+=0.03
      }
    } else if(!hitdelay <= (140-(OD*8)) && hitdelay <= (200-(OD*10)) && !hitdelay < 0 ) {
      countMeh++;
      combo++;
      hitScore = 50 + (50 * ((combo * difficultyMultiplier * modMultiplier) / 25));
      acc.add(16.67f);
      if(health < 0.96f) {
        health-=0.0025f
      }
    } else if(!hitdelay <= (200-(OD*10)) && hitdelay <= 2000 && !hitdelay < 0 ) {
      countMiss++;
      combo = 0;
      acc.add(0f);
      if(health > 0) {
        health-=0.05
      }
    }
    for(int i = 0; i < acc.Count; i++) {
      accuracy+=acc[i];
    }
    score+=hitScore;
    accuracy = accuracy / acc.Count;
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

public class Hold {
  public int x;
  public int y;
  public int time;
  public int type;
  public int hitSound;
  public int endTime
  //public int[] hitSample;
  public Hold(int x, int y, int time, int type, int hitSound, int endTime /*int[] hitSample*/ ) {
    this.x = x;
    this.y = y;
    this.time = time;
    this.type = type;
    this.hitSound = hitSound;
    this.endTime = endTime
    //this.hitSample = hitSample;
  }
}
