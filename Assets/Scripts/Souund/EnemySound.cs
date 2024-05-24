using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public AudioClip[] boneSound;
    public List<AudioClip> attackSound;
    public AudioClip attackedSound;
    public AudioClip deathSound;

    private EnemyCtrl enemyCtrl;

    [SerializeField] private GameObject onTriggerCheckObj;
    [SerializeField] private TriggerCheck triggerCheck;

    float rand;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        enemyCtrl = this.GetComponent<EnemyCtrl>();

        rand = Random.Range(3f, 7f);
    }

    private void Update()
    {
        if (triggerCheck.isPlayerInRoom)
        {
            if (rand >= 0.0f)
            {
                rand -= Time.deltaTime;
            }
            else
            {
                int randIdx = Random.Range(0, boneSound.Length);
                audioSource.PlayOneShot(boneSound[randIdx]);

                rand = Random.Range(3f, 7f);
            }
        }
    }

    private void PlayWalkSound()
    {

    }

    private void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackSound[0]);
    }

    public void PlayAttackedSound()
    {
        audioSource.PlayOneShot(attackedSound);
    }

    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TriggerObj"))
        {
            onTriggerCheckObj = other.gameObject;
            triggerCheck = onTriggerCheckObj.GetComponent<TriggerCheck>();
        }
    }
}
