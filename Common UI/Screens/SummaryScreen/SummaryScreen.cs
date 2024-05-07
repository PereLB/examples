using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SummaryScreen : Modal
{
    #region Variables

    [SerializeField] private RewardSummaryScreen rewardScreen;
    [SerializeField] private XPSummaryScreen xpScreen;
    [SerializeField] private LeaderboardSummaryScreen leaderboardScreen;
    [Header("Listeners")]
    [SerializeField] private ElementChannelSO startUpdateElementEvent;
    [SerializeField] private ElementChannelSO endUpdateElementEvent;
    [SerializeField] private VoidEventChannelSO lvlupEvent;

    private SummaryScreenData data;
    [Space]
    [SerializeField] private GameObject catFinalPosition;
    [SerializeField] GameObject containerPrefab;
    private Camera catCamera;
    public bool TestXP;

    private GameSessionManager gameSessionManager;

    #endregion

    #region Accessors

    private string defaultPet
    {
        get
        {
            return FirebaseManager.Instance.UserData.activePet;
        }
    }

    private List<PetMetadata> ownedPets
    {
        get
        {
            return FirebaseManager.Instance.ownedPetData;
        }
    }

    #endregion

    #region Lifecycle

    private void OnEnable()
    {
        if(!gameSessionManager)
            gameSessionManager = FindObjectOfType<GameSessionManager>();

        CreateData();
        SpawnCat();
        ActivateXPScreen();
    }

    #endregion

    private void CreateData()
    {
        data = new SummaryScreenData();
        if (PersistantSummaryManager.Instance && !TestXP)
        {
            TreatXP();
            data.coinEarned = PersistantSummaryManager.Instance.totalCoinsCollected;
            data.coinTotal = 0; //for now we don't use the total
            data.excitement = PersistantSummaryManager.Instance.statUpdate.fun;
            data.hunger = PersistantSummaryManager.Instance.statUpdate.food;
            data.care = PersistantSummaryManager.Instance.statUpdate.care;
            data.itemsFromServer = new List<DropItems>();
        }
        else
        {
            Debug.LogWarning($"There's been an issue with the persistant summary manager, make sure there's one in the scene");
            data.coinEarned = 1;
            data.coinTotal = 0;
            TreatXPDebug();
            data.excitement = 1;
            data.hunger = 1;
            data.care = 1;
            data.itemsFromServer = new List<DropItems>();
        }
        try
        {
            GameObject cameraObj = GameObject.FindGameObjectWithTag("PetUICamera");
            catCamera = cameraObj ? cameraObj.GetComponent<Camera>() : Camera.main.transform.GetChild(0).GetComponent<Camera>();
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            Debug.LogError("Error setting up the UI cat, do you have a cat camera in the scene?");
        }
    }

    private void TreatXP()
    {
        int currentXP;
        try
        {
            currentXP = (int)ownedPets.FirstOrDefault(x => x.properties.pet.pid == defaultPet).coreStats.xp - PersistantSummaryManager.Instance.totalXPCollected;
            if (currentXP < 0)
                currentXP = 0;
        }
        catch
        {
            currentXP = 0;
        }
        LevelData currentLevelData = GetLevelData(currentXP);
        if (currentLevelData == null)
        {
            TreatXPDebug();
            return;
        }
        data.xpCurrent = currentXP - currentLevelData.xp;
        data.xpEarned = PersistantSummaryManager.Instance.totalXPCollected;
        data.xpTNL = currentLevelData.nextLevel;
        data.level = currentLevelData.level;
    }

    private void TreatXPDebug()
    {
       LevelData currentLevelData = GetLevelData(0);
        if (currentLevelData == null)
        {
            data.xpCurrent = 0;
            data.xpTNL = 100;
        }
        else
        {
            data.xpCurrent = currentLevelData.xp;
            data.xpTNL = currentLevelData.nextLevel;
        }
        data.xpEarned = 250;
        data.level = 1;
    }

    private LevelData GetLevelData(int m_current_xp)
    {
        if (FirebaseManager.Instance)
        {
            return FirebaseManager.Instance.GetLevelData(m_current_xp);
        }
        return null;
    }

    [ContextMenu("SpawnCat")]
    private void SpawnCat()
    {
        if (catCamera == null)
            return;

        if (FirebaseManager.Instance == null)
            return;

        if (!string.IsNullOrEmpty(defaultPet))
        {
            var metadata = ownedPets.FirstOrDefault(x => x.properties.pet.pid == defaultPet);
            if (metadata != null)
            {
                BasePetBuilder _builder = Instantiate(containerPrefab, catCamera.ScreenToWorldPoint(new Vector3(Screen.width * 1.45f, catFinalPosition.transform.position.y, 1.25f)), Quaternion.identity).GetComponent<BasePetBuilder>();
                Vector3 newPos = catCamera.ScreenToWorldPoint(new Vector3(catFinalPosition.transform.position.x, catFinalPosition.transform.position.y, 1.25f));
                _builder.BuildPet(_builder.m_petTypeData, metadata);
                _builder.m_petReference.AddComponent<SummaryCatReaction>().Initialize(startUpdateElementEvent, endUpdateElementEvent, lvlupEvent, newPos, catCamera);
                
                if (AccessoryManager.Instance)
                    AccessoryManager.Instance.BuildPetAccessories(_builder.m_petCrawler);
                return;
            }
        }
    }

    private void ActivateXPScreen()
    {
        xpScreen.gameObject.SetActive(true);
        rewardScreen.gameObject.SetActive(false);
        leaderboardScreen.gameObject.SetActive(false);
        xpScreen.PopulateScreen(ActivateRewardScreen, data.xpCurrent, data.xpEarned, data.xpTNL, data.level);
    }

    private void ActivateRewardScreen()
    {
        xpScreen.gameObject.SetActive(false);
        rewardScreen.gameObject.SetActive(true);
        leaderboardScreen.gameObject.SetActive(false);
        rewardScreen.PopulateScreen(ActivateLeaderboardScreen, data);
    }

    private void ActivateLeaderboardScreen()
    {
        xpScreen.gameObject.SetActive(false);
        rewardScreen.gameObject.SetActive(false);

        leaderboardScreen.gameObject.SetActive(true);
        leaderboardScreen.PopulateScreen(EndSummary, data);
        
        if (gameSessionManager.leaderBoardChangedDuringSession)
        {
            leaderboardScreen.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void EndSummary()
    {
        StartCoroutine(WaitLoadHub());
    }

    private IEnumerator WaitLoadHub()
    {
        yield return new WaitForSeconds(2.0f);
        if (PetUpdateEventManager.Instance)
        {
            AnalyticManager.TriggerEvent(AnalyticDefinitions.MinigameSummary(PetUpdateEventManager.Instance.m_activeMinigame, false), true, false);
        }
        SceneManager.Instance.LoadHub();
    }

}

public class SummaryScreenData
{
    public int coinEarned;
    public int coinTotal;
    public float care;
    public float excitement;
    public float hunger;
    public int xpCurrent;
    public int xpEarned;
    public int xpTNL;
    public int level;
    public List<DropItems> itemsFromServer;
}

public class DropItems
{
    public int id;
    public string name;
    public string description;
    public int amount;
}