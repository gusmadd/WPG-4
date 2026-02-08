using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // PC awalnya MATI
        CurrentState = GameState.FAILED; 
        Debug.Log("PC OFF - Waiting for power button...");
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("STATE CHANGE → " + newState);

        HandleState(newState);
    }

    void HandleState(GameState state)
    {
        switch (state)
        {
            case GameState.BOOTING:
                OnBooting();
                break;

            case GameState.DESKTOP:
                OnDesktop();
                break;

            case GameState.BROWSER:
                OnBrowser();
                break;

            case GameState.SEARCHING:
                OnSearching();
                break;

            case GameState.SELECTING:
                OnSelecting();
                break;

            case GameState.PAYMENT:
                OnPayment();
                break;

            case GameState.BUY_HOLD:
                OnBuyHold();
                break;

            case GameState.SUCCESS:
                OnSuccess();
                break;

            case GameState.FAILED:
                OnFailed();
                break;
        }
    }

    // ================= STATES =================

    void OnBooting()
    {
        Debug.Log("PC Booting...");
        Invoke(nameof(ToDesktop), 2f);
    }

    void ToDesktop()
    {
        SetState(GameState.DESKTOP);
    }

    void OnDesktop()
    {
        Debug.Log("Desktop Active");
    }

    void OnBrowser()
    {
        Debug.Log("Browser Opened");
    }

    void OnSearching()
    {
        Debug.Log("Searching Food");
    }

    void OnSelecting()
    {
        Debug.Log("Selecting Item");
    }

    void OnPayment()
    {
        Debug.Log("Payment Screen");
    }

    void OnBuyHold()
    {
        Debug.Log("Holding Buy Button");
    }

    void OnSuccess()
    {
        Debug.Log("ORDER SUCCESS 🎉");
    }

    void OnFailed()
    {
        Debug.Log("PC OFF / FAILED STATE");
    }

    // ================= PUBLIC CONTROL =================

    // Dipanggil dari HoldPowerButton
    public void PowerOn()
    {
        if (CurrentState == GameState.FAILED) // PC mati
        {
            SetState(GameState.BOOTING);
        }
    }

    public void ClickBrowser()
    {
        if (CurrentState == GameState.DESKTOP)
            SetState(GameState.BROWSER);
    }

    public void StartSearch()
    {
        if (CurrentState == GameState.BROWSER)
            SetState(GameState.SEARCHING);
    }

    public void SelectItem()
    {
        if (CurrentState == GameState.SEARCHING)
            SetState(GameState.SELECTING);
    }

    public void GoPayment()
    {
        if (CurrentState == GameState.SELECTING)
            SetState(GameState.PAYMENT);
    }

    public void HoldBuy()
    {
        if (CurrentState == GameState.PAYMENT)
            SetState(GameState.BUY_HOLD);
    }

    public void FinishOrder()
    {
        if (CurrentState == GameState.BUY_HOLD)
            SetState(GameState.SUCCESS);
    }

    public void Fail()
    {
        SetState(GameState.FAILED);
    }
}
