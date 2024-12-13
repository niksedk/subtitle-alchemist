using SharpHook;

namespace SubtitleAlchemist.Logic
{
    public static class SharpHookHandler
    {
        public static TaskPoolGlobalHook? Hook;
        private static bool _loaded;

        private static readonly Stack<List<EventHandler<KeyboardHookEventArgs>>> StackKeyPressed = new();
        private static readonly List<EventHandler<KeyboardHookEventArgs>> CurrentKeyPressedEventHandlers = new();
        public static void AddKeyPressed(EventHandler<KeyboardHookEventArgs> keyboardHookEventArgs)
        {
            CurrentKeyPressedEventHandlers.Add(keyboardHookEventArgs);
        }

        private static readonly Stack<List<EventHandler<KeyboardHookEventArgs>>> StackKeyReleased = new();
        private static readonly List<EventHandler<KeyboardHookEventArgs>> CurrentKeyReleasedEventHandlers = new();
        public static void AddKeyReleased(EventHandler<KeyboardHookEventArgs> keyboardHookEventArgs)
        {
            CurrentKeyReleasedEventHandlers.Add(keyboardHookEventArgs);
        }

        private static readonly Stack<List<EventHandler<MouseHookEventArgs>>> StackMouseClicked = new();
        private static readonly List<EventHandler<MouseHookEventArgs>> CurrentMouseClickedEventHandlers = new();
        public static void AddMouseClicked(EventHandler<MouseHookEventArgs> mouseHookEventArgs)
        {
            CurrentMouseClickedEventHandlers.Add(mouseHookEventArgs);
        }

        private static readonly Stack<List<EventHandler<MouseHookEventArgs>>> StackMousePressed = new();
        private static readonly List<EventHandler<MouseHookEventArgs>> CurrentMousePressedEventHandlers = new();
        public static void AddMousePressed(EventHandler<MouseHookEventArgs> mouseHookEventArgs)
        {
            CurrentMousePressedEventHandlers.Add(mouseHookEventArgs);
        }

        private static readonly Stack<List<EventHandler<MouseHookEventArgs>>> StackMouseReleased = new();
        private static readonly List<EventHandler<MouseHookEventArgs>> CurrentMouseReleasedEventHandlers = new();
        public static void AddMouseReleased(EventHandler<MouseHookEventArgs> mouseHookEventArgs)
        {
            CurrentMouseReleasedEventHandlers.Add(mouseHookEventArgs);
        }

        public static async Task RunAsync()
        {
            return; //TODO: Remove

            if (_loaded)
            {
                return;
            }

            if (Hook == null || Hook.IsDisposed)
            {
                Hook = new TaskPoolGlobalHook();
            }

            _loaded = true;

            Hook.KeyPressed += (s, e) =>
            {
                foreach (var handler in CurrentKeyPressedEventHandlers)
                {
                    handler?.Invoke(s, e);
                }
            };

            Hook.KeyReleased += (s, e) =>
            {
                foreach (var handler in CurrentKeyReleasedEventHandlers)
                {
                    handler?.Invoke(s, e);
                }
            };

            Hook.MouseClicked += (s, e) =>
            {
                foreach (var handler in CurrentMouseClickedEventHandlers)
                {
                    handler?.Invoke(s, e);
                }
            };

            Hook.MousePressed += (s, e) =>
            {
                foreach (var handler in CurrentMousePressedEventHandlers)
                {
                    handler?.Invoke(s, e);
                }
            };

            Hook.MouseReleased += (s, e) =>
            {
                foreach (var handler in CurrentMouseReleasedEventHandlers)
                {
                    handler?.Invoke(s, e);
                }
            };

            await Hook.RunAsync();
        }

        public static void Clear()
        {
            CurrentKeyPressedEventHandlers.Clear();
            CurrentKeyReleasedEventHandlers.Clear();
            CurrentMouseClickedEventHandlers.Clear();
            CurrentMousePressedEventHandlers.Clear();
            CurrentMouseReleasedEventHandlers.Clear();
        }

        public static void Push()
        {
            StackKeyPressed.Push(CurrentKeyPressedEventHandlers);
            StackKeyReleased.Push(CurrentKeyReleasedEventHandlers);
            StackMouseClicked.Push(CurrentMouseClickedEventHandlers);
            StackMousePressed.Push(CurrentMousePressedEventHandlers);
            StackMouseReleased.Push(CurrentMouseReleasedEventHandlers);
            Clear();
        }

        public static void Pop()
        {
            Clear();

            if (StackKeyPressed.Count > 0)
            {
                CurrentKeyPressedEventHandlers.AddRange(StackKeyPressed.Pop());
            }

            if (StackKeyReleased.Count > 0)
            {
                CurrentKeyReleasedEventHandlers.AddRange(StackKeyReleased.Pop());
            }

            if (StackMouseClicked.Count > 0)
            {
                CurrentMouseClickedEventHandlers.AddRange(StackMouseClicked.Pop());
            }

            if (StackMousePressed.Count > 0)
            {
                CurrentMousePressedEventHandlers.AddRange(StackMousePressed.Pop());
            }

            if (StackMouseReleased.Count > 0)
            {
                CurrentMouseReleasedEventHandlers.AddRange(StackMouseReleased.Pop());
            }
        }

        public static void Dispose()
        {
            Hook?.Dispose();
            _loaded = false;
        }
    }
}