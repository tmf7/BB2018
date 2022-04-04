using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public int state = 0;
    public Transform playerOrigin;
    public PlayerStats playerStats;
    public float fadeTime = .8f;
    public PlayerHUD playerHUD;
    public Transform teleportMainMenu;
    public BlobSwinger[] handBlobs;
    private float timeRemaining;
    private float currentMaxTime;

    public MenuController mainMenu;
    public MenuSludge mainMenuSludge;

    public Transform teleportRoom1;
    public RoomMover roomMover;
    public GameObject sludgeSpawnPoints;
    public GameObject deathParticles;
    public Transform vaultDoor;

    public Transform teleportRoom2;
    public LiquidBottle beakerLiquid;
    public GameObject beakerParticles;
    public AudioClip bubbling, explode;
    public AudioSource beakerSource;
    private ParticleSystem beakerPS;
    private Color BOSS_FIGHT_COLOR = new Color(1, 0, 0, 0.1764706f);
    private Color ORIGINAL_COLOR = new Color(1, 1, 1, 0.1764706f);

    public Transform teleportRoom3;
    public Boss boss;
    public Transform doorL, doorR;

    // Use this for initialization
    void Start () {
        roomMover.enabled = false;
        playerHUD.gameManager = this;
        playerHUD.gameObject.SetActive(false);
        beakerPS = beakerParticles.GetComponent<ParticleSystem>();
        ORIGINAL_COLOR = handBlobs[0].blobMat.GetColor("_Color");
    }
	
	// Update is called once per frame
	void Update () {
        switch (state) {
            case 0: // Main menu
                SetBlobColor(ORIGINAL_COLOR);
                SetBlobShrink(false);
                deathParticles.SetActive(false);
                sludgeSpawnPoints.SetActive(false);
                roomMover.enabled = false;
                Teleport(teleportMainMenu);
                mainMenu.TurnOn();
                state = 1;
                mainMenuSludge.eatenAction += MainMenuStart;
                playerHUD.gameObject.SetActive(false);
                break;
            case 2: // Countdown main menu fade.
                if (timeRemaining > 0) {
                    timeRemaining -= Time.deltaTime;
                } else {
                    Teleport(teleportRoom1);
                    roomMover.enabled = true;
                    roomMover.Center();
                    FadeFrom(Color.white);
                    state = 3;
                }
                break;
            case 3: // Set up room 1.
                SetBlobColor(ORIGINAL_COLOR);
                SetBlobShrink(false);
                deathParticles.SetActive(false);
                sludgeSpawnPoints.SetActive(true);
                // TIME FOR ROOM 2
                timeRemaining = currentMaxTime = 75;
                playerHUD.gameObject.SetActive(true);
                playerStats.Health = 0;
                state = 4;
                break;
            case 4: // Room 1 in progress
                roomMover.targetScale = Mathf.Lerp(8.12f, 1, Mathf.Pow(1f - 1f / (1f + playerStats.Health), 4));
                if (timeRemaining > 0) {
                    timeRemaining -= Time.deltaTime;
                } else {
                    playerHUD.gameObject.SetActive(false);
                    sludgeSpawnPoints.SetActive(false);
                    DestroySludges();
                    if (playerStats.Health >= 100) {
                        vaultDoor.gameObject.GetComponent<AudioSource>().Play();
                        state = 6;
                        FadeTo(Color.white, 4.8f);
                    } else {
                        deathParticles.SetActive(true);
                        FadeTo(Color.black, 6);
                        state = 5;
                    }
                }
                break;
            case 5: // Ded
                if (timeRemaining > 0) {
                    timeRemaining -= Time.deltaTime;
                } else {
                    state = 3;
                    FadeFrom(Color.black);
                }
                break;
            case 6: // Count down room 1 fade.
                if (timeRemaining > 0) {
                    timeRemaining -= Time.deltaTime;
                    vaultDoor.localRotation = Quaternion.Lerp(vaultDoor.localRotation, Quaternion.Euler(-90, 0, 0), .005f);
                } else {
                    SetBlobColor(ORIGINAL_COLOR);
                    beakerParticles.SetActive(false);
                    SetBlobShrink(true);
                    roomMover.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    vaultDoor.localRotation = Quaternion.Euler(-90, 0, -80);
                    roomMover.enabled = false;
                    Teleport(teleportRoom2);
                    FadeFrom(Color.white);
                    beakerLiquid.fillLevel = 0;
                    state = 9;
                }
                break;
            case 8: // Die in room 2
                if (timeRemaining > 0) {
                    timeRemaining -= Time.deltaTime;
                } else {
                    SetBlobColor(ORIGINAL_COLOR);
                    beakerParticles.SetActive(false);
                    SetBlobShrink(true);
                    roomMover.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    roomMover.enabled = false;
                    FadeFrom(Color.black);
                    beakerLiquid.fillLevel = 0;
                    state = 9;
                }
                break;
            case 13:
                if (timeRemaining > 0) {
                    timeRemaining -= Time.deltaTime;
                } else {
                    Teleport(teleportRoom3);
                    beakerParticles.SetActive(false);
                    SetBlobShrink(false);
                    roomMover.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    roomMover.enabled = false;
                    FadeFrom(Color.white);
                    beakerLiquid.fillLevel = 0;
                    state = 14;
                }
                break;
            case 14:
                if (timeRemaining > 0) {
                    timeRemaining -= Time.deltaTime;
                } else {
                    boss.MoveToIntro();
                    boss.currentHealth = 45;
                    timeRemaining = 1;
                    state = 15;
                }
                break;
            case 15:
                if (timeRemaining > 0) {
                    timeRemaining -= Time.deltaTime;
                    doorL.localPosition = Vector3.Lerp(new Vector3(1.57f, 0, 0.048f), new Vector3(0.19f, 0, 0.048f), 1 - timeRemaining);
                    doorR.localPosition = Vector3.Lerp(new Vector3(3.159f, 0, 0.049f), new Vector3(4.539f, 0, 0.049f), 1 - timeRemaining);
                } else {
                    playerHUD.gameObject.SetActive(true);
                    state = 16;
                }
                break;
            case 16:
                if (boss.attacking) {
                    playerStats.RemoveHealth(Time.deltaTime * 4.5f);
                }
                if (playerStats.Health == 0) {
                    state = 17;
                    FadeTo(Color.black, 3);
                }
                if (boss.currentHealth == 0) {
                    FadeTo(Color.white, 15);
                    state = 18;
                }
                break;
            case 17:
                if (timeRemaining > 0) {
                    timeRemaining -= Time.deltaTime;
                } else {
                    roomMover.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    roomMover.enabled = false;
                    FadeFrom(Color.black);
                    beakerLiquid.fillLevel = 0;
                    doorL.localPosition = new Vector3(1.57f, 0, 0.048f);
                    doorR.localPosition = new Vector3(3.159f, 0, 0.049f);
                    state = 14;
                }
                break;
            case 18:
                if (timeRemaining > 0) {
                    timeRemaining -= Time.deltaTime;
                } else {
                    Application.Quit();
                }
                break;
        }

	}

    void DestroySludges() {
        GameObject[] sludges = GameObject.FindGameObjectsWithTag("Sludge");
        foreach (GameObject sludge in sludges) {
            DestroyObject(sludge);
        }
    }

    void MainMenuStart() {
        state = 2;
        FadeTo(Color.white);
        mainMenuSludge.eatenAction -= MainMenuStart;
        mainMenu.TurnOff();
    }

    public void TouchBottle(string bottleName) {
        SoundManager.instance.PlayLocalSoundFx(bubbling, beakerSource);
        switch (state) {
            case 9:
                if (bottleName == "tube_purple") {
                    state = 10;
                    beakerLiquid.liquidMaterial.SetColor("_Color", new Color(1, 0, 0.7254902f, 0));
                    beakerLiquid.fillLevel = .15f;
                } else {
                    FadeTo(Color.black, 3);
                    state = 8;
                }
                break;
            case 10:
                if (bottleName == "tube_blue") {
                    state = 11;
                    beakerLiquid.liquidMaterial.SetColor("_Color", new Color(0, 1, 0.6254902f, 0));
                    beakerLiquid.fillLevel = .3f;
                } else {
                    DieInRoom2();
                }
                break;
            case 11:
                if (bottleName == "tube_red") {
                    state = 12;
                    beakerLiquid.liquidMaterial.SetColor("_Color", new Color(1, 0, .5f, 0));
                    beakerLiquid.fillLevel = .45f;
                } else {
                    DieInRoom2();
                }
                break;
            case 12:
                if (bottleName == "tube_green") {
                    state = 13;
                    beakerLiquid.liquidMaterial.SetColor("_Color", new Color(1, .4f, .2f, 0));
                    beakerLiquid.fillLevel = .6f;
                    beakerParticles.SetActive(true);
                    SetColorGradient(beakerPS, new Color(.3f, .8f, 0), new Color(1, .65f, 0));
                    FadeTo(Color.white, 2);
                    SetBlobColor(BOSS_FIGHT_COLOR);
                    SoundManager.instance.PlayLocalSoundFx(explode, beakerSource);
                } else {
                    DieInRoom2();
                }
                break;
        }
    }

    void DieInRoom2() {
        SoundManager.instance.PlayLocalSoundFx(explode, beakerSource);
        FadeTo(Color.black, 3);
        state = 8;
        beakerParticles.SetActive(true);
        SetColorGradient(beakerPS, new Color(.5f, 0, 0), new Color(0.9019608f, 0, 1));
    }

    void SetColorGradient(ParticleSystem ps, Color color1, Color color2) {
        var col = ps.colorOverLifetime;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(.2f, 1.0f) });
        col.color = grad;
    }

    void Teleport(Transform toTeleport) {
        playerOrigin.position = toTeleport.position;
        playerOrigin.rotation  = toTeleport.rotation;
    }

    void FadeTo(Color color, float time) {
        timeRemaining = time;
        SteamVR_Fade.Start(Color.clear, 0f);
        SteamVR_Fade.Start(color, time);
    }

    void FadeFrom(Color color, float time) {
        timeRemaining = time;
        SteamVR_Fade.Start(color, 0f);
        SteamVR_Fade.Start(Color.clear, time);
    }

    void FadeTo(Color color) {
        timeRemaining = fadeTime;
        SteamVR_Fade.Start(Color.clear, 0f);
        SteamVR_Fade.Start(color, fadeTime);
    }

    void FadeFrom(Color color) {
        timeRemaining = fadeTime;
        SteamVR_Fade.Start(color, 0f);
        SteamVR_Fade.Start(Color.clear, fadeTime);
    }

    void SetBlobShrink(bool shrink) {
        foreach (BlobSwinger blob in handBlobs) {
            blob.SetShrink(shrink);
        }
    }

    void SetBlobColor(Color color) {
        foreach (BlobSwinger blob in handBlobs) {
            blob.blobMat.SetColor("_Color", color);
        }
    }

    public int MinutesRemaining() {
        return (int)(timeRemaining / 60.0f);
    }

    // [0,59]
    public int SecondsRemaining() {
        return (int)(timeRemaining - (MinutesRemaining() * 60));
    }

    public float NomalizedTime {
        get { return timeRemaining / currentMaxTime; }
    }
}
