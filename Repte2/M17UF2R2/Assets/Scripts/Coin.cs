using UnityEngine;

public class Coin : Item
{
    public override void Collect()
    {       
        GetComponent<BoxCollider>().enabled = false;    
        GetComponentInChildren<Animator>().enabled = false;
    }

}
