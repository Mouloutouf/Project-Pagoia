using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class Block : Entity
{
    [MinMaxSlider(1, 10)]
    public Vector2Int resourceAmountMinMax;

    public Transform spawnParent;
    public GameObject resourcePrefab;
    
    public float spawnHeight;
    public float spawnRange;

    public bool Destroyed { get; private set; }

    public LoadingBar loadingBar;
    
    protected override void Start()
    {
        base.Start();
        
        Destroyed = false;
        loadingBar.slider.gameObject.SetActive(false);
    }
    
    public void StartMining()
    {
        if (Destroyed == true)
            return;

        loadingBar.slider.gameObject.SetActive(true);
        
        loadingBar._afterLoadAction_ = MineBlock;
        StartCoroutine(loadingBar.Load());
    }

    private void MineBlock()
    {
        // Play Feedback / Animation

        int resourceAmount = UnityRandom.Range(resourceAmountMinMax.x, resourceAmountMinMax.y + 1);

        // Spread / Blow Animation
        
        for (var i = 0; i < resourceAmount; i++)
        {
            var randomPos = UnityRandom.insideUnitCircle * spawnRange;
            var worldPos = new Vector3(randomPos.x, spawnHeight, randomPos.y);
            var spawnPos = transform.position + worldPos;
            
            Instantiate(resourcePrefab, spawnPos, Quaternion.identity, spawnParent);
        }

        Destroyed = true;
        gameObject.SetActive(false);
        
        // TODO Add state 'destroyed by'
        // TODO Remove state 'exists'
        // TODO Actually destroy the block instead of deactivating it
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}
