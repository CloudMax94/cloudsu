// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Overlays.Profile.Header;
using osu.Game.Rulesets;
using System;
using osuTK;

using osu.Game.Graphics.UserInterface;
using osu.Game.Users;

namespace osu.Game.Overlays.Cloudsu
{
    public class CloudsuHeader : TabControlOverlayHeader<string> // OverlayHeader
    {
        public Action ShowBestPerformancePage;

        public Action ShowOtherPage;

        public readonly Bindable<RulesetInfo> ruleset = new Bindable<RulesetInfo>();

        public CloudsuHeader()
        {
            TabControl.AddItem("Best Performance");
            // TabControl.AddItem("Other");

            Current.ValueChanged += e =>
            {
                if (e.NewValue == "Other")
                {
                    ShowOtherPage.Invoke();
                }
                else
                {
                    ShowBestPerformancePage.Invoke();
                }
                title.Tab = e.NewValue;
            };

            ruleset.ValueChanged += e => title.Ruleset = e.NewValue.Name;
        }

        private CloudsuHeaderTitle title;

        protected override Drawable CreateContent() => new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical
        };

        protected override ScreenTitle CreateTitle() => title = new CloudsuHeaderTitle();

        private class CloudsuHeaderTitle : ScreenTitle
        {
            private string tab;
            public string Tab
            {
                set {
                    tab = value;
                    Section = Ruleset + " " + (value ?? "Best Performance");
                }
                get {
                    return tab;
                }
            }

            private string ruleset;
            public string Ruleset
            {
                set {
                    ruleset = value;
                    Section = (value ?? "osu!") + " " + Tab;
                }
                get {
                    return ruleset;
                }
            }

            public CloudsuHeaderTitle()
            {
                Title = "cloudsu!";
                Tab = null;
                Ruleset = null;
            }

            protected override Drawable CreateIcon() => new SpriteIcon
            {
                Size = new Vector2(24),
                Icon = FontAwesome.Solid.Trophy,
                Shadow = true,
            };
        }
    }
}
