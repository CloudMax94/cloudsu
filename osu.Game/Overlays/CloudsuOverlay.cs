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
using osu.Game.Graphics.Cursor;
using osu.Game.Online.API;
using osu.Game.Overlays.Cloudsu;
using osu.Game.Rulesets;
using osuTK.Graphics;


namespace osu.Game.Overlays
{
    public class CloudsuOverlay : FullscreenOverlay
    {
        private readonly Container scrollContainer;

        private readonly CloudsuHeader Header;

        private CloudsuBestPerformancePanel BestPerformancePanel;

        protected Color4 BackgroundColour => OsuColour.FromHex(@"52899a");
        protected Color4 TrianglesColourLight => OsuColour.FromHex(@"6aa6b9");
        protected Color4 TrianglesColourDark => OsuColour.FromHex(@"4f8394");

        public readonly Bindable<string> Current = new Bindable<string>(null);
        public readonly Bindable<RulesetInfo> ruleset = new Bindable<RulesetInfo>();

        public CloudsuOverlay()
            : base(OverlayColourScheme.Blue)
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
                scrollContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new OsuContextMenuContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        Child = new OsuScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ScrollbarVisible = false,
                            Child = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Padding = new MarginPadding { Horizontal = 70, Bottom = 50 },
                                Direction = FillDirection.Vertical,
                                Children = new[]
                                {
                                    BestPerformancePanel = new CloudsuBestPerformancePanel
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y
                                    },
                                }
                            },
                        },
                    },
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        Header = new CloudsuHeader
                        {
                            ShowBestPerformancePage = ShowBestPerformancePage,
                            ShowOtherPage = ShowOtherPage
                        },
                    },
                },
            };
        }

        [BackgroundDependencyLoader]
        private void load(Bindable<RulesetInfo> parentRuleset)
        {

            ruleset.BindTo(parentRuleset);
            Header.ruleset.Value = ruleset.Value;
            BestPerformancePanel.ruleset.Value = ruleset.Value;
            ruleset.ValueChanged += e => Header.ruleset.Value = e.NewValue;
            ruleset.ValueChanged += e => BestPerformancePanel.ruleset.Value = e.NewValue;

            Current.BindValueChanged(e =>
            {
                showContent(e.NewValue);
            });

            Current.TriggerChange();
        }

        protected override void Update()
        {
            base.Update();

            scrollContainer.Padding = new MarginPadding { Top = Header.Height };
        }

        public void ShowBestPerformancePage()
        {
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
                BestPerformancePanel.Hide();
            }
            else
            {
                BestPerformancePanel.Show();
                BestPerformancePanel.FadeTo(1f, 300, Easing.OutQuint);
            }
        }
    }
}
