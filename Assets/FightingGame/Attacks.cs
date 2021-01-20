using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HitboxVal
{
    public int id;
    public int boneID;
    public Vector2 size;
    public Vector2 offset;
    /*
     * Angle
     * Attribute (Slash/Effect)
     * SoundFX
     */ 
}

[CreateAssetMenu]
public class Attacks : ScriptableObject
{

    public List<HitboxVal> hitboxes;
}

/*
public class A : MonoBehaviour
{
    public static List<e> list = new List<e>() { RemoveHitbox };
    public static List<int> list2 = new List<int>() { f };

    public int f = 2;

    List<e> seq;
    void init()
    {
        seq = new List<e>();
        seq.Add()
    }

    public delegate void e();
    private int finInt;



    public void CreateHitbox(HitboxVal val)
    {
        // gör fin kod här:)
        finInt = 0;
    }

    public void RemoveHitbox()
    {
        List<IEnumerator> enums;
        // mer bra kod:)
        finInt = 0;
    }


    IEnumerator rrr()
    {
        rrr2();
        yield return 0;
    }

    IEnumerator rrr2()
    {

        yield return new WaitForFixedUpdate();
    }

    public void Loop(int count, List<e> sequence)
    {
        for (int i = 0; i < count; i++)
        {
            foreach (var item in sequence)
            {
                item.Invoke();
            }
        }
    }
}

public class B
{
    public void v()
    {
        List<A.e> l = new List<A.e>();

        l.Add(A.RemoveHitbox);

        foreach (var item in l)
        {
            item.Invoke();
        }    
    }
}
*/