// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.Containers;
using osu.Game.Online.API;
using osu.Game.Overlays.Cloudsu;
using osu.Game.Rulesets;
using osuTK.Graphics;


namespace osu.Game.Overlays
{
    public class CloudsuOverlay : FullscreenOverlay
    {
        private CloudsuHeader header;

        private CloudsuBestPerformancePanel bestPerformancePanel;

        protected Color4 BackgroundColour => OsuColour.FromHex(@"52899a");
        protected Color4 TrianglesColourLight => OsuColour.FromHex(@"6aa6b9");
        protected Color4 TrianglesColourDark => OsuColour.FromHex(@"4f8394");

        public readonly Bindable<string> Current = new Bindable<string>(null);
        public readonly Bindable<RulesetInfo> ruleset = new Bindable<RulesetInfo>();

        public CloudsuOverlay()
            : base(OverlayColourScheme.Blue)
        {
        }

        [BackgroundDependencyLoader]
        private void load(IAPIProvider api, Bindable<RulesetInfo> parentRuleset, OsuColour colours)
        {
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = BackgroundColour,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Children = new[]
                    {
                        new Triangles
                        {
                            RelativeSizeAxes = Axes.Both,
                            TriangleScale = 5,
                            ColourLight = TrianglesColourLight,
                            ColourDark = TrianglesColourDark,
                        },
                    },
                },
                new OsuScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Children = new Drawable[]
                        {
                            header = new CloudsuHeader
                            {
                                ShowBestPerformancePage = ShowBestPerformancePage,
                                ShowOtherPage = ShowOtherPage
                            },
                            bestPerformancePanel = new CloudsuBestPerformancePanel
                            {
                                Padding = new MarginPadding { Left = 70, Right = 70 },
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y
                            },
                        },
                    },
                },
            };

            ruleset.BindTo(parentRuleset);
            header.ruleset.Value = ruleset.Value;
            bestPerformancePanel.ruleset.Value = ruleset.Value;
            ruleset.ValueChanged += e => header.ruleset.Value = e.NewValue;
            ruleset.ValueChanged += e => bestPerformancePanel.ruleset.Value = e.NewValue;

            Current.BindValueChanged(e =>
            {
                showContent(e.NewValue);
            });

            Current.TriggerChange();
        }

        protected override void PopIn()
        {
            Current.TriggerChange();
            base.PopIn();
        }

        public void ShowBestPerformancePage()
        {
            bestPerformancePanel.refresh();
            Current.Value = "Best Performance";
            Show();
        }

        public void ShowOtherPage()
        {
            Current.Value = "Other";
            Show();
        }

        private void showContent(string tab)
        {

            if (tab == "Other")
            {
                bestPerformancePanel.Hide();
            }
            else
            {
                bestPerformancePanel.Show();
                bestPerformancePanel.FadeTo(1f, 300, Easing.OutQuint);
            }
        }
    }
}
