using Cysharp.Threading.Tasks;
public interface IClickCommand
{
    UniTask<bool> Execute();
}
