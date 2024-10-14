using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Animator animator;
    public Animator Tutorial_animator;
    public AudioSource audioSource;
    public AudioClip swoosh;
    [SerializeField] bool Played=false;
    private void Awake() {
        animator=GetComponent<Animator>();
        audioSource=GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>();
        Played=false;
    }
    private void Update() {
        if(Played==true)
        {
            audioSource.volume-=Time.deltaTime;
        }
        else if (Played==false){
            audioSource.volume+=Time.deltaTime;
        }
    }
    public void play()
    {
        audioSource.PlayOneShot(swoosh);
        Played=true;
        StartCoroutine(Load(SceneManager.GetActiveScene().buildIndex+1));
    }
    IEnumerator Load(int levelIndex)
    {
        animator.SetTrigger("EnterGame");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(levelIndex);
    }
    public void EndGame()
    {
        audioSource.PlayOneShot(swoosh);
        Application.Quit();
    }
    public void SpawnTutorial()
    {
        audioSource.PlayOneShot(swoosh);
        Tutorial_animator.SetBool("is_opened",true);
    }
    public void DisableTutorial()
    {
        audioSource.PlayOneShot(swoosh);
        Tutorial_animator.SetBool("is_opened",false);
    }
}
