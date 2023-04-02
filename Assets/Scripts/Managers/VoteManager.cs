using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;
using ExitGames.Client.Photon;
using TMPro;

public class VoteManager : MonoBehaviour
{
    public bool hasAccuseVoted;
    public bool hasEliminationVoted;
    public static VoteManager Instance;
    Dictionary<Player, int> playerAccuseVotes;
    int guiltyVotes;
    int innocentVotes;
    private VoteManager.VOTE_CASTED vote_casted;
    private string playerAccused;
    private Dictionary<Player, string> playerVote;
    [SerializeField] public GameObject eliminationVotePrompt;
    [SerializeField] public GameObject eliminationVoteResults;
    [SerializeField] public TMP_Text eliminationHeader;
    [SerializeField] public TMP_Text eliminationGuiltyBody;
    [SerializeField] public TMP_Text eliminationInnocentBody;
    [SerializeField] public GameObject accuseVotePrompt;
    public enum VOTE_CASTED : byte
    {
        NONE,
        GUILTY,
        INNOCENT
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
        playerAccuseVotes = new Dictionary<Player, int>();
        hasAccuseVoted = false;
        hasEliminationVoted = false;
        vote_casted = VOTE_CASTED.NONE;
        playerVote = new Dictionary<Player, string>();
    }

    public void castAccuseVote_S()
    {
        if (hasAccuseVoted == true)
        {
            return;
        }
        hasAccuseVoted = true;
        PhotonNetwork.RaiseEvent((byte)GameManager.EVENT_CODE.CAST_ACCUSE_VOTE, playerAccused,
                                    new RaiseEventOptions
                                    {
                                        Receivers = ReceiverGroup.All
                                    },
                                    new SendOptions { Reliability = true });

        closeAccuseVotePrompt();
    }
    public void castAccuseVote_R(string playerName)
    {
        Player votedPlayer = PhotonNetwork.PlayerList.First(player => player.NickName == playerName);

        if (playerAccuseVotes.ContainsKey(votedPlayer))
        {
            int vote = 1 + playerAccuseVotes[votedPlayer];
            playerAccuseVotes[votedPlayer] = vote;
        }
        else
        {
            playerAccuseVotes.Add(votedPlayer, 1);
        }

        Debug.Log("VOTED THIS PLAYER");
    }

    public void castEliminationVote_S(string newVoteCastedString)
    {
        string sourceName = PhotonNetwork.LocalPlayer.NickName;
        newVoteCastedString = newVoteCastedString.Trim();
        VOTE_CASTED newVoteCasted = VOTE_CASTED.NONE;
        VOTE_CASTED previousVoteCasted = vote_casted;
        if (newVoteCastedString == "GUILTY")
        {
            newVoteCasted = VOTE_CASTED.GUILTY;
        }
        else if (newVoteCastedString == "INNOCENT")
        {
            newVoteCasted = VOTE_CASTED.INNOCENT;
        }

        if (vote_casted == newVoteCasted)
        {
            return;
        }

        vote_casted = newVoteCasted;
        object[] content = new object[] { previousVoteCasted, newVoteCasted, sourceName };

        if (previousVoteCasted == newVoteCasted)
        {
            return;
        }

        PhotonNetwork.RaiseEvent((byte)GameManager.EVENT_CODE.CAST_ELIMINATION_VOTE, content,
                                    new RaiseEventOptions
                                    {
                                        Receivers = ReceiverGroup.All
                                    },
                                    new SendOptions { Reliability = true });

        closeEliminationVotePrompt();
    }

    // TODO: LIST PLAYER VOTES IN ELIMINATION RESULT
    public void castEliminationVote_R(VOTE_CASTED previousVoteCasted, VOTE_CASTED newVoteCasted, string sourceName)
    {
        Player source = null;
        foreach (Player player in GameManager.Instance.getAliveList())
        {
            if (sourceName == player.NickName)
            {
                source = player;
                break;
            }
        }

        if (previousVoteCasted == VOTE_CASTED.NONE)
        {
            if (newVoteCasted == VOTE_CASTED.GUILTY)
            {
                guiltyVotes += 1;
                playerVote[source] = "GUILTY";
            }
            else if (newVoteCasted == VOTE_CASTED.INNOCENT)
            {
                innocentVotes += 1;
                playerVote[source] = "INNOCENT";
            }
        }
        else if (newVoteCasted == VOTE_CASTED.GUILTY)
        {
            guiltyVotes += 1;
            innocentVotes -= 1;
            playerVote[source] = "GUILTY";
        }
        else if (newVoteCasted == VOTE_CASTED.INNOCENT)
        {
            innocentVotes += 1;
            guiltyVotes -= 1;
            playerVote[source] = "INNOCENT";
        }
    }

    public Dictionary<Player, int> getHighestAccusedPlayer()
    {

        playerAccuseVotes = playerAccuseVotes.OrderBy(player => player.Value).ToDictionary(player => player.Key, player => player.Value);

        if (playerAccuseVotes.Count < 1)
        {
            return null;
        }

        if (playerAccuseVotes.Count > 1)
        {
            if (playerAccuseVotes.ElementAt(0).Value == playerAccuseVotes.ElementAt(1).Value)
            {
                return null;
            }
        }
        return new Dictionary<Player, int> { { playerAccuseVotes.ElementAt(0).Key, playerAccuseVotes.ElementAt(0).Value } };
    }

    public bool isGuilty()
    {
        return guiltyVotes > innocentVotes;
    }

    public void showVotes()
    {
        foreach (KeyValuePair<Player, int> entry in playerAccuseVotes)
        {
            Debug.Log(entry.Key + " " + entry.Value);
        }
    }

    public void resetAll()
    {
        hasAccuseVoted = false;
        hasEliminationVoted = false;
        playerAccuseVotes.Clear();
        guiltyVotes = 0;
        innocentVotes = 0;
        vote_casted = VOTE_CASTED.NONE;
        playerVote.Clear();
    }

    public void openAccuseVotePrompt(string playerName)
    {
        playerAccused = playerName;
        accuseVotePrompt.SetActive(true);
        accuseVotePrompt.GetComponentInChildren<TMP_Text>().SetText($"Accuse {playerName} as Mafia?");
    }
    public void closeAccuseVotePrompt()
    {
        accuseVotePrompt.SetActive(false);
    }
    public void openEliminationVotePrompt()
    {
        eliminationVotePrompt.SetActive(true);
        eliminationVotePrompt.GetComponentInChildren<TMP_Text>().SetText($"Accuse {playerAccused} as Mafia?");
    }

    public void closeEliminationVotePrompt()
    {
        eliminationVotePrompt.SetActive(false);
    }

    public Dictionary<Player, int> getPlayerAccuseVotes()
    {
        return playerAccuseVotes;
    }

    public int getGuiltyVotes()
    {
        return guiltyVotes;
    }

    public int getInnocentVotes()
    {
        return innocentVotes;
    }

    public Dictionary<Player, string> getPlayerVote()
    {
        return this.playerVote;
    }
}
