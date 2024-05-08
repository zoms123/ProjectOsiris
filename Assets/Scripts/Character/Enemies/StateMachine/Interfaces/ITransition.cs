public partial interface ITransition {
    IState To { get; }
    IPredicate Condition { get; }
}