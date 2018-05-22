
    [System.Serializable]
    public class ScreenSafeAreaOrientationConfig
    {
        public RectOffsetFloat Relative;
        public RectOffsetInt Absolute;

        private static ScreenSafeAreaOrientationConfig s_Default = null;

        public static ScreenSafeAreaOrientationConfig Default
        {
            get
            {
                if (s_Default == null)
                {
                    s_Default = new ScreenSafeAreaOrientationConfig
                    {
                        Relative = new RectOffsetFloat { Left = 0, Right = 1, Bottom = 0, Top = 1 },
                        Absolute = default(RectOffsetInt),
                    };
                }

                return s_Default;
            }
        }
    }
