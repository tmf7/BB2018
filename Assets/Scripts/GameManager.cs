using UnityEngine;

namespace GameJam.BB2018
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState : int
        {
            // main menu
            MAIN_MENU_SETUP,
            MAIN_MENU_RUN,
            MAIN_MENU_FADE,

            // room 1
            ROOM_1_SETUP,
            ROOM_1_RUN,
            ROOM_1_DEATH,

            // room 2
            ROOM_2_SETUP,
            ROOM_2_RUN,
            ROOM_2_DEATH,
            ROOM_2_SEQ_A,
            ROOM_2_SEQ_B,
            ROOM_2_SEQ_C,
            ROOM_2_SEQ_D,
            ROOM_2_TEARDOWN,

            // room 3
            ROOM_3_SETUP,
            ROOM_3_BOSS_INTRO,
            ROOM_3_RUN,
            ROOM_3_DEATH,

            // shutdown
            CLOSE_APPLICATION
        }

        public GameState state = GameState.MAIN_MENU_SETUP;
        public Transform playerOrigin;
        public PlayerStats playerStats;
        public float fadeTime = 0.8f;
        public PlayerHUD playerHUD;
        public Transform teleportMainMenu;
        public BlobSwinger[] handBlobs;

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
        public Transform teleportRoom3;
        public Boss boss;
        public Transform doorL, doorR;

        private ParticleSystem _beakerPS;
        private float _timeRemaining;
        private float _savedHealth;

        private readonly int MAIN_COLOR_PROPERTY = Shader.PropertyToID("_Color");
        private readonly Color BOSS_FIGHT_COLOR = new Color(1, 0, 0, 0.1764706f);

        private const float TIME_FOR_ROOM_1 = 15.0f;

        private void Start()
        {
            roomMover.enabled = false;
            playerHUD.gameManager = this;
            playerHUD.gameObject.SetActive(false);
            _beakerPS = beakerParticles.GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            // _timeRemaining is used in almost every state to account for fade durations
            switch (state)
            {
                case GameState.MAIN_MENU_SETUP:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        ResetBlobColor();
                        SetBlobShrink(false);
                        deathParticles.SetActive(false);
                        sludgeSpawnPoints.SetActive(false);
                        roomMover.enabled = false;
                        Teleport(teleportMainMenu);
                        mainMenu.TurnOn();
                        state = GameState.MAIN_MENU_RUN;
                        mainMenuSludge.eatenAction += StartGame;
                        playerHUD.gameObject.SetActive(false);
                    }
                    break;
                }
                case GameState.MAIN_MENU_FADE:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        Teleport(teleportRoom1);
                        roomMover.enabled = true;
                        roomMover.Center();
                        FadeFrom(Color.white);
                        state = GameState.ROOM_1_SETUP;
                    }
                    break;
                }
                case GameState.ROOM_1_SETUP:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        ResetBlobColor();
                        SetBlobShrink(false);
                        deathParticles.SetActive(false);
                        sludgeSpawnPoints.SetActive(true);
                        _timeRemaining = TIME_FOR_ROOM_1;
                        playerHUD.gameObject.SetActive(true);
                        playerStats.Health = 0;
                        state = GameState.ROOM_1_RUN;
                    }
                    break;
                }
                case GameState.ROOM_1_RUN:
                {
                    roomMover.targetScale = Mathf.Lerp(8.12f, 1, Mathf.Pow(1f - 1f / (1f + playerStats.Health), 4));
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        playerHUD.gameObject.SetActive(false);
                        sludgeSpawnPoints.SetActive(false);
                        DestroySludges();
                        if (playerStats.Health >= 100)
                        {
                            vaultDoor.gameObject.GetComponent<AudioSource>().Play();
                            state = GameState.ROOM_2_SETUP;
                            FadeTo(Color.white, 4.8f);
                        }
                        else
                        {
                            deathParticles.SetActive(true);
                            FadeTo(Color.black, 6);
                            state = GameState.ROOM_1_DEATH;
                        }
                    }
                    break;
                }
                case GameState.ROOM_1_DEATH:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        state = GameState.ROOM_1_SETUP;
                        FadeFrom(Color.black);
                    }
                    break;
                }
                case GameState.ROOM_2_SETUP:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                        vaultDoor.localRotation = Quaternion.Lerp(vaultDoor.localRotation, Quaternion.Euler(-90, 0, 0), .005f);
                    }
                    else
                    {
                        ResetBlobColor();
                        beakerParticles.SetActive(false);
                        SetBlobShrink(true);
                        roomMover.gameObject.transform.localScale = Vector3.one;
                        vaultDoor.localRotation = Quaternion.Euler(-90, 0, -80);
                        roomMover.enabled = false;
                        Teleport(teleportRoom2);
                        FadeFrom(Color.white);
                        beakerLiquid.fillLevel = 0;
                        boss.PlayConversation();
                        state = GameState.ROOM_2_SEQ_A;
                    }
                    break;
                }
                case GameState.ROOM_2_DEATH:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        ResetBlobColor();
                        beakerParticles.SetActive(false);
                        SetBlobShrink(true);
                        roomMover.gameObject.transform.localScale = Vector3.one;
                        roomMover.enabled = false;
                        FadeFrom(Color.black);
                        beakerLiquid.fillLevel = 0;
                        state = GameState.ROOM_2_SEQ_A;
                    }
                    break;
                }
                case GameState.ROOM_2_TEARDOWN:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        Teleport(teleportRoom3);
                        beakerParticles.SetActive(false);
                        SetBlobShrink(false);
                        roomMover.gameObject.transform.localScale = Vector3.one;
                        roomMover.enabled = false;
                        FadeFrom(Color.white);
                        beakerLiquid.fillLevel = 0;
                        _savedHealth = playerStats.Health;
                        state = GameState.ROOM_3_SETUP;
                    }
                    break;
                }
                case GameState.ROOM_3_SETUP:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                    }
                    else
                    { 
                        boss.StartFightIntro();
                        _timeRemaining = 1;
                        playerStats.Health = _savedHealth;
                        state = GameState.ROOM_3_BOSS_INTRO;
                    }
                    break;
                }
                case GameState.ROOM_3_BOSS_INTRO:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                        doorL.localPosition = Vector3.Lerp(new Vector3(1.57f, 0, 0.048f), new Vector3(0.19f, 0, 0.048f), 1 - _timeRemaining);
                        doorR.localPosition = Vector3.Lerp(new Vector3(3.159f, 0, 0.049f), new Vector3(4.539f, 0, 0.049f), 1 - _timeRemaining);
                    }
                    else
                    {
                        playerHUD.gameObject.SetActive(true);
                        state = GameState.ROOM_3_RUN;
                    }
                    break;
                }
                case GameState.ROOM_3_RUN:
                {
                    if (boss.Attacking)
                    {
                        playerStats.RemoveHealth(Time.deltaTime * 4.5f);
                    }

                    if (playerStats.Health <= 0)
                    {
                        state = GameState.ROOM_3_DEATH;
                        FadeTo(Color.black, 3);
                    }
                    else if (boss.State == Boss.BossStates.DEAD)
                    {
                        FadeTo(Color.white, 15);
                        state = GameState.CLOSE_APPLICATION;
                    }
                    break;
                }
                case GameState.ROOM_3_DEATH:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        roomMover.gameObject.transform.localScale = new Vector3(1, 1, 1);
                        roomMover.enabled = false;
                        FadeFrom(Color.black);
                        beakerLiquid.fillLevel = 0;
                        doorL.localPosition = new Vector3(1.57f, 0, 0.048f);
                        doorR.localPosition = new Vector3(3.159f, 0, 0.049f);
                        state = GameState.ROOM_3_SETUP;
                    }
                    break;
                }
                case GameState.CLOSE_APPLICATION:
                {
                    if (_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        Application.Quit();
                    }
                    break;
                }
            }
        }

        private void DestroySludges()
        {
            GameObject[] sludges = GameObject.FindGameObjectsWithTag("Sludge");
            foreach (GameObject sludge in sludges)
            {
                DestroyObject(sludge);
            }
        }

        private void StartGame()
        {
            state = GameState.MAIN_MENU_FADE;
            FadeTo(Color.white);
            mainMenuSludge.eatenAction -= StartGame;
            mainMenu.TurnOff();
        }

        public void TouchBottle(string bottleName)
        {
            SoundManager.PlayLocalSoundFx(bubbling, beakerSource);

            switch (state)
            {
                case GameState.ROOM_2_SEQ_A:
                {
                    if (bottleName == "tube_purple")
                    {
                        state = GameState.ROOM_2_SEQ_B;
                        beakerLiquid.liquidMaterial.SetColor(MAIN_COLOR_PROPERTY, new Color(1, 0, 0.7254902f, 0));
                        beakerLiquid.fillLevel = 0.15f;
                    }
                    else
                    {
                        FadeTo(Color.black);
                        state = GameState.ROOM_2_DEATH;
                    }
                    break;
                }
                case GameState.ROOM_2_SEQ_B:
                {
                    if (bottleName == "tube_blue")
                    {
                        state = GameState.ROOM_2_SEQ_C;
                        beakerLiquid.liquidMaterial.SetColor(MAIN_COLOR_PROPERTY, new Color(0, 1, 0.6254902f, 0));
                        beakerLiquid.fillLevel = .3f;
                    }
                    else
                    {
                        DieInRoom2();
                    }
                    break;
                }
                case GameState.ROOM_2_SEQ_C:
                {
                    if (bottleName == "tube_red")
                    {
                        state = GameState.ROOM_2_SEQ_D;
                        beakerLiquid.liquidMaterial.SetColor(MAIN_COLOR_PROPERTY, new Color(1, 0, .5f, 0));
                        beakerLiquid.fillLevel = .45f;
                    }
                    else
                    {
                        DieInRoom2();
                    }
                    break;
                }
                case GameState.ROOM_2_SEQ_D:
                {
                    if (bottleName == "tube_green")
                    {
                        state = GameState.ROOM_2_TEARDOWN;
                        beakerLiquid.liquidMaterial.SetColor(MAIN_COLOR_PROPERTY, new Color(1, .4f, .2f, 0));
                        beakerLiquid.fillLevel = .6f;
                        beakerParticles.SetActive(true);
                        SetColorGradient(_beakerPS, new Color(.3f, .8f, 0), new Color(1, .65f, 0));
                        FadeTo(Color.white);
                        SetBlobColor(BOSS_FIGHT_COLOR);
                        SoundManager.PlayLocalSoundFx(explode, beakerSource);
                    }
                    else
                    {
                        DieInRoom2();
                    }
                    break;
                }
            }
        }

        private void DieInRoom2()
        {
            state = GameState.ROOM_2_DEATH;
            SoundManager.PlayLocalSoundFx(explode, beakerSource);
            FadeTo(Color.black);
            beakerParticles.SetActive(true);
            SetColorGradient(_beakerPS, new Color(.5f, 0, 0), new Color(0.9019608f, 0, 1));
        }

        private void SetColorGradient(ParticleSystem ps, Color color1, Color color2)
        {
            var col = ps.colorOverLifetime;
            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(.2f, 1.0f) });
            col.color = grad;
        }

        private void Teleport(Transform toTeleport)
        {
            playerOrigin.position = toTeleport.position;
            playerOrigin.rotation = toTeleport.rotation;
        }

        private void FadeTo(Color color, float time)
        {
            _timeRemaining = time;
            SteamVR_Fade.Start(Color.clear, 0f);
            SteamVR_Fade.Start(color, time);
        }

        private void FadeFrom(Color color, float time)
        {
            _timeRemaining = time;
            SteamVR_Fade.Start(color, 0f);
            SteamVR_Fade.Start(Color.clear, time);
        }

        private void FadeTo(Color color)
        {
            _timeRemaining = fadeTime;
            SteamVR_Fade.Start(Color.clear, 0f);
            SteamVR_Fade.Start(color, fadeTime);
        }

        private void FadeFrom(Color color)
        {
            _timeRemaining = fadeTime;
            SteamVR_Fade.Start(color, 0f);
            SteamVR_Fade.Start(Color.clear, fadeTime);
        }

        private void SetBlobShrink(bool shrink)
        {
            foreach (BlobSwinger blob in handBlobs)
            {
                blob.SetShrink(shrink);
            }
        }

        private void ResetBlobColor()
        {
            foreach (BlobSwinger blob in handBlobs)
            {
                blob.ResetColor();
            }
        }

        private void SetBlobColor(Color color)
        {
            foreach (BlobSwinger blob in handBlobs)
            {
                blob.SetColor(color);
            }
        }

        public int MinutesRemaining()
        {
            return (int)(_timeRemaining / 60.0f);
        }

        // [0,59]
        public int SecondsRemaining()
        {
            return (int)(_timeRemaining - (MinutesRemaining() * 60));
        }

        public float NomalizedTime
        {
            get { return _timeRemaining / TIME_FOR_ROOM_1; }
        }
    }
}