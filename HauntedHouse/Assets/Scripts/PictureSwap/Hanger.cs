using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hanger : MonoBehaviour
{
    [SerializeField] private Transform[]    m_neighbors;

    public List< FamilyMember > Neighbors
    {
        get
        {
            List< FamilyMember > familyMembers = new List< FamilyMember >();

            foreach (Transform neighbor in m_neighbors)
            {
                familyMembers.Add(neighbor.GetComponentInChildren< Picture >().FamilyMember);
            }

            return familyMembers;
        }
    }
}