using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Machine : MonoBehaviour {
    public enum GameState { BeforStartGame, StartGame, PauseBeforStart, Playing, Winner, GameOver, ReloadGame }

    public GameState currentState;
    GameObject mainCamera;
    float timeStateChange = 0.0f;
    Transform cameraStartPlace;
    Transform playerStartPlace;
    GameObject playerStart;
    int counterLife = 3;
    int levelGame = 1;

    GameObject gameTextObject;
    Text gameText;

    void Start()
    {

        gameTextObject = GameObject.Find("myText");
        gameText = gameTextObject.GetComponent<Text>();
        gameText.text = " ";

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraStartPlace = mainCamera.transform;
        SetCurrentState(GameState.BeforStartGame);
        playerStart = GameObject.Find("Player");
        playerStartPlace = playerStart.transform;

    }
    void SetCurrentState(GameState state)
    {
        currentState = state;
        timeStateChange = Time.time;
    }
    float PauseBeforePlaying()
    {
        return Time.time - timeStateChange;
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.BeforStartGame:
                gameText.text = "To start the game, press the spacebar.";
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    SetCurrentState(GameState.StartGame);
                }
                break;

            case GameState.StartGame:
                gameText.text = " ";
                mainCamera.transform.Translate(Vector3.up * 2f * Time.deltaTime);
                Vector3 pos = playerStartPlace.position - cameraStartPlace.position;
                Quaternion rotation = Quaternion.LookRotation(pos);
                cameraStartPlace.rotation = rotation;
                if (PauseBeforePlaying() > 1.5f)
                {
                    SetCurrentState(GameState.PauseBeforStart);
                }
                break;
            case GameState.PauseBeforStart:
                if (PauseBeforePlaying() > 1f)
                {
                    SetCurrentState(GameState.Playing);
                }
                break;
            case GameState.Playing:

                gameText.text = "Level " + levelGame + "       Number of lives " + counterLife;
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    playerStartPlace.transform.Translate(Vector3.left * 2 * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                {
                    playerStartPlace.transform.Translate(Vector3.right * 2 * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    playerStartPlace.transform.Translate(Vector3.forward * 2 * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    playerStartPlace.transform.Translate(Vector3.back * 2 * Time.deltaTime);
                }
                break;
            case GameState.GameOver:
                gameText.text = "Game Over \n To restart the game, press the spacebar";
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    counterLife = 3;
                    SetCurrentState(GameState.PauseBeforStart);
                }
                break;
            case GameState.Winner:
                playerStart.transform.localScale = playerStart.transform.localScale * 1.15f;
                levelGame++;
                SetCurrentState(GameState.PauseBeforStart);
                if (levelGame == 4)
                {
                    levelGame = 1;
                    SetCurrentState(GameState.ReloadGame);
                }
                break;

            case GameState.ReloadGame:
                gameText.text = "You won the game \n To restart the game, press the Escape";
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    playerStart.transform.localScale = new Vector3(1, 1, 1);
                    SetCurrentState(GameState.PauseBeforStart);
                }
                break;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Winner")
        {
            playerStart.transform.position = new Vector3(-3.9f, 0.46f, -13.94f);
            currentState = GameState.Winner;
            Debug.Log("counterLife Winner" + counterLife);
        }
        if (col.gameObject.tag == "GameController")
        {
            counterLife--;
            playerStart.transform.position = new Vector3(-3.9f, 0.46f, -13.94f);
            currentState = GameState.PauseBeforStart;
        }
        if (counterLife == 0)
        {
            currentState = GameState.GameOver;
        }
    }
}
