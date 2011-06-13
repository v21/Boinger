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
		public string splashScreenPath;
		public Texture2D splashScreen = null;
		public MovieTexture splashVideo = null;
		public bool splashIsVideo = false;
		
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
			
			Process process = new Process();
			process.StartInfo.FileName = filePath;
			process.Start();
			
			//how do Events work in c#?  - trigger on process.Exited , assuming Unity is happy to run in the backround.
			
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
		
		if (guiTexture.texture == null){ //to handle rendering once it's loaded
			ChangeTexture();
		}
		
		if (Input.GetButtonDown("Left")){
			currentGameI --;
			ChangeTexture();
			
		}
		if (Input.GetButtonDown("Right")){
			currentGameI ++;
			ChangeTexture();
			
		}
		
		if (Input.GetButtonDown("Launch")){
			configFile.games[currentGameI].LaunchGame();
		}
		
		currentGameI = currentGameI % configFile.games.Count;
		if (currentGameI < 0){
			currentGameI = configFile.games.Count -1 ;
		}
		
	}
	
	void ChangeTexture(){
		Game game = configFile.games[currentGameI];
		
		if (game.splashIsVideo){
			if (game.splashVideo != null){
				
				guiTexture.texture = game.splashVideo;
				MovieTexture guiMovieTex = (MovieTexture)guiTexture.texture;
				guiMovieTex.Play();
				
			}
		}
		else {
		
		if (game.splashScreen != null){
				guiTexture.texture = game.splashScreen;
		}
		}
		
	}
	
	ConfigFile ReadJson(string path){
		
		StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
		return JsonMapper.ToObject<ConfigFile>(sr);
		
	}
	
	
	

	
}