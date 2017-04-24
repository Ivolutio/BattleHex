using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    public bool good;
    public Tile currentTile;
    GameObject cam;
    GameObject mainCam;

    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        cam = transform.FindChild("Camera").gameObject;
    }

    public void StopCam()
    {
        mainCam.gameObject.SetActive(true);
        cam.SetActive(false);
    }

    public IEnumerator StartCam(Pawn pawn, bool player1)
    {
        if (mainCam == null)
            mainCam = GameObject.Find("Main Camera");
        mainCam.gameObject.SetActive(false);
        cam.SetActive(true);
        yield return new WaitForSeconds(.2f);
        transform.FindChild("Chest").GetChild(0).FindChild("PixelChestLid").GetComponent<Animation>()["Cube.001|Cube.001Action.001"].speed *= 2f;
        transform.FindChild("Chest").GetChild(0).FindChild("PixelChestLid").GetComponent<Animation>().Play();
        yield return new WaitForSeconds(transform.FindChild("Chest").GetChild(0).FindChild("PixelChestLid").GetComponent<Animation>()["Cube.001|Cube.001Action.001"].length - 0.2f);
        transform.parent.GetComponent<World>().MovePawn(pawn, currentTile, player1);
        if (good)
        {
            yield return new WaitForSeconds(.5f);
            StopCam();
            yield return new WaitForSeconds(.5f);
            transform.parent.GetComponent<World>().Win(player1);
        }
        else
        {
            StopCam();
            mainCam.GetComponent<AudioPlayer>().Explode();            
            yield return new WaitForSeconds(.5f);
            transform.parent.GetComponent<World>().RemovePawn(pawn, player1);
            Destroy(gameObject);
        }
    }
}
