public class MineAction : ActionBehavior
{
    private Block targetBlock;

    protected override bool Check()
    {
        return targetBlock.Destroyed;
    }

    public override void StartAction()
    {
        targetBlock = Target.GetComponent<Block>();
        targetBlock.StartMining();
    }
    public override void StopAction() { }
}
