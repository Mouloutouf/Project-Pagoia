using UnityEngine;

public class CraftItem : MonoBehaviour
{
    public CraftingTable craftingTable;

    public Entity itemEntity { get; set; }

    public LoadingBar loadingBar;

    public CraftRecipe recipe { get; set; }

    public void StartCrafting()
    {
        loadingBar.slider.gameObject.SetActive(true);
        loadingBar._afterLoadAction_ = Craft;
        StartCoroutine(loadingBar.Load());
    }

    public void Craft()
    {
        itemEntity = Instantiate(recipe.itemPrefab, craftingTable.craftSpawn).GetComponent<Entity>();
    }
}
