using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Health : NetworkBehaviour
{
    private SpriteRenderer renderer;
    [SerializeField] private Sprite healthFull;
    [SerializeField] private Sprite healthEmpty;

    List<VisualElement> healthSprites = new List<VisualElement>();

    private Rigidbody2D rb;

    [SyncVar(Channel = Channel.Unreliable, ReadPermissions = ReadPermission.Observers, SendRate = 0.1f, OnChange = nameof(SyncHealth))]
    private float health = 3;

    private UIDocument gameMenu;

    private ScreenShake shaker;


    private void SyncHealth(float prev, float next, bool asServer)
    {
        if (!asServer)
        {
            GetComponent<Health>().health = next;
        }
    }

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        shaker = Camera.main.GetComponent<ScreenShake>();
        gameMenu = GameObject.Find("UIManager").GetComponent<UIDocument>();

        healthSprites.Add(gameMenu.rootVisualElement.Q<VisualElement>("HealthIcon1"));
        healthSprites.Add(gameMenu.rootVisualElement.Q<VisualElement>("HealthIcon2"));
        healthSprites.Add(gameMenu.rootVisualElement.Q<VisualElement>("HealthIcon3"));
    }

    private void Update()
    {
        if (!base.IsOwner) return;

        var healthEmptyBg = new StyleBackground(healthEmpty);
        var healthFullBg = new StyleBackground(healthFull);

        if(healthSprites[(int)health] != null || health == 3) healthSprites[(int)health].style.backgroundImage = healthEmptyBg;

        for (int i = 0; i < health; i++)
        {
            healthSprites[i].style.backgroundImage = healthFullBg;
        }
      
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damager")
        {
            StartCoroutine(TakeDamage());

            if(base.IsOwner)
            {
                shaker.start = true;
                if (health != 0) health--;
            }

            if (collision.gameObject.name == "Bottom") rb.AddForce(new Vector2(Random.Range(-20, 20), 25), ForceMode2D.Impulse);
        }
    }

    IEnumerator TakeDamage()
    {
        renderer.material.SetInt("_Hit", 1);
        yield return new WaitForSeconds(0.2f);
        renderer.material.SetInt("_Hit", 0);
    }
}
