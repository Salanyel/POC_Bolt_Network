using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region Callbacks

    public override void BoltStartDone()
    {
        Debug.Log("1");
        if (BoltNetwork.isServer)
        {
            Debug.Log("2");
            BoltNetwork.LoadScene("_MainScene");
        } else
        {
            BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse(_ipAddress));
        }
    }

    public override void SceneLoadLocalDone(string map)
    {
        StartCoroutine(TravelCamera());
    }

    #endregion
}
