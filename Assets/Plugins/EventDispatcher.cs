using System;
using System.Collections.Generic;
using BeauUtil;

namespace BeauUtil.Extensions {
    public sealed class EventDispatcher<TArg> {
        #region Types

        /// <summary>
        /// Small struct for holding a queued event.
        /// </summary>
        private struct QueuedEvent {
            public readonly StringHash32 Id;
            public readonly TArg Argument;

            public QueuedEvent(StringHash32 id, TArg argument) {
                Id = id;
                Argument = argument;
            }
        }

        #endregion // Types

        private readonly Dictionary<StringHash32, CastableEvent<TArg>> m_Handlers = new Dictionary<StringHash32, CastableEvent<TArg>>();
        private readonly RingBuffer<CastableEvent<TArg>> m_FreeHandlers = new RingBuffer<CastableEvent<TArg>>();
        private readonly RingBuffer<QueuedEvent> m_QueuedEvents = new RingBuffer<QueuedEvent>(16, RingBufferMode.Expand);
        private readonly List<StringHash32> m_TempEventList = new List<StringHash32>();

        #region Registration

        /// <summary>
        /// Registers an event handler, optionally bound to a given object.
        /// </summary>
        public EventDispatcher<TArg> Register(StringHash32 eventId, Action inAction, UnityEngine.Object binding = null) {
            CastableEvent<TArg> block = GetBlock(eventId, true);
            block.Register(inAction, binding);
            return this;
        }

        /// <summary>
        /// Registers an event handler, optionally bound to a given object.
        /// </summary>
        public EventDispatcher<TArg> Register(StringHash32 eventId, Action<object> inActionWithContext, UnityEngine.Object binding = null) {
            CastableEvent<TArg> block = GetBlock(eventId, true);
            block.Register(inActionWithContext, binding);
            return this;
        }

        /// <summary>
        /// Registers an event handler, optionally bound to a given object.
        /// </summary>
        public EventDispatcher<TArg> Register<U>(StringHash32 eventId, Action<U> inActionWithCastedContext, UnityEngine.Object binding = null) {
            CastableEvent<TArg> block = GetBlock(eventId, true);
            block.Register(inActionWithCastedContext, binding);
            return this;
        }

        /// <summary>
        /// Deregisters an event handler.
        /// </summary>
        public EventDispatcher<TArg> Deregister(StringHash32 eventId, Action action) {
            CastableEvent<TArg> block;
            if (m_Handlers.TryGetValue(eventId, out block)) {
                block.Deregister(action);
                if (block.IsEmpty) {
                    m_Handlers.Remove(eventId);
                    m_FreeHandlers.PushBack(block);
                }
            }

            return this;
        }

        /// <summary>
        /// Deregisters an event handler.
        /// </summary>
        public EventDispatcher<TArg> Deregister(StringHash32 eventId, Action<object> action) {
            CastableEvent<TArg> block;
            if (m_Handlers.TryGetValue(eventId, out block)) {
                block.Deregister(action);
                if (block.IsEmpty) {
                    m_Handlers.Remove(eventId);
                    m_FreeHandlers.PushBack(block);
                }
            }

            return this;
        }

        /// <summary>
        /// Deregisters an event handler.
        /// </summary>
        public EventDispatcher<TArg> Deregister<T>(StringHash32 eventId, Action<T> castedAction) {
            CastableEvent<TArg> block;
            if (m_Handlers.TryGetValue(eventId, out block)) {
                block.Deregister(castedAction);
                if (block.IsEmpty) {
                    m_Handlers.Remove(eventId);
                    m_FreeHandlers.PushBack(block);
                }
            }

            return this;
        }

        /// <summary>
        /// Deregisters all handlers for the given event.
        /// </summary>
        public EventDispatcher<TArg> DeregisterAll(StringHash32 eventId) {
            CastableEvent<TArg> block;
            if (m_Handlers.TryGetValue(eventId, out block)) {
                block.Clear();
                m_Handlers.Remove(eventId);
                m_FreeHandlers.PushBack(block);
            }

            return this;
        }

        /// <summary>
        /// Deregisters all handlers associated with the given binding.
        /// </summary>
        public EventDispatcher<TArg> DeregisterAll(UnityEngine.Object binding) {
            if (binding.IsReferenceNull()) {
                return this;
            }

            m_TempEventList.Clear();
            foreach (var blockKV in m_Handlers) {
                blockKV.Value.DeregisterAll(binding);
                if (blockKV.Value.IsEmpty) {
                    m_FreeHandlers.PushBack(blockKV.Value);
                    m_TempEventList.Add(blockKV.Key);
                }
            }

            foreach(var tempEventId in m_TempEventList) {
                m_Handlers.Remove(tempEventId);
            }

            m_TempEventList.Clear();
            return this;
        }

        private CastableEvent<TArg> GetBlock(StringHash32 eventId, bool createIfNotThere) {
            CastableEvent<TArg> block;
            if (!m_Handlers.TryGetValue(eventId, out block) && createIfNotThere) {
                if (m_FreeHandlers.Count > 0) {
                    block = m_FreeHandlers.PopBack();
                } else {
                    block = new CastableEvent<TArg>();
                }
                m_Handlers.Add(eventId, block);
            }
            return block;
        }

        #endregion // Registration

        #region Operations

        /// <summary>
        /// Dispatches the event to its handlers.
        /// </summary>
        public void Dispatch(StringHash32 eventId, TArg argument = default) {
            CastableEvent<TArg> block;
            if (m_Handlers.TryGetValue(eventId, out block)) {
                block.Invoke(argument);
            }
        }

        /// <summary>
        /// Dispatches the event to its handlers the next time FlushQueue() is called.
        /// </summary>
        public void Queue(StringHash32 eventId, TArg argument = default) {
            m_QueuedEvents.PushBack(new QueuedEvent(eventId, argument));
        }

        /// <summary>
        /// Dispatches all events queued up with Queue.
        /// </summary>
        public void FlushQueue() {
            QueuedEvent evt;
            while (m_QueuedEvents.TryPopFront(out evt)) {
                Dispatch(evt.Id, evt.Argument);
            }
        }

        #endregion // Operations
    }
}