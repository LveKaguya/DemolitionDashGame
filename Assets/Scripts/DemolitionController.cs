using UnityEngine;
using System.Collections;

public class DemolitionController : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip explodeSound;
    public AudioClip crackleSound;
    public AudioClip failSound;
    public GameObject crate;
    public Light directionalLight;
    public ParticleSystem explosionEffect;
    public GameObject bombSphere;
    public Light explosionFlash;
    private float originalLightIntensity;

  
    private enum AnimationState { Idle, Windup, Throw, Fail }
    private AnimationState currentState;

    void Start()
    {
        
        if (directionalLight != null)
        {
            originalLightIntensity = directionalLight.intensity;
        }

       
        if (explosionFlash != null)
        {
            explosionFlash.intensity = 0f;
        }

       
        currentState = AnimationState.Idle;
        if (animator != null)
        {
            animator.SetTrigger("ToIdle");
        }
    }

    public void SetToIdle()
    {
        
        if (currentState == AnimationState.Fail)
        {
            if (animator != null) animator.SetTrigger("ToIdle");
            if (directionalLight != null) directionalLight.intensity = originalLightIntensity;
            if (explosionEffect != null) explosionEffect.Stop();
            if (crate != null) crate.SetActive(true);
            if (bombSphere != null) bombSphere.SetActive(true);
            currentState = AnimationState.Idle;
        }
    }

    public void SetToWindup()
    {
       
        if (currentState == AnimationState.Idle)
        {
            if (animator != null) animator.SetTrigger("ToWindup");
            if (directionalLight != null) directionalLight.intensity = originalLightIntensity;
            if (explosionEffect != null) explosionEffect.Stop();
            if (crate != null) crate.SetActive(true);
            if (bombSphere != null) bombSphere.SetActive(true);
            currentState = AnimationState.Windup;
        }
    }

    public void SetToThrow()
    {
       
        if (currentState == AnimationState.Windup)
        {
            if (animator != null) animator.SetTrigger("ToThrow");
            if (audioSource != null && explodeSound != null) audioSource.PlayOneShot(explodeSound);
            if (audioSource != null && crackleSound != null) audioSource.PlayOneShot(crackleSound, 0.5f);
            if (explosionEffect != null) explosionEffect.Play();
            
            if (bombSphere != null) bombSphere.SetActive(false);
            if (directionalLight != null) directionalLight.intensity = originalLightIntensity;

            if (explosionFlash != null)
            {
                StartCoroutine(FlashEffect());
            }
            currentState = AnimationState.Throw;
        }
    }

    public void SetToFail()
    {
        
        if (currentState == AnimationState.Throw)
        {
            if (animator != null) animator.SetTrigger("ToFail");
            if (audioSource != null && failSound != null) audioSource.PlayOneShot(failSound);
            if (directionalLight != null) directionalLight.intensity = originalLightIntensity * 0.5f;
            if (explosionEffect != null) explosionEffect.Stop();
            if (crate != null) crate.SetActive(true);
            if (bombSphere != null) bombSphere.SetActive(true);
            currentState = AnimationState.Fail;
        }
    }

    private IEnumerator FlashEffect()
    {
        explosionFlash.intensity = 5f;
        yield return new WaitForSeconds(0.1f);
        float elapsed = 0f;
        float duration = 0.3f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            explosionFlash.intensity = Mathf.Lerp(5f, 0f, elapsed / duration);
            yield return null;
        }
        explosionFlash.intensity = 0f;
    }
}