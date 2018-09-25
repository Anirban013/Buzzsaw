using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

    // Use this for initialization
    public Player player;
    public Transform foregroundSprite;
    public SpriteRenderer foregroundRenderer;
    public Color MaxHealthColor = new Color(255 / 255f, 63 / 255f, 63 / 255f);
    public Color MinHealthColor = new Color(64 / 255f, 137 / 255f, 255 / 255f);
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var healthPercent = player.Health / (float)player.maxHealth;

        foregroundSprite.localScale = new Vector3(healthPercent, 1, 1);
        foregroundRenderer.color = Color.Lerp(MaxHealthColor, MinHealthColor, healthPercent);
	}
}
