using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILibrary : MonoBehaviour {

	public void OnClickToCreateSever()
    {
        NetworkManager.Instance.CreateServer();
    }

    public void OnClickToJoinSever()
    {
        NetworkManager.Instance.JoinServer();
    }
}
