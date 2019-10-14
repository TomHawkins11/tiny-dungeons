using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
		[FormerlySerializedAs("healthText")] public GameObject healthBar;						//UI Text to display current player health total.
		public GameObject useBar;
		
		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.
		[FormerlySerializedAs("WeaponPosition")] public GameObject weaponPosition;
		public GameObject equippedWeapon;
		
		private Animator _animator;					//Used to store a reference to the Player's animator component.
		private int _health;                           //Used to store player health points total during level.
		private static readonly int PlayerHit = Animator.StringToHash("playerHit");
		private static readonly int PlayerAttack = Animator.StringToHash("playerAttack");
		private static readonly int PlayerMove = Animator.StringToHash("playerMove");
		public GameObject tombStone;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif
		
		
		//Start overrides the Start function of MovingObject
		protected override void Start ()
		{
			//Get a component reference to the Player's animator component
			_animator = GetComponent<Animator>();
			
			//Get the current health point total stored in GameManager.instance between levels.
			_health = GameManager.Instance.playerHealthPoints;
			

			
			//Call the Start function of the MovingObject base class.
			base.Start ();
		}
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			//When Player object is disabled, store the current local health total in the GameManager so it can be re-loaded in other levels.
			GameManager.Instance.playerHealthPoints = _health;
		}
		
		
		private void Update ()
		{
			//If it's not the player's turn, exit the function.
			if (!GameManager.Instance.playersTurn)
			{
				return;
			}
			
			int horizontal = 0;  	//Used to store the horizontal move direction.
			int vertical = 0;		//Used to store the vertical move direction.
			
			//Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
			
			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
			//Check if moving horizontally, if so set vertical to zero.
			if(horizontal != 0)
			{
				vertical = 0;
			}
			//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
			//Check if we have a non-zero value for horizontal or vertical
			if(horizontal != 0 || vertical != 0)
			{
				//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
				//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
				AttemptMove<Enemy> (horizontal, vertical);
			}
		}
		
		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//Every time player moves, subtract from food points total.
			
			
			//Update food text display to reflect current score.
			//foodText.text = "Food: " + food;

			
			//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
			base.AttemptMove <T> (xDir, yDir);
			
			//Hit allows us to reference the result of the Linecast done in Move.
			RaycastHit2D hit;
			
			//If Move returns true, meaning Player was able to move into an empty space.
			if (Move (xDir, yDir, out hit)) 
			{
				//Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
				_animator.SetTrigger (PlayerMove);

			}
			
			//Since the player has moved and lost health points, check if the game has ended.
			CheckIfGameOver ();
			
			//Set the playersTurn boolean of GameManager to false now that players turn is over.
			GameManager.Instance.playersTurn = false;
		}

		public void WeaponPickup(GameObject weapon)
		{
			weapon.transform.SetParent(weaponPosition.transform);
			weapon.GetComponent<BoxCollider2D>().enabled = false;
			weapon.transform.localPosition = Vector3.zero;
			equippedWeapon = weapon;
		}

		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		protected override void OnCantMove <T> (T component)
		{
			Debug.Log("Enemy Hit for" );
			//Set hitWall to equal the component passed in as a parameter.
			Enemy hitEnemy = component as Enemy;
			
			//Call the DamageWall function of the Wall we are hitting.

			System.Diagnostics.Debug.Assert(hitEnemy != null, "hitEnemy != null");
			hitEnemy.DamageEnemy(equippedWeapon.GetComponent<Weapon>().DealDamage());
			equippedWeapon.GetComponent<Weapon>().UpdateUses();
			Instantiate(equippedWeapon.GetComponent<Weapon>().attackPrefab, hitEnemy.gameObject.transform.position, Quaternion.identity);
			

			//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
			_animator.SetTrigger (PlayerAttack);
			

		}
		
		
		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Check if the tag of the trigger collided with is Exit.
			if(other.CompareTag("Exit"))
			{
				//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
				Invoke (nameof(Restart), restartLevelDelay);
				
				//Disable the player object since level is over.
				enabled = false;
			}
			
			//Check if the tag of the trigger collided with is Rations.
			else if(other.CompareTag("Rations"))
			{
				return;
			}
			
			else if(other.CompareTag("Weapon"))
			{
				WeaponPickup(other.gameObject);
				equippedWeapon.GetComponent<Weapon>().UpdateUses();
			}
			
			//Check if the tag of the trigger collided with is Gold.
			else if(other.CompareTag("Gold"))
			{
				
			}
		}
		
		
		//Restart reloads the scene when called.
		private void Restart ()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
            //and not load all the scene object in the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		
		
		// is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void PlayerDamaged (int loss)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
			_animator.SetTrigger (PlayerHit);
			
			//Subtract lost health points from the players total.
			_health -= loss;
			
			//Update the health display with the new total.
			healthBar.GetComponent<HeartDisplay>().health = _health;
			
			//Check to see if game has ended.
			CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of health points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if health point total is less than or equal to zero.
			if (_health <= 0) 
			{
				//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
				SoundManager.instance.PlaySingle (gameOverSound);
				
				//Stop the background music.
				SoundManager.instance.musicSource.Stop();
				
				//Call the GameOver function of GameManager.
				GameManager.Instance.GameOver ();

				Instantiate(tombStone, gameObject.transform.position, Quaternion.identity);
				Destroy(gameObject);
				GameObject.FindWithTag("GameOverPanel").SetActive(true);
			}
		}
	}


