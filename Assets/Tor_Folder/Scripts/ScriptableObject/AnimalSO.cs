using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AnimalSO : ScriptableObject
{
    public string animalName;
    public Transform prefab;
    public Sprite icon;
    public AudioClip sound;
    public float moveSpeed;
}
