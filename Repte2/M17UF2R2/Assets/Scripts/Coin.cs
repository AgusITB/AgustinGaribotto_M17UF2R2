using UnityEngine;

public class Coin : Item
{
    public override void Collect()
    {
        GetComponent<BoxCollider>().enabled = false;    
        
        this.transform.localScale = Vector3.one/2;
        
        Debug.Log("Coin picked up");
    }

}
