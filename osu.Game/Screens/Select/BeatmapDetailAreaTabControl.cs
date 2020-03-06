// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osuTK;
using osuTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Framework.Graphics.Shapes;

namespace osu.Game.Screens.Select
{
    public class BeatmapDetailAreaTabControl : Container
    {
        public const float HEIGHT = 24;

        public Bindable<BeatmapDetailAreaTabItem> Current
        {
            get => tabs.Current;
            set => tabs.Current = value;
        }

        public Action<BeatmapDetailAreaTabItem, bool, bool> OnFilter; //passed the selected tab and if pp sort or mods is checked

        public IReadOnlyList<BeatmapDetailAreaTabItem> TabItems
        {
            get => tabs.Items;
            set => tabs.Items = value;
        }

        private readonly OsuTabControlCheckbox ppSortCheckbox;
        private readonly OsuTabControlCheckbox modsCheckbox;
        private readonly OsuTabControl<BeatmapDetailAreaTabItem> tabs;
        private readonly Container tabsContainer;

        public BeatmapDetailAreaTabControl()
        {
            Height = HEIGHT;

            Children = new Drawable[]
            {
                new Box
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    RelativeSizeAxes = Axes.X,
                    Height = 1,
                    Colour = Color4.White.Opacity(0.2f),
                },
                tabsContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = tabs = new OsuTabControl<BeatmapDetailAreaTabItem>
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        RelativeSizeAxes = Axes.Both,
                        IsSwitchable = true,
                    },
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(10, 0),
                    Children = new Drawable[]
                    {
                        modsCheckbox = new OsuTabControlCheckbox
                        {
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            Text = @"Selected Mods",
                            Alpha = 0,
                        },
                        ppSortCheckbox = new OsuTabControlCheckbox
                        {
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            Text = @"Sort by PP",
                            Alpha = 0,
                        },
                    }
                },
            };

            ppSortCheckbox.Current.Value = true;
            tabs.Current.ValueChanged += _ => invokeOnFilter();
            ppSortCheckbox.Current.ValueChanged += _ => invokeOnFilter();
            modsCheckbox.Current.ValueChanged += _ => invokeOnFilter();
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colour, OsuConfigManager config)
        {
            ppSortCheckbox.AccentColour = modsCheckbox.AccentColour = tabs.AccentColour = colour.YellowLight;
        }

        private void invokeOnFilter()
        {
            OnFilter?.Invoke(tabs.Current.Value, modsCheckbox.Current.Value, ppSortCheckbox.Current.Value);

            modsCheckbox.FadeTo(tabs.Current.Value.FilterableByMods ? 1: 0, 200, Easing.OutQuint);
            ppSortCheckbox.FadeTo(tabs.Current.Value.SortableByPP ? 1 : 0, 200, Easing.OutQuint);

            if (tabs.Current.Value.FilterableByMods || tabs.Current.Value.SortableByPP)
                tabsContainer.Padding = new MarginPadding { Right = 100 };
            else
                tabsContainer.Padding = new MarginPadding();
        }
    }
}
