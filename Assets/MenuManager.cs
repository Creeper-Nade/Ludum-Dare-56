using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Animator animator;
    public Animator Tutorial_animator;
    private void Awake() {
        animator=GetComponent<Animator>();
    }
    public void play()
    {
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
        Application.Quit();
    }
    public void SpawnTutorial()
    {
        Tutorial_animator.SetBool("is_opened",true);
    }
    public void DisableTutorial()
    {
        Tutorial_animator.SetBool("is_opened",false);
    }
}
