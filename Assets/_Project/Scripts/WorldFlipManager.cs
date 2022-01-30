using System;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts
{
    public class WorldFlipManager : SingletonBehaviour<WorldFlipManager>
    {
        [SerializeField] private GameObject[] LightObjects;
        [SerializeField] private GameObject[] DarkObjects;

        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private float playerYAnchor;

        public bool IsWorldFlipped;

        private void Start()
        {
            CalculatePlayerYAnchor();
            foreach (var obj in DarkObjects)
            {
                obj.SetActive(false);
            }
        }

        private void CalculatePlayerYAnchor()
        {
            GameObject lightPlayer = LightObjects.Single(x => x.CompareTag("Player"));
            GameObject darkPlayer = DarkObjects.Single(x => x.CompareTag("Player"));
            
            float lightY = lightPlayer.transform.position.y;
            float darkY = darkPlayer.transform.position.y;
            
            playerYAnchor = lightY + (darkY - lightY) / 2;
        }

        public void FlipWorld()
        {
            IsWorldFlipped = !IsWorldFlipped;

            Vector2 previousPlayerPosition = new Vector2();
            bool savedPlayerFlip = false;

            if (IsWorldFlipped)
            {
                foreach (var obj in LightObjects)
                {
                    if (obj.CompareTag("Player"))
                    {
                        previousPlayerPosition = obj.transform.position;
                        savedPlayerFlip = obj.GetComponent<PlayerController>().isFlipped;
                    }

                    obj.SetActive(false);
                }

                foreach (var obj in DarkObjects)
                {
                    obj.SetActive(true);
                    if (obj.CompareTag("Player"))
                    {
                        Vector2 anchorPosition = new Vector2(previousPlayerPosition.x, playerYAnchor);
                        Vector2 newPosition;
                        newPosition.x = previousPlayerPosition.x;
                        newPosition.y = playerYAnchor - Vector2.Distance(anchorPosition, previousPlayerPosition);
                        obj.transform.position = newPosition;
                        
                        obj.GetComponent<PlayerController>().FlipDirection(savedPlayerFlip);
                    }
                }
            }
            else
            {
                foreach (var obj in DarkObjects)
                {
                    if (obj.CompareTag("Player"))
                    {
                        previousPlayerPosition = obj.transform.position;
                        savedPlayerFlip = obj.GetComponent<PlayerController>().isFlipped;
                    }

                    obj.SetActive(false);
                }

                foreach (var obj in LightObjects)
                {
                    obj.SetActive(true);
                    if (obj.CompareTag("Player"))
                    {
                        Vector2 anchorPosition = new Vector2(previousPlayerPosition.x, playerYAnchor);
                        Vector2 newPosition;
                        newPosition.x = previousPlayerPosition.x;
                        newPosition.y = playerYAnchor + Vector2.Distance(anchorPosition, previousPlayerPosition);
                        obj.transform.position = newPosition;
                        
                        obj.GetComponent<PlayerController>().FlipDirection(savedPlayerFlip);
                    }
                }
            }
        }
    }
}