using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : Bolt.GlobalEventListener
{
    private static NetworkManager _instance;

    private static string _ipAddress = "127.0.0.1:27000";

    public static NetworkManager Instance
    {
        get { if (_instance == null)
            {
                _instance = new NetworkManager();
            }

            return _instance;
        }
    }
    public Text[] _scoreTexts;
    private int[] _scores = new int[2];

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateServer()
    {
        BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse(_ipAddress));
    }

    public void JoinServer()
    {
        BoltLauncher.StartClient();
    }

    void InstantiatePlayer()
    {
        // randomize a position
        var pos = new Vector3(Random.Range(-4, 14), 0.1f, Random.Range(-4, 14));

        // instantiate cube
        GameObject newPlayer = BoltNetwork.Instantiate(BoltPrefabs.Player, pos, Quaternion.identity);
    }    

    IEnumerator TravelCamera()
    {
        Transform startPosition = Camera.main.gameObject.transform;
        Transform destination = GameObject.FindGameObjectWithTag("EndPoint").transform;

        float currentTime = 0f;

        while (currentTime < 5f)
        {
            Camera.main.transform.position = Vector3.Lerp(startPosition.position, destination.position, currentTime / 5f);
            Camera.main.transform.rotation = Quaternion.Lerp(startPosition.rotation, destination.rotation, currentTime / 5f);

            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
    }

    void UpdateScore(int index)
    {
        _scores[index] += 10;
        _scoreTexts[index].text = "Points: " + _scores[index].ToString();
    }

    #region Callbacks

    public override void BoltStartDone()
    {
        if (BoltNetwork.isServer)
        {
            BoltNetwork.LoadScene("_MainScene");
        } else
        {
            BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse(_ipAddress));
        }
    }

    public override void SceneLoadLocalDone(string map)
    {
        StartCoroutine(TravelCamera());
        InstantiatePlayer();
        _scoreTexts[0] = GameObject.FindGameObjectWithTag("P1Score").GetComponent<Text>();
        _scoreTexts[1] = GameObject.FindGameObjectWithTag("P2Score").GetComponent<Text>();
    }

    public override void OnEvent(PlayerScoring evnt)
    {
        UpdateScore(evnt.PlayerIndex);
    }

    #endregion
}
