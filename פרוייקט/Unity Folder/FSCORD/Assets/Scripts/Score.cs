using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
    public static Score instance;
    public int naplamLeft;
    public int minesLeft;
    public int nukesLeft;
    public GameObject[] friendlyTanks;
    public string nextLevel;
    public bool won;
    public bool totalDefeat;
    public float totalNumberOfEnemyTanksDestroyed = 0;
    public float numberOfEnemyTanksDestroyedThisLevel;
    // Use this for initialization

    void Awake()
    {
        DontDestroyOnLoad(this.transform.gameObject);

        if (instance != null)
        {
            Debug.Log("Singleton error!");
        }
        else
        {
            instance = this;
            return;
        }
        


    }

    void Start()
    {

    }
	
	// Update is called once per frame
	

    public void SumThingsUp()
    {
        //Debug.Log("ping");
        friendlyTanks = GameObject.FindGameObjectsWithTag("Player");
        naplamLeft = Game.instance.naplamLeft;
        minesLeft = Game.instance.minesLeft;
        nukesLeft = Game.instance.nukesLeft;       
        nextLevel = Game.instance.nextLevel;
        won = Game.instance.won;
        totalDefeat = Game.instance.isTotalDefeat;       
        numberOfEnemyTanksDestroyedThisLevel = Game.instance.numberOfEnemyTanksDestroyed;
        totalNumberOfEnemyTanksDestroyed = totalNumberOfEnemyTanksDestroyed + numberOfEnemyTanksDestroyedThisLevel;
        //Debug.Log()
    }

    public void Destructor()
    {
        Destroy(this.gameObject);
    }
}
