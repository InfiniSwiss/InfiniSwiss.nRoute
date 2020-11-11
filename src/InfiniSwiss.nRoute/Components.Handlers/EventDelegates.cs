using System;

namespace nRoute.Components.Handlers
{
    public delegate void EventDelegate<E>(Object sender, E args)
        where
            E : EventArgs;

    public delegate void EventDelegate<T, E>(T @this, Object sender, E args)
        where
            E : EventArgs;
}
