using UnityEngine;
using System.Collections;

public class SoundTest : MonoBehaviour
{
    public AudioSource source;
    public AudioListener ears;

    public Vector3[] positions;

    RaycastHit[] hits;
    private bool play = false;

    void Update()
    {
        hits = Physics.RaycastAll(source.transform.position, ears.transform.position - source.transform.position);
        /// Draw ray from sound to ears
        Debug.DrawRay(source.transform.position, -(source.transform.position - ears.transform.position), Color.magenta, 10);
        /// Draw line from ears to forward
        Debug.DrawLine(ears.transform.position, ears.transform.position + transform.forward, Color.red, 10);

        float angleToHit = AngleOfHit();
       

        float leftRight = LeftToRight();

        //source.panStereo = leftRight;

        source.gameObject.GetComponent<AudioLowPassFilter>().cutoffFrequency = angleToHit - ObstacleMod();

        if (Input.GetMouseButton(0) /*&& Vector3.Distance(source.transform.position, ears.transform.position) < 5*/)
        {
            if (!source.isPlaying)
            {   
                source.Play();
            }
        }


        if (Input.GetKey(KeyCode.Keypad8))
        {
            source.transform.position = positions[0];
            play = true;
        }
        if (Input.GetKey(KeyCode.Keypad2))
        {
            source.transform.position = positions[1];
            play = true;
        }
        if (Input.GetKey(KeyCode.Keypad4))
        {
            source.transform.position = positions[2];
            play = true;
        }
        if (Input.GetKey(KeyCode.Keypad6))
        {
            source.transform.position = positions[3];
            play = true;
        }
        if (Input.GetKey(KeyCode.Keypad9))
        {
            source.transform.position = positions[4];
            play = true;
        }
        if (Input.GetKey(KeyCode.Keypad3))
        {
            source.transform.position = positions[5];
            play = true;
        }

        if (!source.isPlaying && play)
        {
            source.Play();
            play = false;
        }

    }

    private float AngleOfHit()
    {
        Vector3 hitpoint = hits[hits.Length - 1].point;
        Vector3 forwardPos = ears.transform.position + transform.forward;
        float angle = Vector3.Angle((ears.transform.position + transform.forward) - ears.transform.position, hitpoint - ears.transform.position);
        /// Print angle of the hit
        Debug.Log("Angle of hit from forward: " + angle);
        return (22000 - (17000 * (angle / 180)));
    }

    private float LeftToRight()
    {
        float ratio = 0;
        Vector3 inverseHit = transform.InverseTransformPoint(hits[hits.Length - 1].point);
        ratio = Mathf.Atan2(inverseHit.x, inverseHit.z) * Mathf.Rad2Deg;
        Debug.Log("Left Right Ratio; " + ratio);
        float leftOverRatio = Mathf.Abs(ratio) - 90;
        ratio = 0 + (ratio / 90);
        ratio = -ratio;

        return ratio;
    }

    private float ObstacleMod()
    {
        float modAmount = 0;

        for(int i = 0; i < hits.Length; ++i)
        {
            switch(LayerMask.LayerToName(hits[i].collider.gameObject.layer))
            {
                case "Wood":
                    modAmount += 22000 * 0.05f;
                    break;
                case "Rock":
                    modAmount += 22000 * 0.25f;
                    break;
                case "Ice":
                    modAmount += 22000 * 0.01f;
                    break;
            }
        }

        return modAmount;
    }

    void OnDrawGizmos()
    {
        if (hits != null)
        {
            Gizmos.DrawSphere(hits[hits.Length - 1].point, 0.1f);
            Gizmos.DrawSphere(ears.transform.position + transform.forward, 0.1f);
        }
    }
}
