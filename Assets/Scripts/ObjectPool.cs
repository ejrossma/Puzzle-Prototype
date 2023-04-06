using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    public List<GameObject> pooledCircles;
    public GameObject circleToPool;
    public List<GameObject> pooledSquares;
    public GameObject squareToPool;
    public List<GameObject> pooledStars;
    public GameObject starToPool;
    public List<GameObject> pooledTriangles;
    public GameObject triangleToPool;
    public int amountToPool;

    public Transform poolContainer;

    void Awake()
    {
        pooledCircles = new List<GameObject>();
        pooledSquares = new List<GameObject>();
        pooledStars = new List<GameObject>();
        pooledTriangles = new List<GameObject>();

        GameObject tmp;

        //circles
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(circleToPool);
            tmp.transform.parent = poolContainer;
            tmp.SetActive(false);
            pooledCircles.Add(tmp);
        }

        //squares
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(squareToPool);
            tmp.transform.parent = poolContainer;
            tmp.SetActive(false);
            pooledSquares.Add(tmp);
        }

        //stars
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(starToPool);
            tmp.transform.parent = poolContainer;
            tmp.SetActive(false);
            pooledStars.Add(tmp);
        }

        //triangles
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(triangleToPool);
            tmp.transform.parent = poolContainer;
            tmp.SetActive(false);
            pooledTriangles.Add(tmp);
        }
    }

    //0 = circle
    //1 = square
    //2 = star
    //3 = triangle
    const int circle = 0;
    const int square = 1;
    const int star = 2;
    const int triangle = 3;
    public GameObject GetPooledObject(int objectType)
    {
        switch (objectType)
        {
            case circle:
               for (int i = 0; i < amountToPool; i++)
                {
                    if (!pooledCircles[i].activeInHierarchy)
                        return pooledCircles[i];
                }
                break;
            case square:
               for (int i = 0; i < amountToPool; i++)
                {
                    if (!pooledSquares[i].activeInHierarchy)
                        return pooledSquares[i];
                }
                break;
            case star:
               for (int i = 0; i < amountToPool; i++)
                {
                    if (!pooledStars[i].activeInHierarchy)
                        return pooledStars[i];
                }
                break;
            case triangle:
               for (int i = 0; i < amountToPool; i++)
                {
                    if (!pooledTriangles[i].activeInHierarchy)
                        return pooledTriangles[i];
                }
                break;
            default:
                break;
        }
        return null;
    }
}
