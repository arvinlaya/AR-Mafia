using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;
using ExitGames.Client.Photon;
public class VoteManager : MonoBehaviour
{
    public bool hasAccuseVoted;
    public bool hasEliminationVoted;
    public static VoteManager Instance;
    Dictionary<Player, int> playerAccuseVotes;
    int guiltyVotes;
    int innocentVotes;
    private VoteManager.VOTE_CASTED vote_casted;
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
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
    void Start()
    {
        playerAccuseVotes = new Dictionary<Player, int>();
        hasAccuseVoted = false;
        hasEliminationVoted = false;
        vote_casted = VOTE_CASTED.NONE;
    }

    public void castAccuseVote_S(string playerName)
    {
        PhotonNetwork.RaiseEvent((byte)GameManager.EVENT_CODE.CAST_ACCUSE_VOTE, playerName,
                                    new RaiseEventOptions
                                    {
                                        Receivers = ReceiverGroup.All
                                    },
                                    new SendOptions { Reliability = true });
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
    }

    public void castEliminationVote_S(string newVoteCastedString)
    {
        closeEliminationVotePrompt();
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
        object[] content = new object[] { previousVoteCasted, newVoteCasted };

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
    }
    public void castEliminationVote_R(VOTE_CASTED previousVoteCasted, VOTE_CASTED newVoteCasted)
    {

        if (previousVoteCasted == VOTE_CASTED.NONE)
        {
            if (newVoteCasted == VOTE_CASTED.GUILTY)
            {
                guiltyVotes += 1;
            }
            else if (newVoteCasted == VOTE_CASTED.INNOCENT)
            {
                innocentVotes += 1;
            }
        }
        else if (newVoteCasted == VOTE_CASTED.GUILTY)
        {
            guiltyVotes += 1;
            innocentVotes -= 1;
        }
        else if (newVoteCasted == VOTE_CASTED.INNOCENT)
        {
            innocentVotes += 1;
            guiltyVotes -= 1;
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
    }

    public void openEliminationVotePrompt()
    {
        ReferenceManager.Instance.eliminationVotePrompt.SetActive(true);
    }

    public void closeEliminationVotePrompt()
    {
        ReferenceManager.Instance.eliminationVotePrompt.SetActive(false);
    }
}
