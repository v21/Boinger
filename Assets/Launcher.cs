using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LitJson;

public class Launcher : MonoBehaviour {

	public class Game{
		public string author;
		public string name;
		public string filePath;
	}
	
	public class ConfigFile {
		public List<Game> games;
		
	}
	
	public ConfigFile configFile;
	
	public string localJsonPath;
	// Use this for initialization
	void Start () {
		
		configFile = ReadJson(localJsonPath);
		
		
		//LaunchFile( "C:\\Windows\\system32\\calc.exe");
	}
	
	// Update is called once per frame
	void Update () {
		
	
	}
	
	ConfigFile ReadJson(string path){
		
		StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
		return JsonMapper.ToObject<ConfigFile>(sr);
		
	}
	
	void OnGUI(){
		foreach (Game game in configFile.games){
			
			if (GUILayout.Button(game.name)) {
				LaunchFile(game.filePath);
			}
		}
		
	}
	
	void LaunchFile (string path) {
		
		Process process = new Process();
		process.StartInfo.FileName = path;
		process.Start();
		
	}
	
}