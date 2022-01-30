using System;
using UnityEngine;

namespace _Project.Scripts
{
    public class WorldFlipManager : SingletonBehaviour<WorldFlipManager>
    {
        [SerializeField]
        private GameObject[] LightObjects;
        [SerializeField]
        private GameObject[] DarkObjects;

        public bool IsWorldFlipped;

        public void FlipWorld()
        {
            IsWorldFlipped = !IsWorldFlipped;

            float savedPlayerX = 0f;
            
            if (IsWorldFlipped)
            {
                foreach (var obj in LightObjects)
                {
                    if (obj.CompareTag("Player"))
                    {
                        savedPlayerX = obj.transform.position.x;
                    }
                    obj.SetActive(false);
                }
                foreach (var obj in DarkObjects)
                {
                    obj.SetActive(true);
                    if (obj.CompareTag("Player"))
                    {
                        obj.transform.position = new Vector3(savedPlayerX, obj.transform.position.y);
                    }
                }
            }
            else
            {
                foreach (var obj in DarkObjects)
                {
                    if (obj.CompareTag("Player"))
                    {
                        savedPlayerX = obj.transform.position.x;
                    }
                    obj.SetActive(false);
                }
                foreach (var obj in LightObjects)
                {
                    obj.SetActive(true);
                    if (obj.CompareTag("Player"))
                    {
                        obj.transform.position = new Vector3(savedPlayerX, obj.transform.position.y);
                    }
                }
            }
        }
    }
}