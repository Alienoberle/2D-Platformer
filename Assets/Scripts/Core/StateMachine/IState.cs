public interface IState 
{
    string name {get; }
    void Tick();
    void OnEnter();
    void OnExit();
}
