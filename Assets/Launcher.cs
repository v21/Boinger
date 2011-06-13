using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using LitJson;

public class Launcher : MonoBehaviour {

	public bool currentlyVideo;
	
	public Process launched = null;
	
	
	public class Game{
		public string author;
		public string name;
		public string filePath;
		public string splashScreenPath;
		public Texture2D splashScreen = null;
		public MovieTexture splashVideo = null;
		public bool splashIsVideo = false;
		
		public Process process = new Process();
		
		public IEnumerator LoadTexture(){
			if (splashIsVideo){
				WWW www = new WWW(splashScreenPath);
				yield return www;
				splashVideo = www.movie;
			}
			else {
				WWW www = new WWW(splashScreenPath);
				yield return www;
				splashScreen = www.texture;
			}
			
		}
		
		public void LaunchGame () {
			UnityEngine.Debug.Log("game launching");
			process.StartInfo.FileName = filePath;
			process.Start();
			
			//how do Events work in c#?  - trigger on process.Exited , assuming Unity is happy to run in the backround.
			
			
			Screen.fullScreen = false;
			
		}
		
		public void OnGameExited(){
			UnityEngine.Debug.Log("game exited");
			Screen.fullScreen = true;
		}
		
	}
	
	
	
	public class ConfigFile {
		public List<Game> games;
		

		
	}
	
	public ConfigFile configFile;
	
	public int currentGameI = 0;
	
	public string localJsonPath;
	// Use this for initialization
	void Start () {
		
		configFile = ReadJson(localJsonPath);
		
		
		foreach (Game game in configFile.games) {
			StartCoroutine(game.LoadTexture());
		}
		
		currentGameI = 0;
		
		
		//LaunchFile( "C:\\Windows\\system32\\calc.exe");
	}
	
	// Update is called once per frame
	void Update () {
			try {
			if (configFile.games[currentGameI].process.HasExited){ //believe me, i know this isn't the way to do this - but i just want this to work and go to bed
				configFile.games[currentGameI].OnGameExited();
			}
			} catch (Exception) {
				
			}

		
		if (guiTexture.texture == null){ //to handle rendering once it's loaded
			ChangeTexture();
		}
		
		if (Input.GetButtonDown("Left")){
			currentGameI --;
			
			currentGameI = currentGameI % configFile.games.Count;
			if (currentGameI < 0){
				currentGameI = configFile.games.Count -1 ;
			}
			
			ChangeTexture();
			
		}
		if (Input.GetButtonDown("Right")){
			
			currentGameI ++;
			
			currentGameI = currentGameI % configFile.games.Count;
			if (currentGameI < 0){
				currentGameI = configFile.games.Count -1 ;
			}
			
			
			ChangeTexture();
			
		}
		
		if (Input.GetButtonDown("Launch")){
			configFile.games[currentGameI].LaunchGame();
		}
		

		
	}
	
	void ChangeTexture(){
		Game game = configFile.games[currentGameI];
		
		if (currentlyVideo){
			MovieTexture guiMovieTex = (MovieTexture)guiTexture.texture;
			guiMovieTex.Stop();
		}
		
		if (game.splashIsVideo){
			if (game.splashVideo != null){
				
				currentlyVideo = true;
				
				guiTexture.texture = game.splashVideo;
				MovieTexture guiMovieTex = (MovieTexture)guiTexture.texture;
				guiMovieTex.Play();
				guiMovieTex.loop = true;
				
			}
		}
		else {
		
		if (game.splashScreen != null){
				
				currentlyVideo = false;
				guiTexture.texture = game.splashScreen;
		}
		}
		
	}
	
	
	ConfigFile ReadJson(string path){
		
		StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
		return JsonMapper.ToObject<ConfigFile>(sr);
		
	}
	
	
	

	
}