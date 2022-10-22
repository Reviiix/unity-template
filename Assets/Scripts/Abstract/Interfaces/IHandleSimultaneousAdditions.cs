using System.Collections;

namespace Abstract.Interfaces
{
    /// <summary>
    /// This interface is intended for classes that display and incremental value.
    /// Its intent is to allow value changes to queue up and be displayed after the current value change has finished displaying/animating/rolling up
    /// </summary>
    public interface IHandleSimultaneousAdditions
    { 
        IEnumerator HandleDelayedAdditions();
    }
}
