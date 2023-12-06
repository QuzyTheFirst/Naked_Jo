using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    public enum TurnWind
    {
        On,
        Off
    }

    [SerializeField] private TurnWind _turnWind;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            switch (_turnWind)
            {
                case TurnWind.On:
                    Debug.Log("Turned on wind");
                    SoundManager.Instance.FadeInVolume("Wind", 1, .5f);
                    break;
                case TurnWind.Off:
                    Debug.Log("Turned off wind");
                    SoundManager.Instance.FadeAwayVolume("Wind", .5f);
                    break;
            }
        }
    }
}
