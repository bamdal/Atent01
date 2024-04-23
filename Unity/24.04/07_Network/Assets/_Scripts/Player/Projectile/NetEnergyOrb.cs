using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class NetEnergyOrb : MonoBehaviour
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

    Rigidbody rigid;
    VisualEffect effect;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        effect = GetComponent<VisualEffect>();
    }

    private void Start()
    {
        transform.Rotate(-30.0f,0,0);
        rigid.velocity = speed * transform.forward;
        Destroy(this.gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider[] result = Physics.OverlapSphere(transform.position, expolsionRadius);

        if(result.Length > 0 )
        {
            foreach(Collider collider in result )
            {
                Debug.Log(collider.name);
            }
        }

        StartCoroutine(EffectFinishProcess());
    }

    IEnumerator EffectFinishProcess()
    {
        effect.SetBool("Power", false);
        float elapledTime = 0;
        while (elapledTime < 0.5f)
        {
            elapledTime += Time.deltaTime;
            if (effect.GetFloat("BaseSize") < expolsionRadius)
            {
                effect.SetFloat("BaseSize", effect.GetFloat("BaseSize")+0.1f);

            }
            
            yield return null;
        }

        elapledTime = 0;
        while (elapledTime < 1f)
        {
            elapledTime += Time.deltaTime;
            if (effect.GetFloat("BaseSize") > 0.01f)
            {
                effect.SetFloat("BaseSize", effect.GetFloat("BaseSize") - 0.15f);

            }
            
            yield return null;
        }
        effect.SendEvent("OnEffectFinish");
        while(effect.aliveParticleCount > 1)
        {
            Destroy(this.gameObject);
            yield return null;
        }
        // effect.SetFloat("BaseSize", expolsionRadius); 0.5초간 증가 1초동안 0으로 감소 그후 생성 중지
        //effect.SendEvent(""); // 이벤트 날리기
        // effect.aliveParticleCount // 남아있는 파티클 수

    }
}
