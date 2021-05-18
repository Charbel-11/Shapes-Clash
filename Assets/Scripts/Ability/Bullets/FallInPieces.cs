using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallInPieces : MonoBehaviour
{
    public GameObject miniCube;
    public GameObject miniCubeThisPlayer;
    public GameObject miniCubeOtherPlayer;
    Transform pos;

    private Shape_Player player;
    private Shape_Player otherPlayer;
    private GameMaster GM;

    private void Start()
    {
        if (GameObject.Find("Game Manager")) GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        if (gameObject.transform.parent != null && gameObject.transform.parent.name == "MiddleRubble")
        {
            player = GameObject.Find("Player1").GetComponent<Shape_Player>();
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
            if (otherPlayer is Cube_Player)
                miniCubeOtherPlayer = GM.miniCubeEarth;
            else if (otherPlayer is Pyramid_Player)
                miniCubeOtherPlayer = GM.miniCubeWater;
            else if (otherPlayer is Star_Player)
                miniCubeOtherPlayer = GM.miniCubeFire;
            else if (otherPlayer is Sphere_Player)
                miniCubeOtherPlayer = GM.miniCubeWind;

            if (player is Cube_Player)
                miniCubeThisPlayer = GM.miniCubeEarth;
            else if (player is Pyramid_Player)
                miniCubeThisPlayer = GM.miniCubeWater;
            else if (player is Star_Player)
                miniCubeThisPlayer = GM.miniCubeFire;
            else if (player is Sphere_Player)
                miniCubeThisPlayer = GM.miniCubeWind;
        }
    }

    public void dead()
    {
        player = GetComponent<Shape_Player>();
        if (player is Cube_Player)
            miniCubeThisPlayer = GM.miniCubeEarth;
        else if (player is Pyramid_Player)
            miniCubeThisPlayer = GM.miniCubeWater;
        else if (player is Star_Player)
            miniCubeThisPlayer = GM.miniCubeFire;
        else if (player is Sphere_Player)
            miniCubeThisPlayer = GM.miniCubeWind;
        
        pos = gameObject.GetComponent<Transform>();
        for (int i = 0; i < 100; i++)
        {
            Instantiate(miniCubeThisPlayer, new Vector3(pos.position.x + (i * 0.05f), pos.position.y, pos.position.z + (i * 0.05f)), pos.rotation);
        }
        for (int i = 0; i < 100; i++)
        {
            Instantiate(miniCubeThisPlayer, new Vector3(pos.position.x + 5 - (i * 0.05f), pos.position.y, pos.position.z + 5 - (i * 0.05f)), pos.rotation);
        }
    }

    public void Fall()
    {
        player = GetComponentInParent<Shape_Player>();

        if (player != null)
        {
            if (player.name == "Player1")
                otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
            else
                otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
        }

        if (Sealingscript.playerHurt)
        {
            if (otherPlayer is Cube_Player)
                miniCubeOtherPlayer = GM.miniCubeEarth;
            else if (otherPlayer is Pyramid_Player)
                miniCubeOtherPlayer = GM.miniCubeWater;
            else if (otherPlayer is Star_Player)
                miniCubeOtherPlayer = GM.miniCubeFire;
            else if (otherPlayer is Sphere_Player)
                miniCubeOtherPlayer = GM.miniCubeWind;

            pos = gameObject.GetComponent<Transform>();
            for (int i = 0; i < 4; i++)
            {
                if (i < 2)
                    Instantiate(miniCubeOtherPlayer, new Vector3(pos.position.x + (i * 0.05f), pos.position.y, pos.position.z + (i * 0.05f)), pos.rotation);
                else if (i == 2)
                    Instantiate(miniCubeOtherPlayer, new Vector3(pos.position.x - 0.05f, pos.position.y, pos.position.z + 0.04f), pos.rotation);
                else
                    Instantiate(miniCubeOtherPlayer, new Vector3(pos.position.x - 0.007f, pos.position.y, pos.position.z + 0.1f), pos.rotation);
            }
            Sealingscript.playerHurt = false;
        }
        else if (Sealingscript.playerEscape)
        {
            if (player is Cube_Player)
                miniCubeThisPlayer = GM.miniCubeEarth;
            else if (player is Pyramid_Player)
                miniCubeThisPlayer = GM.miniCubeWater;
            else if (player is Star_Player)
                miniCubeThisPlayer = GM.miniCubeFire;
            else if (player is Sphere_Player)
                miniCubeThisPlayer = GM.miniCubeWind;

            pos = gameObject.GetComponent<Transform>();
            for (int i = 0; i < 4; i++)
            {
                //Added 1 to the y position for the tackle busting through cube sealing, the mini cubes would go underground otherwise
                if (i < 2)
                    Instantiate(miniCubeThisPlayer, new Vector3(pos.position.x + (i * 0.05f), 1 + pos.position.y, pos.position.z + (i * 0.05f)), pos.rotation);
                else if (i == 2)
                    Instantiate(miniCubeThisPlayer, new Vector3(pos.position.x - 0.05f, 1 + pos.position.y, pos.position.z + 0.04f), pos.rotation);
                else
                    Instantiate(miniCubeThisPlayer, new Vector3(pos.position.x - 0.007f, 1 + pos.position.y, pos.position.z + 0.1f), pos.rotation);
            }
            Sealingscript.playerEscape = false;
        }
        else if (gameObject.name == "Assist")
        {
            pos = gameObject.transform.Find("position").GetComponent<Transform>();
            for (int i = 0; i < 4; i++)
            {
                if (i < 2)
                    Instantiate(miniCube, new Vector3(pos.position.x + (i * 0.05f), pos.position.y, pos.position.z + (i * 0.05f)), pos.rotation);
                else if (i == 2)
                    Instantiate(miniCube, new Vector3(pos.position.x - 0.05f, pos.position.y, pos.position.z + 0.04f), pos.rotation);
                else
                    Instantiate(miniCube, new Vector3(pos.position.x - 0.007f, pos.position.y, pos.position.z + 0.1f), pos.rotation);
            }
        }
        else
        {
            pos = gameObject.GetComponent<Transform>();
            for (int i = 0; i < 4; i++)
            {
                if (i < 2)
                    Instantiate(miniCube, new Vector3(pos.position.x + (i * 0.05f), pos.position.y, pos.position.z + (i * 0.05f)), pos.rotation);
                else if (i == 2)
                    Instantiate(miniCube, new Vector3(pos.position.x - 0.05f, pos.position.y, pos.position.z + 0.04f), pos.rotation);
                else
                    Instantiate(miniCube, new Vector3(pos.position.x - 0.007f, pos.position.y, pos.position.z + 0.1f), pos.rotation);
            }
        }
    }
    public void SpecialFall(bool Addwater = false)
    {
        pos = gameObject.GetComponent<Transform>();
        for (int i = 0; i < 4; i++)
        {
            if (i < 2)
                Instantiate(miniCubeOtherPlayer, new Vector3(pos.position.x + (i * 0.05f), pos.position.y, pos.position.z + (i * 0.05f)), pos.rotation);
            else if (i == 2)
                Instantiate(miniCubeOtherPlayer, new Vector3(pos.position.x - 0.05f, pos.position.y, pos.position.z + 0.04f), pos.rotation);
            else
                Instantiate(miniCubeOtherPlayer, new Vector3(pos.position.x - 0.007f, pos.position.y, pos.position.z + 0.1f), pos.rotation);
        }
        for (int i = 0; i < 4; i++)
        {
            //Added 1 to the y position for the tackle busting through cube sealing, the mini cubes would go underground otherwise
            if (i < 2)
                Instantiate(miniCubeThisPlayer, new Vector3(pos.position.x + (i * 0.05f), 1 + pos.position.y, pos.position.z + (i * 0.05f)), pos.rotation);
            else if (i == 2)
                Instantiate(miniCubeThisPlayer, new Vector3(pos.position.x - 0.05f, 1 + pos.position.y, pos.position.z + 0.04f), pos.rotation);
            else
                Instantiate(miniCubeThisPlayer, new Vector3(pos.position.x - 0.007f, 1 + pos.position.y, pos.position.z + 0.1f), pos.rotation);
        }
        if ((player is Pyramid_Player || otherPlayer is Pyramid_Player) && gameObject.name == "Water" && Addwater)
        {
            gameObject.GetComponent<ParticleSystem>().Play();
        }
    }
}
