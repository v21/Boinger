using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using LitJson;
using System.Windows.Forms;



public class Launcher : MonoBehaviour {

	public bool currentlyVideo;
	
	public Process launched = null;
	
	KeyboardHook hook;
	
	public class Game{
		public string author;
		public string name;
		public string filePath;
		public string splashScreenPath;
		public Texture2D splashScreen = null;
		public MovieTexture splashVideo = null;
		public bool splashIsVideo = false;
		
		public Process process = null;
		
		public IEnumerator LoadTexture(){
			splashScreenPath = "file://" + splashScreenPath; 
			
			if (splashIsVideo){
				WWW www = new WWW(splashScreenPath);
				yield return www;
				if (www.error == null){
					splashVideo = www.movie;
				}
			}
			else {
				WWW www = new WWW(splashScreenPath);
				yield return www;
				if (www.error == null){
					splashScreen = www.texture;
				}
			}
			
			
		}
		
		public void LaunchGame () {
			UnityEngine.Debug.Log("game launching");
			process = new Process();
			process.StartInfo.FileName = filePath;
			process.Start();
			
			//how do Events work in c#?  - trigger on process.Exited , assuming Unity is happy to run in the backround.
			
			
			UnityEngine.Screen.fullScreen = false;
			
		}
		
		public void OnGameExited(){
			UnityEngine.Debug.Log("game exited");
			UnityEngine.Screen.fullScreen = true;
		}
		
	}
	
	
	
	public class ConfigFile {
		public List<Game> games;
		

		
	}
	
	public ConfigFile configFile;
	
	public int currentGameI = 0;
	
	public Texture2D errorTex;
	public GUIText errorGUIText;
	
	public Texture2D imgMissing;
	
	public bool firstImageLoaded = false;
	
	public string localJsonPath;
	// Use this for initialization
	void Start () {
		UnityEngine.Screen.showCursor = false;
		try{
			configFile = ReadJson(localJsonPath);
		} catch (Exception e){
			
			
			guiTexture.texture = errorTex;
			errorGUIText.text = e.Message;
		}
		
		
		foreach (Game game in configFile.games) {
			StartCoroutine(game.LoadTexture());
		}
		
		currentGameI = 0;
		
		hook = new KeyboardHook();
		// register the event that is fired after the key press.
        hook.KeyPressed += 
            new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
        // register the control + alt + F12 combination as hot key.
        hook.RegisterHotKey(ModifierKeys.None, 
            Keys.F11);
		
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
		
		if (!firstImageLoaded && (configFile.games[currentGameI].splashScreen != null || configFile.games[currentGameI].splashVideo != null)){ //to handle rendering once it's loaded
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
			try {
				MovieTexture guiMovieTex = (MovieTexture)guiTexture.texture;
				guiMovieTex.Stop();
			}
			catch (Exception){
				
			}
		}
		
		if (game.splashIsVideo){
			if (game.splashVideo != null){
				
				currentlyVideo = true;
				
				guiTexture.texture = game.splashVideo;
				MovieTexture guiMovieTex = (MovieTexture)guiTexture.texture;
				guiMovieTex.Play();
				errorGUIText.text = "";
				
				guiMovieTex.loop = true;
				firstImageLoaded = true;
			}
			else {
				guiTexture.texture = imgMissing;
				errorGUIText.text = game.name + "\n" + game.author;
				
				
			}
		}
		else {
		
			if (game.splashScreen != null){
					
				currentlyVideo = false;
				guiTexture.texture = game.splashScreen;
				errorGUIText.text = "";
				firstImageLoaded = true;
			}
			else {
				guiTexture.texture = imgMissing;
				errorGUIText.text = game.name + "\n" + game.author;
			}
		}
		
		
	}
	
	
	ConfigFile ReadJson(string path){
		
		StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
		return JsonMapper.ToObject<ConfigFile>(sr);
		
	}
	
	
	void hook_KeyPressed(object sender, KeyPressedEventArgs e)
    {
        // show the keys pressed in a label.
        print("hotkey pressed");
        try {
			if (configFile.games[currentGameI].process != null){ 
        		if (!configFile.games[currentGameI].process.CloseMainWindow()){
        			configFile.games[currentGameI].process.Kill();
        			
        		}
        		
				configFile.games[currentGameI].process.Close();
				configFile.games[currentGameI].OnGameExited();
				
			}
		} catch (Exception err) {
			
		}
        
    }

	
}