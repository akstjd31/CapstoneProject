using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public AudioClip[] lobbyWalkSound, dungeonWalkSound;
    public List<AudioClip> attackSound;
    public List<AudioClip> attackedSound;
    public AudioClip itemPickupSound;
    public AudioClip levelUpSound;
    public AudioClip openInventorySound, closeInventorySound;
    public AudioClip attackJewelSound;
    public AudioClip deathSound;
    public AudioClip openStoreSound;

    private PlayerCtrl playerCtrl;
    private Status status;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        playerCtrl = this.GetComponent<PlayerCtrl>();
        status = this.GetComponent<Status>();
    }

    // 애니메이션 이벤트
    private void PlayWalkSound()
    {
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            int rand = Random.Range(0, lobbyWalkSound.Length);

            audioSource.PlayOneShot(lobbyWalkSound[rand]);
        }
        else
        {
            int rand = Random.Range(0, dungeonWalkSound.Length);

            audioSource.PlayOneShot(dungeonWalkSound[rand]);
        }
    }
    
    private void PlayAttackSound()
    {
        if (attackSound.Count < 2)
        {
            audioSource.PlayOneShot(attackSound[0]);
        }
        else
        {
            if (status.charType.Equals("Warrior"))
            {
                if (playerCtrl.GetEquipItem().attackSpeed <= 5)
                {
                    audioSource.PlayOneShot(attackSound[0]);
                }
                else
                {
                    audioSource.PlayOneShot(attackSound[1]);
                }
            }
        }
    }

    public void PlayAttackJewelSound()
    {
        audioSource.PlayOneShot(attackJewelSound);
    }

    public void PlayAttackedSound()
    {
        int rand = Random.Range(0, attackedSound.Count);
        audioSource.PlayOneShot(attackedSound[rand]);
    }
    
    public void PlayItemPickupSound()
    {
        audioSource.PlayOneShot(itemPickupSound);
    }

    public void PlayLevelUpSound()
    {
        audioSource.PlayOneShot(levelUpSound);
    }

    public void PlayOpenInventorySound()
    {
        audioSource.PlayOneShot(openInventorySound);
    }

    public void PlayCloseInventorySound()
    {
        audioSource.PlayOneShot(closeInventorySound);
    }

    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }
}
