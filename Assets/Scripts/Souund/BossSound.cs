using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] golemRoarSound;
    public AudioClip attackSound;
    public AudioClip attackedSound;
    public AudioClip armorBuffSound;
    public AudioClip rangeAttackSound;
    public AudioClip[] deathSound;

    [SerializeField] private GameObject onTriggerCheckObj;
    [SerializeField] private TriggerCheck triggerCheck;

    float GolemRoarTime = 0.0f;
    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (triggerCheck.isPlayerInRoom)
        {
            if (GolemRoarTime <= 0.0f)
            {
                int rand = Random.Range(0, golemRoarSound.Length);
                audioSource.PlayOneShot(golemRoarSound[rand]);

                GolemRoarTime = Random.Range(5f, 10f);
            }
            else
            {
                GolemRoarTime -= Time.deltaTime;
            }
        }
    }
    private void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackSound);
    }

    public void PlayAttackedSound()
    {
        audioSource.PlayOneShot(attackedSound);
    }

    private void PlayArmorBuffSound()
    {
        audioSource.PlayOneShot(armorBuffSound);
    }

    public void PlayRangeAttackSound()
    {
        audioSource.PlayOneShot(rangeAttackSound);
    }

    private void PlayDeathRoarSound()
    {
        audioSource.PlayOneShot(deathSound[0]);
    }

    private void PlayBreakRockSound()
    {
        audioSource.PlayOneShot(deathSound[1]);
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
