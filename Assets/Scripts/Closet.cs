using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Closet : MonoBehaviour, IInteractable
{
    [SerializeField] private CostumeChanger.Costumes _joCostume;

    public bool Interaction(UnitsHandler unitsHandler)
    {
        unitsHandler.ChangeJOAppearance(_joCostume);
        return true;
    }
}
