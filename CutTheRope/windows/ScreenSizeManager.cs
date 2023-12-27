using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CutTheRope.iframework.core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CutTheRope.windows
{
    internal class ScreenSizeManager
    {
        public const int MIN_WINDOW_WIDTH = 800;

        private bool _isFullScreen;

        private Microsoft.Xna.Framework.Rectangle _windowRect;

        private Microsoft.Xna.Framework.Rectangle _fullScreenRect;

        private int _gameWidth;

        private int _gameHeight;

        private double _gameAspectRatio;

        private Microsoft.Xna.Framework.Rectangle _scaledViewRect;

        private bool _skipChanges;

        private bool _fullScreenCropWidth = true;

        public static int MAX_WINDOW_WIDTH
        {
            get
            {
                if (Global.GraphicsDeviceManager.GraphicsProfile == GraphicsProfile.HiDef)
                {
                    return 4096;
                }
                return 2048;
            }
        }

        public int WindowWidth
        {
            get
            {
                return _windowRect.Width;
            }
        }

        public int WindowHeight
        {
            get
            {
                return _windowRect.Height;
            }
        }

        public int ScreenWidth
        {
            get
            {
                return _fullScreenRect.Width;
            }
        }

        public int ScreenHeight
        {
            get
            {
                return _fullScreenRect.Height;
            }
        }

        public bool IsFullScreen
        {
            get
            {
                return _isFullScreen;
            }
        }

        public Microsoft.Xna.Framework.Rectangle CurrentSize
        {
            get
            {
                if (IsFullScreen)
                {
                    return _fullScreenRect;
                }
                return _windowRect;
            }
        }

        public int GameWidth
        {
            get
            {
                return _gameWidth;
            }
        }

        public int GameHeight
        {
            get
            {
                return _gameHeight;
            }
        }

        public Microsoft.Xna.Framework.Rectangle ScaledViewRect
        {
            get
            {
                return _scaledViewRect;
            }
        }

        public bool SkipSizeChanges
        {
            get
            {
                return _skipChanges;
            }
        }

        public bool FullScreenCropWidth
        {
            set
            {
                if (_fullScreenCropWidth != value)
                {
                    _fullScreenCropWidth = value;
                    UpdateScaledView();
                }
            }
        }

        public double WidthAspectRatio
        {
            get
            {
                return (double)_scaledViewRect.Width / (double)_gameWidth;
            }
        }

        [DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        private void RefreshDesktop()
        {
            SHChangeNotify(134217728, 4096, IntPtr.Zero, IntPtr.Zero);
        }

        public void SetWindowMinimumSize(Form form)
        {
            form.MinimumSize = new Size(800, ScaledGameHeight(800));
        }

        public int TransformWindowToViewX(int x)
        {
            return x - _scaledViewRect.X;
        }

        public int TransformWindowToViewY(int y)
        {
            return y - _scaledViewRect.Y;
        }

        public float TransformViewToGameX(float x)
        {
            return x * (float)_gameWidth / (float)_scaledViewRect.Width;
        }

        public float TransformViewToGameY(float y)
        {
            return y * (float)_gameHeight / (float)_scaledViewRect.Height;
        }

        public ScreenSizeManager(int gameWidth, int gameHeight)
        {
            _gameWidth = gameWidth;
            _gameHeight = gameHeight;
            _gameAspectRatio = (double)gameHeight / (double)gameWidth;
        }

        public void Init(DisplayMode displayMode, int windowWidth, bool isFullScreen)
        {
            FullScreenRectChanged(displayMode);
            int num = ((windowWidth > 0) ? windowWidth : (displayMode.Width - 100));
            if (num < 800)
            {
                num = 800;
            }
            if (num > MAX_WINDOW_WIDTH)
            {
                num = MAX_WINDOW_WIDTH;
            }
            if (num > displayMode.Width)
            {
                num = displayMode.Width;
            }
            WindowRectChanged(new Microsoft.Xna.Framework.Rectangle(0, 0, num, ScaledGameHeight(num)));
            if (isFullScreen)
            {
                ToggleFullScreen();
            }
            else
            {
                ApplyWindowSize(WindowWidth);
            }
        }

        public int ScaledGameWidth(int scaledHeight)
        {
            return (int)((double)scaledHeight / _gameAspectRatio + 0.5);
        }

        public int ScaledGameHeight(int scaledWidth)
        {
            return (int)((double)scaledWidth * _gameAspectRatio + 0.5);
        }

        private void UpdateScaledView()
        {
            if (_skipChanges)
            {
                return;
            }
            if (_isFullScreen)
            {
                if (_fullScreenRect.Width >= _fullScreenRect.Height)
                {
                    int num = (_fullScreenCropWidth ? _fullScreenRect.Height : ScaledGameHeight(_fullScreenRect.Width));
                    int num2 = (_fullScreenCropWidth ? ScaledGameWidth(num) : _fullScreenRect.Width);
                    _scaledViewRect = new Microsoft.Xna.Framework.Rectangle((_fullScreenRect.Width - num2) / 2, (_fullScreenRect.Height - num) / 2, num2, num);
                }
                else
                {
                    int num3 = (_fullScreenCropWidth ? ((int)((float)_fullScreenRect.Width / 5f * 4f)) : ScaledGameHeight(_fullScreenRect.Width));
                    int num4 = (_fullScreenCropWidth ? ScaledGameWidth(num3) : _fullScreenRect.Width);
                    _scaledViewRect = new Microsoft.Xna.Framework.Rectangle((_fullScreenRect.Width - num4) / 2, (_fullScreenRect.Height - num3) / 2, num4, num3);
                }
            }
            else
            {
                _scaledViewRect = _windowRect;
            }
        }

        public void ApplyWindowSize(int width)
        {
            GraphicsDeviceManager graphicsDeviceManager = Global.GraphicsDeviceManager;
            graphicsDeviceManager.PreferredBackBufferWidth = width;
            graphicsDeviceManager.PreferredBackBufferHeight = ScaledGameHeight(width);
            graphicsDeviceManager.ApplyChanges();
            WindowRectChanged(new Microsoft.Xna.Framework.Rectangle(0, 0, graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight));
        }

        public void ToggleFullScreen()
        {
            _skipChanges = true;
            GraphicsDeviceManager graphicsDeviceManager = Global.GraphicsDeviceManager;
            bool fullScreenCropWidth = _fullScreenCropWidth;
            FullScreenCropWidth = true;
            int preferredBackBufferWidth = (IsFullScreen ? _windowRect.Width : _fullScreenRect.Width);
            int preferredBackBufferHeight = (IsFullScreen ? _windowRect.Height : _fullScreenRect.Height);
            graphicsDeviceManager.PreferredBackBufferWidth = preferredBackBufferWidth;
            graphicsDeviceManager.PreferredBackBufferHeight = preferredBackBufferHeight;
            graphicsDeviceManager.IsFullScreen = !graphicsDeviceManager.IsFullScreen;
            graphicsDeviceManager.ApplyChanges();
            _skipChanges = false;
            EnableFullScreen(!IsFullScreen);
            Save();
            iframework.core.Application.sharedCanvas().reshape();
            iframework.core.Application.sharedRootController().fullscreenToggled(IsFullScreen);
            FullScreenCropWidth = fullScreenCropWidth;
            if (!IsFullScreen)
            {
                RefreshDesktop();
            }
        }

        public void FixWindowSize(Microsoft.Xna.Framework.Rectangle newWindowRect)
        {
            if (_skipChanges)
            {
                return;
            }
            GraphicsDeviceManager graphicsDeviceManager = Global.GraphicsDeviceManager;
            FullScreenRectChanged(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode);
            if (!IsFullScreen)
            {
                try
                {
                    int num = graphicsDeviceManager.PreferredBackBufferWidth;
                    if (newWindowRect.Width != WindowWidth)
                    {
                        num = newWindowRect.Width;
                    }
                    else if (newWindowRect.Height != WindowHeight)
                    {
                        num = ScaledGameWidth(newWindowRect.Height);
                    }
                    if (num < 800 || ScaledGameHeight(num) < ScaledGameHeight(800))
                    {
                        num = 800;
                    }
                    if (num > MAX_WINDOW_WIDTH)
                    {
                        num = MAX_WINDOW_WIDTH;
                    }
                    if (num > ScreenWidth)
                    {
                        num = ScreenWidth;
                    }
                    ApplyWindowSize(num);
                }
                catch (Exception)
                {
                }
            }
            Save();
            iframework.core.Application.sharedCanvas().reshape();
        }

        public void ApplyViewportToDevice()
        {
            Microsoft.Xna.Framework.Rectangle bounds = ((!_isFullScreen) ? Microsoft.Xna.Framework.Rectangle.Intersect(_scaledViewRect, _windowRect) : Microsoft.Xna.Framework.Rectangle.Intersect(_scaledViewRect, _fullScreenRect));
            try
            {
                Global.GraphicsDevice.Viewport = new Viewport(bounds);
            }
            catch (Exception)
            {
            }
        }

        public void Save()
        {
            Preferences._setIntforKey(_windowRect.Width, "PREFS_WINDOW_WIDTH", false);
            Preferences._setIntforKey(_windowRect.Height, "PREFS_WINDOW_HEIGHT", false);
            Preferences._setBooleanforKey(_isFullScreen, "PREFS_WINDOW_FULLSCREEN", true);
        }

        private void WindowRectChanged(Microsoft.Xna.Framework.Rectangle newWindowRect)
        {
            if (!_skipChanges)
            {
                _windowRect = newWindowRect;
                _windowRect.X = 0;
                _windowRect.Y = 0;
                UpdateScaledView();
            }
        }

        private void FullScreenRectChanged(DisplayMode d)
        {
            FullScreenRectChanged(new Microsoft.Xna.Framework.Rectangle(0, 0, d.Width, d.Height));
        }

        private void FullScreenRectChanged(Microsoft.Xna.Framework.Rectangle r)
        {
            if (!_skipChanges)
            {
                _fullScreenRect = r;
                UpdateScaledView();
            }
        }

        private void EnableFullScreen(bool bFull)
        {
            if (!_skipChanges)
            {
                _isFullScreen = bFull;
                UpdateScaledView();
            }
        }
    }
}
