using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour {

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] Image healthBar;

    //[SerializeField] AudioClip[] damageSounds;
    //[SerializeField] AudioClip[] deathSounds;
    [SerializeField] float deathVanishSeconds = 2f;

    const string DEATH_TRIGGER = "Death";

    public float currentHealthPoints;

    Animator animator;
    AudioSource audioSource;
    Character characterMovement;

    public float healthAsPercentage {
        get {
            return currentHealthPoints / maxHealthPoints;
        }
    }

    void Start() {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterMovement = GetComponent<Character>();

        currentHealthPoints = maxHealthPoints;
    }

    void Update() {
        UpdateHealthBar();

    }

    void UpdateHealthBar() {
        if (healthBar) {
            healthBar.fillAmount = healthAsPercentage;
        }
    }

    public void Heal(float amount) {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints + amount, 0f, maxHealthPoints);
    }

    public void TakeDamage(float amount) {
        bool characterDies = (currentHealthPoints - amount <= 0);

        currentHealthPoints = Mathf.Clamp(currentHealthPoints - amount, 0f, maxHealthPoints);
        //var clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
        //audioSource.PlayOneShot(clip);

        if (characterDies) {
            StartCoroutine(KillCharacter());
        }
    }

    IEnumerator KillCharacter() {
        if (gameObject.GetComponent<PlayerControl>() != null) {
            var playerControl = GetComponent<PlayerControl>();
            playerControl.Kill();
            yield break;
        }

        characterMovement.Kill();
        animator.SetTrigger(DEATH_TRIGGER);

        //audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
        //audioSource.Play();
        //yield return new WaitForSecondsRealtime(audioSource.clip.length);
        yield return new WaitForSecondsRealtime(2f);
        var playerComponent = GetComponent<PlayerControl>();
        if (playerComponent && playerComponent.isActiveAndEnabled) { // Lazy evaluation
            //SceneManager.LoadScene(0);
            print("Reload scene");
        } else { // Assuming Enemy for now, reconsider for other NPCs
            Destroy(gameObject, deathVanishSeconds);
        }
    }

} // HealthSystem
