using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MyPlayer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private RaycastHit rayHit = new RaycastHit();
    private EventSystem eSystem;
    
    private Animator animator;
    
    private Vector3 lastPos;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;

    private List<int> animHash = new List<int>(); 
    
    private void Awake()
    {
        eSystem = TownManager.Instance.E_System;
        
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        TownManager.Instance.FreeLook.Follow = transform;
        TownManager.Instance.FreeLook.LookAt = transform;
        
        TownManager.Instance.FreeLook.gameObject.SetActive(true);
        
        lastPos = transform.position;
        
        animHash.Add(Constants.TownPlayerAnim1);
        animHash.Add(Constants.TownPlayerAnim2);
        animHash.Add(Constants.TownPlayerAnim3);
        animHash.Add(Constants.TownPlayerAnim4);
        animHash.Add(Constants.TownPlayerAnim5);
        animHash.Add(Constants.TownPlayerAnim6);
    }

    public void Set(ParticleSystem clickEffect, LayerMask clickableLayers)
    {
        this.clickEffect = clickEffect;
        this.clickableLayers = clickableLayers;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(eSystem.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out rayHit, 100f, clickableLayers))
            {
                agent.SetDestination(rayHit.point);
                if (clickEffect != null)
                {
                    ParticleSystem go = Instantiate(clickEffect, rayHit.point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
                    Destroy(go.gameObject, 1);
                }
            }
        }

        CheckMove();
    }

    public void AnimationExecute(int animIdx)
    {
        int animKey = animHash[animIdx];
        
        agent.SetDestination(transform.position);

        C_Animation animationPacket = new C_Animation { AnimCode = animKey };
        GameManager.Network.Send(animationPacket);
    }

    void CheckMove()
    {
        float dist = Vector3.Distance(lastPos, transform.position);
        animator.SetFloat(Constants.TownPlayerMove, dist * 100);

        if (dist > 0.01f)
        {
            TransformInfo tr = new TransformInfo();
            tr.PosX = transform.position.x;
            tr.PosY = transform.position.y;
            tr.PosZ = transform.position.z;
            tr.Rot = transform.eulerAngles.y;
            
            C_Move enterPacket = new C_Move { Transform = tr };
            GameManager.Network.Send(enterPacket);
        }

        lastPos = transform.position;
    }
}
