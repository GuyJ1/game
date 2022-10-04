using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //access the unity enginer to load scenes

//This is a script specifiically for loading scenes from the main menu

public class MainMenu : MonoBehaviour
{
    public void StartGame () //method to start game, attach to startgamebutton
        {
        //use SceneManager.LoadScene(i) where i indicates the scene "name" of which to load or a # based on index queue
        //SceneManager.LoadScene("Chapter1") //Labeling the start of the game as Chapter1
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);  //Loads next scene based in queue, when building scene.
        }

      public void ContinueGame () //method to load continue game screen
        {
          SceneManager.LoadScene("SavedGames"); //loads scene where saved games are saved.
        }

      public void LoadCredits () //method to access credit scene
        {
          SceneManager.LoadScene("Credits"); //Loads "Credit" Ccene
        }
    public void ExitGame () //method exit the game, attach to exitgamebutton
      {
        Debug.Log("Exiting Adrift..."); //For testing purposes, text shows in log
        Application.Quit(); //exits and quits the game application
      }

}
