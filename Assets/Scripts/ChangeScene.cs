using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    float TimeScene = 0f;

    // Start is called before the first frame update
    void Start()
    {
        TimeScene = 0.0001f;
    }

    // Update is called once per frame
    void Update()
    {
        TimeScene += Time.unscaledDeltaTime;

        if (TimeScene > 0f && TimeScene > 2.5f)
        {
            TimeScene = 0f;
            SceneManager.LoadScene("Mike_AI", LoadSceneMode.Single);
        }
    }
}
