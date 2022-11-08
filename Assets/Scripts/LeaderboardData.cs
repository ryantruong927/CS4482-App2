using System;
using System.Collections.Generic;

[Serializable]
public class ScoreData {
	public int level;
	public string name;
	public float score;
}

[Serializable]
public class LeaderboardData {
	public List<ScoreData> entries = new List<ScoreData>();
}
