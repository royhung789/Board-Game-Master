using UnityEngine;

/// <summary>
/// interface for canvases which are associated with a certain program state.
/// The type parameters tells the previous/next states
/// </summary>
/// <typeparam name="T">Type of arguments to be passed from previous state</typeparam>>
/// <typeparam name="R">Type of arguments to be passed onto next state</typeparam>>
internal interface IAssociatedState<T, R> : 
    IAssociatedStateEnter<T>, IAssociatedStateLeave<R>
{
    // NOTE: Implementation details are in parent interfaces
}



/// <summary>
/// "Existential type" interface for IAssociatedState.
/// Useful for storing multiple generic IAssociatedState objects in a single container
/// </summary>
internal interface IAssociatedStateObj
{
    /// <summary>
    /// returns canvas associated with program state
    /// </summary>
    /// <returns>The canvas</returns>
    Canvas GetCanvas();

    /// <summary>
    /// Gets the associated state
    /// </summary>
    /// <returns>The associated state</returns>
    ProgramData.State GetAssociatedState();
}



/// <summary>
/// "Existential type" interface for IAssociatedState.
/// For when specification of only previous state type is needed
/// </summary>
internal interface IAssociatedStateEnter<T> : IAssociatedStateObj
{
    /// <summary>
    /// Method to be called when state is switched over to this one
    /// </summary>
    /// <param name="args">arguments passed on from previous state</param>
    void OnEnterState(IAssociatedStateLeave<T> previousState, T args);
}



/// <summary>
/// "Existential type" interface for IAssociatedState.
/// For when specification of only next state type is needed
/// </summary>
internal interface IAssociatedStateLeave<R> : IAssociatedStateObj 
{
    /// <summary>
    /// Method ot be called upon switching over from this state
    /// </summary>
    /// <returns>Parameters to pass onto next state</returns>
    R OnLeaveState(IAssociatedStateEnter<R> nextState);
}


