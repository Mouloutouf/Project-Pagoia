public class CraftAction : ActionBehavior
{
    private CraftItem craftBehavior;
    public CraftRecipe currentRecipe { get; set; }

    protected override bool Check() { return false; }

    public override void StartAction()
    {
        craftBehavior = Target.GetComponent<CraftItem>();
        craftBehavior.recipe = currentRecipe;
        craftBehavior.StartCrafting();
    }

    public override void StopAction() { }
}
