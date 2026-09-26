using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StationListSO", menuName = "WHUNWHY/Station List")]
public class StationListSO : ScriptableObject
{
    public List<StationData> stationList;
}