using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class OreBlock : MonoBehaviour
{
    [Header("Range")]
    [MinMaxSlider(1, 10)]
    public Vector2Int minMaxOreAmount;

    [Header("Instantiate")]
    public Transform oreParent;
    public GameObject oreCrystalPrefab;
    public float spawnHeight;
    public float spawnRange;

    [Header("Destroy")]
    public Transform oreModel;
    private bool mined; public bool Mined { get => mined; }

    [Header("Loading")]
    public LoadingBar loadingBar;

    private void Start()
    {
        mined = false;
        loadingBar.slider.gameObject.SetActive(false);
    }

    public void StartMining()
    {
        if (mined) return;

        loadingBar.slider.gameObject.SetActive(true);
        loadingBar._afterLoadAction_ = MineOre;
        StartCoroutine(loadingBar.Load());
    }

    private void MineOre()
    {
        // MMFeedbacks
        // Play Feedback / Animation

        var oreAmount = UnityRandom.Range(minMaxOreAmount.x, minMaxOreAmount.y + 1);

        // DoTween
        // Spread / Blow Animation
        for (var i = 0; i < oreAmount; i++)
        {
            var randomPos = UnityRandom.insideUnitCircle * spawnRange;
            var worldPos = new Vector3(randomPos.x, spawnHeight, randomPos.y);
            var spawnPos = transform.position + worldPos;
            Instantiate(oreCrystalPrefab, spawnPos, Quaternion.identity, oreParent);
        }

        mined = true;

        this.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, spawnRange);
    }
}
