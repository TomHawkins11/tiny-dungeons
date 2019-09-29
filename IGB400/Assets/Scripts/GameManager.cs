using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Serialization;


	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
		public float turnDelay = 0.1f;							//Delay between each Player turn.
		public int playerHealthPoints = 100;						//Starting value for Player food points.
		public static GameManager Instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
		
		
		private List<Enemy> _enemies;							//List of all Enemy units, used to issue them move commands.
		private bool _enemiesMoving;								//Boolean to check if enemies are moving.
		private bool _doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.
		
		
		
		//Awake is always called before any Start functions
		void Awake()
		{
            //Check if instance already exists
            if (Instance == null)

                //if not, set instance to this
                Instance = this;

            //If instance already exists and it's not this:
            else if (Instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);	
			
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			
			//Assign enemies to a new List of Enemy objects.
			_enemies = new List<Enemy>();
			
			//Get a component reference to the attached BoardManager script
			
			//Call the InitGame function to initialize the first level 
			InitGame();
		}

        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Instance.InitGame();
        }

		
		//Initializes the game for each level.
		public void InitGame()
		{
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			_doingSetup = true;
			

			
			//Clear any Enemy objects in our List to prepare for next level.
			_enemies.Clear();

			StartPlay();
		}
		
		
		//Hides black image used between levels
		void StartPlay()
		{
			//Set doingSetup to false allowing player to move again.

			_doingSetup = false;

		}

		//Update is called every frame.
		void Update()
		{
			//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
			if(playersTurn || _enemiesMoving || _doingSetup)
				
				//If any of these are true, return and do not start MoveEnemies.
				return;
			
			//Start moving enemies.
			StartCoroutine (MoveEnemies ());
		}
		
		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			_enemies.Add(script);
		}
		
		public void RemoveEnemyFromList(Enemy script)
		{
			//Add Enemy to List enemies.
			_enemies.Remove(script);
		}
		
		
		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{
			
			//Disable this GameManager.
			enabled = false;
		}
		
		//Coroutine to move enemies in sequence.
		IEnumerator MoveEnemies()
		{
			//While enemiesMoving is true player is unable to move.
			_enemiesMoving = true;
			
			
			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);
			
			//If there are no enemies spawned (IE in first level):
			if (_enemies.Count == 0) 
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(turnDelay);
				playersTurn = true;
			}
			
			//Loop through List of Enemy objects.
			foreach (var enemy in _enemies)
			{
//Call the MoveEnemy function of Enemy at index i in the enemies List.
				enemy.MoveEnemy ();
				
				//Wait for Enemy's moveTime before moving next Enemy, 
				yield return new WaitForSeconds(enemy.moveTime);
			}
			
			//Once Enemies are done moving, set playersTurn to true so player can move.
			playersTurn = true;
			
			//Enemies are done moving, set enemiesMoving to false.
			_enemiesMoving = false;
		}
	}


