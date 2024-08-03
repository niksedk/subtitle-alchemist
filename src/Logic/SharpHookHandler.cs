using SharpHook;

namespace SubtitleAlchemist.Logic
{
    public static class SharpHookHandler
    {
        public static readonly TaskPoolGlobalHook Hook = new();
        private static bool _loaded;

        private static readonly Stack<List<EventHandler<KeyboardHookEventArgs>>> StackKeyPressed = new();
        private static readonly List<EventHandler<KeyboardHookEventArgs>> CurrentKeyPressedEventHandlers = new();

        public static void AddKeyPressed(EventHandler<KeyboardHookEventArgs> keyboardHookEventArgs)
        {
            CurrentKeyPressedEventHandlers.Add(keyboardHookEventArgs);
        }

        private static readonly Stack<List<EventHandler<MouseHookEventArgs>>> StackMouseClicked = new();
        private static readonly List<EventHandler<MouseHookEventArgs>> CurrentMouseClickedEventHandlers = new();
        public static void AddMouseClicked(EventHandler<MouseHookEventArgs> mouseHookEventArgs)
        {
            CurrentMouseClickedEventHandlers.Add(mouseHookEventArgs);
        }

        public static async Task RunAsync()
        {
            if (_loaded)
            {
                return;
            }

            _loaded = true;

            Hook.KeyPressed += (s, e) =>
            {
                foreach (var handler in CurrentKeyPressedEventHandlers)
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

            await Hook.RunAsync();
        }

        public static void Clear()
        {
            CurrentKeyPressedEventHandlers.Clear();
            CurrentMouseClickedEventHandlers.Clear();
        }

        public static void Push()
        {
            StackKeyPressed.Push(CurrentKeyPressedEventHandlers);
            StackMouseClicked.Push(CurrentMouseClickedEventHandlers);
            Clear();
        }

        public static void Pop()
        {
            Clear();

            if (StackKeyPressed.Count > 0)
            {
                CurrentKeyPressedEventHandlers.AddRange(StackKeyPressed.Pop());
            }

            if (StackMouseClicked.Count > 0)
            {
                CurrentMouseClickedEventHandlers.AddRange(StackMouseClicked.Pop());
            }
        }

        public static void Dispose()
        {
            Clear();
            Hook.Dispose();
        }
    }
}