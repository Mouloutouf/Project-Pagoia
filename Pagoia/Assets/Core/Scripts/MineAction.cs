public class MineAction : ActionBehavior
{
    private OreBlock targetBlock;

    protected override bool Check()
    {
        return targetBlock.Mined;
    }

    public override void StartAction()
    {
        targetBlock = Target.GetComponent<OreBlock>();
        targetBlock.StartMining();
    }
    public override void StopAction() { }
}
