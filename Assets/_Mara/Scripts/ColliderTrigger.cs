using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
   public Transform DarkChar;

    private void  OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Lightcol") { 
            DarkChar.gameObject.SetActive(true);
            DarkChar.position = new Vector3(this.transform.position.x, DarkChar.position.y, DarkChar.position.z);
        }

        if(collision.gameObject.tag == "Water") { 
            
            //flip is enable

        }

        if(collision.gameObject.tag == "Alter") { 
            
            Application.LoadLevel("level_1");

        }

    }

    private void  OnTriggerExit2D(Collider2D collision)
    {
    
        if(collision.gameObject.tag == "Lightcol") { 
            DarkChar.gameObject.SetActive(false);

        }

        if(collision.gameObject.tag == "Water") { 
            
            //flip is disable

        }
    }
}
