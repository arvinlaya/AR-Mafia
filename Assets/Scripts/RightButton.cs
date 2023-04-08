using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RightButton : MonoBehaviour
{
    Vector3 offset = new Vector3(1f, 5, 0);
    PlayerController ownerController;

    public HouseController house { get; set; }

    public Player owner { get; set; }

    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        ownerController = PlayerManager.getPlayerController(owner);
        transform.localPosition += offset;
        GameManager.Instance.OnPhaseChange += ChangePhase;
        renderer = GetComponent<Renderer>();
        renderer.enabled = true;
    }

    void ChangePhase()
    {
        switch (GameManager.Instance.GAME_STATE)
        {
            case GameManager.GAME_PHASE.NIGHT:
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[getSkillMaterialIndex()];
                break;

            case GameManager.GAME_PHASE.DAY_DISCUSSION:
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[7];
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE:
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[7];
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE:
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[7];
                break;

            case GameManager.GAME_PHASE.DAY_VOTE:
                transform.position = ownerController.transform.position;
                transform.localPosition += offset;
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[5];
                break;
        }
    }

    void OnClick(GameManager.GAME_PHASE GAME_STATE)
    {
        switch (GAME_STATE)
        {
            case GameManager.GAME_PHASE.NIGHT:
                Skill((string)PhotonNetwork.LocalPlayer.CustomProperties["ROLE"], owner);
                break;

            case GameManager.GAME_PHASE.DAY_DISCUSSION:
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE:
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE:
                break;

            case GameManager.GAME_PHASE.DAY_VOTE:
                Vote();
                break;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == gameObject.transform)
                {
                    StartCoroutine(nameof(clickFeedback));
                    OnClick(GameManager.Instance.GAME_STATE);
                }
            }
        }

        GameManager.Instance.rotateToCamera(gameObject.transform, ReferenceManager.Instance.camera.transform, 10f, 0, 0);
    }

    void Skill(string ROLE, Player target)
    {
        if (CooldownManager.Instance.getIsSkillCooldown() == false)
        {
            switch (ROLE)
            {
                case "VILLAGER":
                    LogManager.Instance.villagerSkillLog();
                    break;

                case "DOCTOR":
                    new Doctor().skill(target);
                    StartCoroutine(SoundManager.Instance.playGameClip(SoundManager.DOCTOR_SKILL, 0));
                    CooldownManager.Instance.setSkillCooldown(true);
                    break;

                case "MAFIA":
                    new Mafia().skill(target);
                    StartCoroutine(SoundManager.Instance.playGameClip(SoundManager.MAFIA_SKILL, 0));
                    CooldownManager.Instance.setSkillCooldown(true);
                    break;

                case "DETECTIVE":
                    new Detective().skill(target);
                    StartCoroutine(SoundManager.Instance.playGameClip(SoundManager.DETECTIVE_SKILL, 0));
                    CooldownManager.Instance.setSkillCooldown(true);
                    break;

                default:
                    Debug.Log("ROLE NOT FOUND...");
                    break;
            }
        }
        else
        {
            LogManager.Instance.skillCooldown();
        }


    }

    void Vote()
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 0)
        {
            CustomPropertyWrapper.incrementProperty(owner, "GUILTY_VOTE", 1);

            CustomPropertyWrapper.setPropertyInt(PhotonNetwork.LocalPlayer, "VOTE_VALUE", 1);
        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == -1)
        {
            CustomPropertyWrapper.decrementProperty(owner, "INNOCENT_VOTE", 1);

            CustomPropertyWrapper.incrementProperty(owner, "GUILTY_VOTE", 1);

            CustomPropertyWrapper.setPropertyInt(PhotonNetwork.LocalPlayer, "VOTE_VALUE", -1);
        }
    }

    int getSkillMaterialIndex()
    {
        switch (PhotonNetwork.LocalPlayer.CustomProperties["ROLE"])
        {
            case "VILLAGER":
                return 7;

            case "DOCTOR":
                return 2;

            case "MAFIA":
                return 1;

            case "DETECTIVE":
                return 3;

            default:
                Debug.Log("ROLE NOT FOUND...");
                return -1;
        }
    }
    IEnumerator clickFeedback()
    {
        gameObject.transform.localScale += new Vector3(-.15f, -.15f, -.15f);

        yield return new WaitForSeconds(.1f);

        gameObject.transform.localScale += new Vector3(.15f, .15f, .15f);
    }
}
