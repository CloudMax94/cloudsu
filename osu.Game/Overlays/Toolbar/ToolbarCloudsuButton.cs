// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;


namespace osu.Game.Overlays.Toolbar
{
    public class ToolbarCloudsuButton : ToolbarOverlayToggleButton
    {
        public ToolbarCloudsuButton()
        {
            SetIcon(FontAwesome.Solid.Trophy);
        }

        [BackgroundDependencyLoader(true)]
        private void load(CloudsuOverlay cloudsu)
        {
            StateContainer = cloudsu;
        }
    }
}
