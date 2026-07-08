using System;
using System.Collections.Generic;

namespace FSCORD.Core
{
    /// <summary>
    /// Lightweight typed publish/subscribe bus. Replaces the 2015 code's
    /// GameObject.Find lookups and direct cross-script references: systems
    /// raise and listen for plain-struct events (see GameEvents) and never
    /// reach into each other's internals.
    /// </summary>
    public sealed class EventBus
    {
        readonly Dictionary<Type, Delegate> _handlers = new();

        public void Subscribe<T>(Action<T> handler) where T : struct
        {
            if (handler == null) return;
            _handlers.TryGetValue(typeof(T), out var existing);
            _handlers[typeof(T)] = (existing as Action<T>) + handler;
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            if (handler == null) return;
            if (!_handlers.TryGetValue(typeof(T), out var existing)) return;
            var updated = (existing as Action<T>) - handler;
            if (updated == null) _handlers.Remove(typeof(T));
            else _handlers[typeof(T)] = updated;
        }

        public void Publish<T>(T evt) where T : struct
        {
            if (_handlers.TryGetValue(typeof(T), out var d))
                (d as Action<T>)?.Invoke(evt);
        }

        public void Clear() => _handlers.Clear();
    }
}
