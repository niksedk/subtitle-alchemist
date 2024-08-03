using SharpHook;

namespace SubtitleAlchemist.Logic
{
    public static class SharpHookHandler
    {
        public static readonly TaskPoolGlobalHook Hook = new();
        private static bool _loaded;

        private static readonly Stack<List<EventHandler<KeyboardHookEventArgs>?>> _stack = new();

        public static EventHandler<KeyboardHookEventArgs>? KeyPressed { get; set; }
        public static EventHandler<MouseHookEventArgs>? MouseClicked { get; set; }

        public static async Task RunAsync()
        {
          //  if (_loaded)
            {
                return;
            }

            _loaded = true;

            Hook.KeyPressed += (s, e) =>
            {
                KeyPressed?.Invoke(s, e);
            };

            Hook.MouseClicked += (s, e) =>
            {
                MouseClicked?.Invoke(s, e);
            };

            await Hook.RunAsync();
        }

        public static void Clear()
        {
            KeyPressed = null;
        }

        public static void Push()
        {
            _stack.Push(new List<EventHandler<KeyboardHookEventArgs>?>
            {
                KeyPressed
            });
        }

        public static void Pop()
        {
            if (_stack.Count == 0)
            {
                return;
            }

            var list = _stack.Pop();
            KeyPressed = list[0];
        }

        public static void Dispose()
        {
            Hook.Dispose();
        }
    }
}