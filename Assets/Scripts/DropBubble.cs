using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBubble : MonoBehaviour
{
    float Speed = 1f;
    public InGameManager m_pManager = null;

    // Start is called before the first frame update
    public void SetSpeed(float fSpeed)
    {
        Speed = fSpeed; // + Random.RandomRange(-15f, 15f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.localPosition;
        pos.y -= Speed * Time.unscaledDeltaTime;
        transform.localPosition = pos;

        if (pos.y < -850f)
        {
            m_pManager.ClickBubble(gameObject);
            Destroy(gameObject);

        }
    }
}
