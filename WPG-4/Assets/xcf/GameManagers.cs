using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        SetState(GameState.Tutorial);
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        Debug.Log("Game State: " + newState);
    }
}