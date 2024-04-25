using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public class NetEnergyOrb : NetworkBehaviour
{

    /// <summary>
    /// 발사 초기 속도
    /// </summary>
    public float speed = 10.0f;

    /// <summary>
    /// 수명
    /// </summary>
    public float lifeTime = 20.0f;

    /// <summary>
    /// 폭발 범위
    /// </summary>
    public float expolsionRadius = 5.0f;

    bool isUsed = false;

    Rigidbody rigid;
    VisualEffect effect;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        effect = GetComponent<VisualEffect>();
    }

    public override void OnNetworkSpawn()
    {
        transform.Rotate(-30.0f, 0, 0);
        if (IsServer)
        {
            //rigid.isKinematic = false;
            if (IsOwner)
                rigid.velocity = speed * transform.forward;
        }
        else
        {
            if(IsOwner)
                SetVelocityServerRpc(speed * transform.forward);
        }
        StartCoroutine(SelfDestroy());
    }

    [ServerRpc]
    void SetVelocityServerRpc(Vector3 newVelocity)
    {
        rigid.velocity = newVelocity;
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        if (IsOwner && this.NetworkObject.IsSpawned)
        {
            if (IsServer)
            {
                this.NetworkObject.Despawn();
            }
            else
            {
                RequestDespawnServerRpc();

            }
        }

    }

    // 충돌은 서버만 감지 가능
    private void OnCollisionEnter(Collision collision)
    {
        if (!this.NetworkObject.IsSpawned)  // 오너가 아니여도 무시spawn되기전에 일어난 충돌은 무시
            return;

        if (!isUsed)
        {
            isUsed = true;

            Collider[] result = Physics.OverlapSphere(transform.position, expolsionRadius);

            if (result.Length > 0)
            {
                List<ulong> targets = new List<ulong>(result.Length);
                foreach (Collider collider in result)
                {
                    Debug.Log(collider.name);
                    NetPlayer hitted = collider.GetComponent<NetPlayer>();
                    GameManager.Instance.Log("");
                    if(hitted != null) { 
                    targets.Add(hitted.OwnerClientId);
                    
                    }

                }

                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = targets.ToArray(),
                    }
                };

                PlayerDieClientRpc(clientRpcParams);
            }


            EffectProcessClientRpc();



        }

    }

    [ClientRpc]
    void PlayerDieClientRpc(ClientRpcParams clientRpcParams = default)
    {
  
        NetPlayer player = GameManager.Instance.Player;
        player.SendChat($"{GameManager.Instance.Player.name}이 사망");
    }

    /// <summary>
    /// ClientRpc는 서버가 모든 클라이언트에게 로컬에서 실행하라고 하는 함수
    /// </summary>
    [ClientRpc]
    void EffectProcessClientRpc()
    {
        if (IsOwner)
        {
            isUsed = true;
            rigid.isKinematic = true;
            rigid.useGravity = false;
            //rigid.velocity = Vector3.zero;
            StartCoroutine(EffectFinishProcess());
        }

    }


    IEnumerator EffectFinishProcess()
    {
        int BaseSize_ID = Shader.PropertyToID("BaseSize");
        int OnEffectFinish_ID = Shader.PropertyToID("OnEffectFinish");


        effect.SetBool("Power", false);

        float expendDuration = 0.5f;
        float elapledTime = 0.0f;

        float preCompute = (1 / expendDuration) * expolsionRadius;
        // 0~1 사이 만들기 : elapsedTime / expendDuration
        // 나누기를 없애기 위해 elapsedTime * (1 / expendDuration)로 계산
        // expolsionRadius을 곱해서 0~expolsionRadius 크기까지 증가

        while (elapledTime < expendDuration)
        {
            elapledTime += Time.deltaTime;
            effect.SetFloat(BaseSize_ID, elapledTime * preCompute);
            yield return null;
        }

        /*        elapledTime = 0;
                expendDuration = 1.0f;
                while (elapledTime < expendDuration)
                {
                    elapledTime += Time.deltaTime;
                    effect.SetFloat("BaseSize",1/ elapledTime * expolsionRadius);
                    yield return null;
                }*/ // 팡 하고 터짐
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = false;


        float reductionDuration = 1.0f;
        elapledTime = reductionDuration;

        float preCompute2 = 1 / reductionDuration;
        while (elapledTime > 0.0f)
        {
            elapledTime -= Time.deltaTime;
            effect.SetFloat(BaseSize_ID, elapledTime * preCompute2 * expolsionRadius);
            yield return null;
        }

        effect.SendEvent("OnEffectFinish");
        while (effect.aliveParticleCount > 0)
        {

            yield return null;
        }
        //Destroy(this.gameObject);
        if (IsServer)
        {
            this.NetworkObject.Despawn();
        }
        else
        {
            RequestDespawnServerRpc();

        }
        // effect.SetFloat("BaseSize", expolsionRadius); 0.5초간 증가 1초동안 0으로 감소 그후 생성 중지
        //effect.SendEvent(""); // 이벤트 날리기
        // effect.aliveParticleCount // 남아있는 파티클 수

    }

    [ServerRpc]
    void RequestDespawnServerRpc()
    {
        this.NetworkObject.Despawn();
    }
}
