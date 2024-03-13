using System;
using UnityEngine;

/// <summary>
/// Basisklasse, die von den entsprechenden Controllern der Spieler bzw. Computergegner implementiert wird.
/// </summary>
public abstract class BallSwitch : MonoBehaviour
{
    public abstract event EventHandler<BallUpdatesEventArgs> OnBallRemoved;
    public abstract event EventHandler<BallUpdatesEventArgs> OnBallUpdated;
    public abstract event EventHandler<BallSwitchEventArgs> OnBallSwitched;

    public class BallUpdatesEventArgs
    {
        public int index;
    }

    public class BallSwitchEventArgs
    {
        public int currentIndex;
        public int? lastIndex;
    }
}
