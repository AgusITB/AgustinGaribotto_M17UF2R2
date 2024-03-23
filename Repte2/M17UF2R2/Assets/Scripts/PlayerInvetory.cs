using System.Collections.Generic;
using UnityEngine;

public class PlayerInvetory : MonoBehaviour
{
   [SerializeField] public List<ItemData> itemsCollected = new List<ItemData>();

    public void AddToInventory(ItemData item)
    {
        itemsCollected.Add(item);
    }

    private void BuildInventory()
    {
        itemsCollected = new List<ItemData>( 
          
            
            
            );
    }


    public void RemoveFromInventory(ItemData item)
    {
        itemsCollected.Remove(item);
    }



}
