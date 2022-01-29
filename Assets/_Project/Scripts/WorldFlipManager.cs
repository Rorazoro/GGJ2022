using System;
using UnityEngine;

namespace _Project.Scripts
{
    public class WorldFlipManager : SingletonBehaviour<WorldFlipManager>
    {
        [SerializeField] private GameObject[] LightObjects;
        [SerializeField] private GameObject[] DarkObjects;

        [SerializeField] private LayerMask groundLayerMask;

        public bool IsWorldFlipped;

        public void FlipWorld()
        {
            IsWorldFlipped = !IsWorldFlipped;

            float savedPlayerX = 0f;
            bool savedPlayerFlip = false;

            if (IsWorldFlipped)
            {
                foreach (var obj in LightObjects)
                {
                    if (obj.CompareTag("Player"))
                    {
                        savedPlayerX = obj.transform.position.x;
                        savedPlayerFlip = obj.GetComponent<PlayerController>().isFlipped;
                    }

                    obj.SetActive(false);
                }

                foreach (var obj in DarkObjects)
                {
                    obj.SetActive(true);
                    if (obj.CompareTag("Player"))
                    {
                        RaycastHit2D hit = Physics2D.Raycast(new Vector2(savedPlayerX, -100), Vector2.up, 200,
                            groundLayerMask);

                        if (hit.collider != null)
                        {
                            Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
                        }
                        else
                        {
                            Debug.Log($"Raycast hit not found!");
                        }

                        obj.transform.position = new Vector3(savedPlayerX, hit.point.y);
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
                        savedPlayerX = obj.transform.position.x;
                        savedPlayerFlip = obj.GetComponent<PlayerController>().isFlipped;
                    }

                    obj.SetActive(false);
                }

                foreach (var obj in LightObjects)
                {
                    obj.SetActive(true);
                    if (obj.CompareTag("Player"))
                    {
                        RaycastHit2D hit = Physics2D.Raycast(new Vector2(savedPlayerX, 100), Vector2.down, 200,
                            groundLayerMask);
                        
                        if (hit.collider != null)
                        {
                            Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
                        }
                        else
                        {
                            Debug.Log($"Raycast hit not found!");
                        }

                        obj.transform.position = new Vector3(savedPlayerX, hit.point.y);
                        obj.GetComponent<PlayerController>().FlipDirection(savedPlayerFlip);
                    }
                }
            }
        }
    }
}