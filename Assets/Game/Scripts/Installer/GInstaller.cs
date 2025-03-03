
using Zenject;

public class GInstaller : MonoInstaller
{
    public override void InstallBindings()
    {

        G.run = new RunState(1);

    }
}