using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Global_Data : MonoBehaviour
{
    [Header("production")]
    public float[] production_rate={3,3,2};
    public static float productionCD=10;
    [SerializeField] float elapsedTime;

    [Header("list of objects")]
    public List<GameObject> Team1;
    public List<GameObject> Team2;
    public List<GameObject> Team3;

    public GameObject winScreen;
    public GameObject loseScreen;
    public AudioSource audioSource;
    public AudioClip WinSound;
    public AudioClip LoseSound;

    public GameObject matrix1;
    public GameObject matrix2;
    public GameObject matrix3;
    [Header("misc")]
    private bool win_sound_played=false;
    private bool lose_sound_played=false;
    
    void Awake()
    {
        audioSource=GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>();
        //initialize data of scriptable objects
        foreach(GameObject matrix in GameObject.FindGameObjectsWithTag("Matrix"))
            {
                matrix.GetComponent<Bacterial_Matrix>().production_x=0;
                matrix.GetComponent<Bacterial_Matrix>().production_y=0;
                matrix.GetComponent<Bacterial_Matrix>().production_z=0;

                //shitty placeholder for finding matrix elements
                winScreen.SetActive(false);
                loseScreen.SetActive(false);
                if(matrix.GetComponent<player_matrix>()!=null)
                {
                    matrix1=matrix;
                }
                if(matrix.GetComponent<enemy1matrix>()!=null)
                {
                    matrix2=matrix;
                }
                if(matrix.GetComponent<enemy2matrix>()!=null)
                {
                    matrix3=matrix;
                }
            }
    }
    void Start()
    {
        StartCoroutine(AddProduction());
    }

    void Update()
    {
        elapsedTime+=Time.deltaTime;
        int minutes=Mathf.FloorToInt(elapsedTime/60);
        if(minutes>=3)
        {
            if(minutes>=6)
            {
                production_rate[0]=6;
                production_rate[1]=6;
                production_rate[2]=3;
                productionCD=8;
            }
            else{
                production_rate[0]=4;
            production_rate[1]=4;
            production_rate[2]=2;
            }
            
        }

        //detect win condition
        if(!Team2.Contains(matrix2)&&!Team3.Contains(matrix3))
        {
            if(win_sound_played==false)
            {
                audioSource.PlayOneShot(WinSound);
                win_sound_played=true;
            }
            winScreen.SetActive(true);
            Time.timeScale=0;
        }
        if(!Team1.Contains(matrix1))
        {
            if(lose_sound_played==false)
            {
                audioSource.PlayOneShot(LoseSound);
                lose_sound_played=true;
            }
            loseScreen.SetActive(true);
            Time.timeScale=0;
        }
    }

    IEnumerator AddProduction()
    {
        while(true)
        {
            yield return new WaitForSeconds(productionCD);
            foreach(GameObject matrix in GameObject.FindGameObjectsWithTag("Matrix"))
            {
                //add resources to every bacterial matrix
                matrix.GetComponent<Bacterial_Matrix>().production_x+=production_rate[0];
                matrix.GetComponent<Bacterial_Matrix>().production_y+=production_rate[1];
                matrix.GetComponent<Bacterial_Matrix>().production_z+=production_rate[2];
            }
            
        }
    }

}
