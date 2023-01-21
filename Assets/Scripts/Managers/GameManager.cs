using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public static int NIGHT_LENGHT = 40;
    public static int DAY_DISCUSSION_LENGHT = 30;
    public static int DAY_ACCUSE_LENGHT = 20;
    public static int DAY_ACCUSE_DEFENSE_LENGHT = 20;
    public static int DAY_VOTE_LENGHT = 20;

    public static GAME_PHASE GAME_STATE;
    PhotonView PV;
    Role[] roles;

    [SerializeField] public GameObject[] characterModels;

    public enum EVENT_CODE : byte
    {
        REFRESH_TIMER,
        DAY_DISCUSSION_START,
        DAY_ACCUSE_START,
        DAY_ACCUSE_DEFENSE_START,
        DAY_VOTE_START,
        NIGHT_START,
        PHASE_END
    }

    public enum GAME_PHASE : byte
    {
        DAY_DISCUSSION,
        DAY_ACCUSE,
        DAY_ACCUSE_DEFENSE,
        DAY_VOTE,
        NIGHT
    }

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV = GetComponent<PhotonView>();
            characterModels = new GameObject[4];
        }
    }
    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        if (PhotonNetwork.IsMasterClient)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (PhotonNetwork.IsMasterClient)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            int playerCount = 5;
            generateRoles(playerCount, out roles);
            int index = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                Hashtable roleCustomProps = new Hashtable();
                roleCustomProps.Add("ROLE", roles[index].ROLE_TYPE);
                roleCustomProps.Add("IS_KILLED", false);
                roleCustomProps.Add("IS_SAVED", false);
                roleCustomProps.Add("HAS_VOTED", false);
                roleCustomProps.Add("VOTED", "");
                roleCustomProps.Add("ACCUSE_VOTE_COUNT", 0);
                roleCustomProps.Add("GUILTY_VOTE", 0);
                roleCustomProps.Add("INNOCENT_VOTE", 0);
                player.SetCustomProperties(roleCustomProps);
                index++;
            }
        }
    }

    void generateRoles(int playerCount, out Role[] rolesArray)
    {
        Debug.Log("Player count: " + playerCount);
        if (playerCount == 5)
        {
            rolesArray = new Role[] { new Villager(),
                                new Villager(),
                                new Police(),
                                new Mafia(),
                                new Doctor() };
            shuffleArray(rolesArray, rolesArray.Length);
            return;
        }
        else if (playerCount == 6)
        {
            rolesArray = new Role[] { new Villager(),
                                new Villager(),
                                new Villager(),
                                new Police(),
                                new Mafia(),
                                new Doctor() };
            shuffleArray(rolesArray, rolesArray.Length);
            return;
        }
        else if (playerCount == 7)
        {
            rolesArray = new Role[] { new Villager(),
                                new Villager(),
                                new Villager(),
                                new Police(),
                                new Mafia(),
                                new Mafia(),
                                new Doctor() };
            shuffleArray(rolesArray, rolesArray.Length);
            return;
        }
        else if (playerCount == 8)
        {
            rolesArray = new Role[] { new Villager(),
                                new Villager(),
                                new Villager(),
                                new Villager(),
                                new Police(),
                                new Mafia(),
                                new Mafia(),
                                new Doctor() };
            shuffleArray(rolesArray, rolesArray.Length);
            return;
        }
        rolesArray = null;
        Debug.Log("PLAYER COUNT ERROR");
    }

    private void shuffleArray(Role[] roles, int arraySize)
    {
        System.Random random = new System.Random();
        for (int x = 0; x < arraySize; x++)
        {
            swap(roles, x, x + random.Next(arraySize - x));
        }
    }

    private void swap(Role[] arr, int a, int b)
    {
        Role temp = arr[a];
        arr[a] = arr[b];
        arr[b] = temp;
    }

    private void displayArray(Role[] arr)
    {
        for (int x = 0; x < arr.Length; x++)
        {
            Debug.Log(arr[x].ToString());
        }
    }
}
