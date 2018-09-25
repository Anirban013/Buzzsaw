using UnityEngine;
using System.Collections;
using System;

public class PointStar : MonoBehaviour, IPlayerRespawnListener {

    public GameObject effect;
    public int poinstToAdd = 10;

    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);
    }

    // Use this for initialization
    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.GetComponent<Player>() == null)
            return;

        Instantiate(effect, transform.position, transform.rotation);
        GameManager.Instance.AddPoints(poinstToAdd);

        FloatingText.Show(string.Format("+{0}", poinstToAdd), "PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
        gameObject.SetActive(false);
    }
}
