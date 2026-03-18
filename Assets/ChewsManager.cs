using UnityEngine;

public class ChewsManager : MonoBehaviour
{
    public static ChewsManager Instance;
    [Range(0f, 1f)] public float isIndoors = 0f;
    [Range(0f, 1f)] public float indoorFadeSpeed = 1f;
    float speedMultiplier = 3f; //speed mult for faster fade

    // session start vals for ref to chew grid
    private float baseWindVolume;
    private float baseLpfOpen;
    private float baseRainVolume;
    private float baseDropDensity;
    private bool baseIsIndoors;
   

    [Header("Rain / Wind Settings")]
    [Range(0f, 1f)] public float rainDensity;
    [Range(0f, 1f)] public float windIntensity;
    [Range(0f, 10f)] public float windVolume;
    [Range(0f,1f)] public float rainVolume;
    [Range(0f, 5000f)] public float masterLPF;
    [Range(20f, 2000f)] public float masterHPF;
    [Range(0f, 5000f)] public float dropLPF;
    [Range(20f, 2000f)] public float dropHPF;


    public bool windOn = true;
    public bool rainOn = true;
    public bool playerIndoors = false;

    [Header("Player Reference")]
    public Transform player;                 // drag in player and detect rb or cc
    private CharacterController controller;            
    private Rigidbody rb;                              

    private ChuckSubInstance chuck;
    private bool chuckStarted = false;

    void Awake()
    {

        Instance = this;

        baseWindVolume  = windVolume;
        baseRainVolume  = rainVolume;
        baseDropDensity = rainDensity;
        chuck = GetComponent<ChuckSubInstance>();
        if (!chuck)
            Debug.LogError("ERROR: No ChuckSubInstance found on this object!");

        if (player)
        {
            controller = player.GetComponent<CharacterController>();
            rb = player.GetComponent<Rigidbody>();
        }
    }

    void Start()
    {
        StartChucKAudio();
        SyncInitialChucKValues();
    }

    private void StartChucKAudio()
    {
        if (chuckStarted) return;

        if (windOn)
            chuck.RunFile("wind.ck", true);

        if (rainOn)
            chuck.RunFile("rain.ck", true);

        chuckStarted = true;
    }

    private void SyncInitialChucKValues()
    {
        chuck.SetFloat("rainIntensity", rainDensity);
        chuck.SetFloat("windIntensity", windIntensity);
        chuck.SetFloat("windVolume", windVolume);
        chuck.SetFloat("rainVolume",rainVolume);
        chuck.SetFloat("playerIndoors", playerIndoors ? 1f : 0f);
        chuck.SetFloat("masterLPF", masterLPF);
        chuck.SetFloat("masterHPF", masterHPF);
        chuck.SetFloat("dropLPF", dropLPF);
        chuck.SetFloat("dropHPF", dropHPF);
    }

    void Update()
    {
        FadeIndoors();
        SendChucKGlobals();
    }

    private void SendChucKGlobals()
    {
        if (!chuckStarted || !player) return;

        Vector3 pos = player.position;
        float speed = GetPlayerSpeed();

        // Positional motion values
        chuck.SetFloat("playerElevation", pos.y);
        chuck.SetFloat("playerSpeed", speed);
        chuck.SetFloat("playerPosX", pos.x);
        chuck.SetFloat("playerPosY", pos.z);        // using Z instead of Y for horizontal

        // Slider global values
        chuck.SetFloat("rainIntensity", rainDensity);
        chuck.SetFloat("windIntensity", windIntensity);
        chuck.SetFloat("windVolume", windVolume);
        chuck.SetFloat("rainVolume", rainVolume);
        chuck.SetFloat("rainLPF", masterLPF);
        chuck.SetFloat("rainHPF", masterHPF); 
        chuck.SetFloat("dropLPF", dropLPF);
        chuck.SetFloat("dropHPF", dropHPF);

        // Indoor filter
        chuck.SetFloat("playerIndoors", playerIndoors ? 1f : 0f);
    }
public void SetIndoors(bool indoors)
    {
        playerIndoors = indoors;
    }

    private void FadeIndoors()
    {
        float target = playerIndoors ? 1f : 0f;
        isIndoors = Mathf.MoveTowards(isIndoors, target, indoorFadeSpeed * speedMultiplier * Time.deltaTime);   

        // send to ChucK
        chuck.SetFloat("isIndoors", isIndoors);
    }
    private float GetPlayerSpeed()
    {
        if (controller)
            return controller.velocity.magnitude;
        if (rb)
            return rb.linearVelocity.magnitude;

        return 0f; // fallback for if nothing is given (doesnt fucking crash)
    }
    // chew grid functions
        public void SetWindVolume(float v)
    {
        windVolume = v*10;
    }

    public void SetLPFOpen(float v)
    {
        // convert 
        chuck.SetFloat("outdoorLpfOpen", v);
    }

    public void SetRainVolume(float v)
    {
        rainVolume = v;  
    }

    public void SetDropDensity(float v)
    {
    
        chuck.SetFloat("rainDensity", v);
        rainDensity = v;
    }
    // for exit of ChewGrid back to base chew values
   public void RestoreBaseState()
    {
        SetWindVolume(baseWindVolume/10);
        SetLPFOpen(baseLpfOpen);
        SetRainVolume(baseRainVolume);
        SetDropDensity(baseDropDensity);
        SetIndoors(baseIsIndoors);
        chuck.SetFloat("dropLPF", 3500f);
        chuck.SetFloat("dropHPF", 400f);

        Debug.Log("CHEWS → Restored to base session values.");
    }

}

