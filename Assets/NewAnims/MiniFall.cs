using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniFall : MonoBehaviour {
    public FallInPieces Fall;
    void Start() {
        if (GetComponent<Shape_Player>()) {
            GameObject miniEnemy = transform.Find("MiniEnemy").gameObject;
            Shape_Player otherPlayer = GameObject.Find(name == "Player1" ? "Player2" : "Player1").GetComponent<Shape_Player>();
            if (otherPlayer is Cube_Player)
                Fall = miniEnemy.transform.Find("MiniCube").GetComponent<FallInPieces>();
            else if (otherPlayer is Pyramid_Player)
                Fall = miniEnemy.transform.Find("MiniPyramid").GetComponent<FallInPieces>();
            else if (otherPlayer is Star_Player)
                Fall = miniEnemy.transform.Find("MiniStar").GetComponent<FallInPieces>();
            else if (otherPlayer is Sphere_Player)
                Fall = miniEnemy.transform.Find("MiniSphere").GetComponent<FallInPieces>();
        }
    }
    public void fall() { 
        Fall.Fall();
    }
}
