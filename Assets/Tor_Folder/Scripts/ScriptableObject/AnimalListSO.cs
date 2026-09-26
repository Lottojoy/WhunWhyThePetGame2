using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimalList", menuName = "WHUNWHY/Animal List")]
public class AnimalListSO : ScriptableObject
{
    public List<AnimalData> animalList;
}