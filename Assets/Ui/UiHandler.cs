using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiHandler : MonoBehaviour
{
    public Global_Data data;
    public TextMeshProUGUI X_display;
    public TextMeshProUGUI Y_display;
    public TextMeshProUGUI Z_display;
    private player_matrix _player_matrix;
    public Animator Black_screen_animator;
    public Animator Pause_animator;
    public Animator Tutorial_animator;
    public AudioSource audioSource;
    public AudioClip swoosh;
    public AudioClip boing;
    //animator string to hash
    private int Enabled=Animator.StringToHash("enabled");
    private int is_opened=Animator.StringToHash("is_opened");
    private void Awake()
    {
        _player_matrix=FindObjectOfType<player_matrix>();
        audioSource=GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }

    private void Start() {
        StartCoroutine(ShowTutorial());
    }
    void Update()
    {
        X_display.text=string.Format("{0}",_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_x);
        Y_display.text=string.Format("{0}",_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_y);
        Z_display.text=string.Format("{0}",_player_matrix.gameObject.GetComponent<Bacterial_Matrix>().production_z);
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Pause_animator.GetBool(Enabled)==true)
            {
                Time.timeScale=1;
                StartCoroutine(SetPauseBoolCoroutine(Enabled,false));
            }
            if(Pause_animator.GetBool(Enabled)==false)
            {
                audioSource.PlayOneShot(swoosh);
                Pause_animator.SetBool(Enabled,true);
                Time.timeScale=0;
            }
        }
    }
    public void Return()
    {
        audioSource.PlayOneShot(boing);
        StartCoroutine(Load(SceneManager.GetActiveScene().buildIndex-1));
    }
    public void Restart()
    {
        audioSource.PlayOneShot(boing);
        StartCoroutine(Load(SceneManager.GetActiveScene().buildIndex));
    }
    IEnumerator Load(int levelIndex)
    {
        Black_screen_animator.SetBool(is_opened,true);
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale=1;
        SceneManager.LoadScene(levelIndex);
    }
    public void CloseTutorial()
    {
        Tutorial_animator.SetBool(is_opened,false);
    }
    public void SetPauseBool()
    {
        Time.timeScale=1;
        audioSource.PlayOneShot(boing);
        
        StartCoroutine(SetPauseBoolCoroutine(Enabled,false));
    }
    IEnumerator SetPauseBoolCoroutine(int name, bool value)
    {
        yield return null;
        audioSource.PlayOneShot(swoosh);
        Pause_animator.SetBool(name,value);
        yield return null;
    }
    IEnumerator ShowTutorial()
    {
        yield return new WaitForSeconds(1);
        Tutorial_animator.SetBool(is_opened,true);
    }

}
