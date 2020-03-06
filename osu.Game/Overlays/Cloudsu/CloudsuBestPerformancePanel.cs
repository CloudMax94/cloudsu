// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osu.Game.Scoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace osu.Game.Overlays.Cloudsu
{
    public class CloudsuBestPerformancePanel : FillFlowContainer
    {
        private readonly OsuSpriteText missingText;
        private CancellationTokenSource loadCancellation;

        [Resolved]
        private ScoreManager scoreManager { get; set; }

        [Resolved]
        private BeatmapManager beatmapManager { get; set; }

        [Resolved]
        private Bindable<WorkingBeatmap> beatmap { get; set; }

        public readonly Bindable<RulesetInfo> ruleset = new Bindable<RulesetInfo>();

        private List<ScoreInfo> Scores = new List<ScoreInfo>();

        protected readonly FillFlowContainer StatsContainer;
        protected readonly FillFlowContainer ItemsContainer;

        protected readonly OsuSpriteText PlaysText;
        protected readonly OsuSpriteText PPText;
        protected readonly OsuSpriteText PPSuffix;
        protected readonly OsuSpriteText BonusPPText;
        protected readonly OsuSpriteText BonusPPSuffix;

        public CloudsuBestPerformancePanel()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            ruleset.ValueChanged += _ => Schedule(refreshScores);

            Children = new Drawable[]
            {
                StatsContainer = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Direction = FillDirection.Vertical,
                    Alpha = 0,
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(0, 0),
                            Margin = new MarginPadding { Top = 10 },
                            Children = new Drawable[]
                            {
                                PlaysText = new OsuSpriteText
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Font = OsuFont.GetFont(size: 24, weight: FontWeight.Bold),
                                    Text = ""
                                },
                                new OsuSpriteText
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Font = OsuFont.GetFont(size: 24, weight: FontWeight.Bold),
                                    Text = " performances"
                                }
                            }
                        },
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(0, 0),
                            Margin = new MarginPadding { Top = 5 },
                            Children = new Drawable[]
                            {

                                PPText = new OsuSpriteText
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Font = OsuFont.GetFont(size: 20, weight: FontWeight.Bold),
                                    Text = ""
                                },
                                PPSuffix = new OsuSpriteText
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Font = OsuFont.GetFont(size: 16, weight: FontWeight.Bold),
                                    Text = "pp"
                                },
                                BonusPPText = new OsuSpriteText
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Font = OsuFont.GetFont(size: 15, weight: FontWeight.Bold),
                                    Text = "",
                                    Margin = new MarginPadding { Left = 5 }
                                },
                                BonusPPSuffix = new OsuSpriteText
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Font = OsuFont.GetFont(size: 11, weight: FontWeight.Bold),
                                    Text = "pp"
                                }
                            }
                        },
                    }
                },
                ItemsContainer = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Margin = new MarginPadding { Top = 10 },
                    Spacing = new Vector2(0, 2)
                },
                missingText = new OsuSpriteText
                {
                    Font = OsuFont.GetFont(size: 18),
                    Text = "",
                    Alpha = 0,
                },
            };
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            PPText.Colour = colourProvider.Highlight1;
            PPSuffix.Colour = colourProvider.Highlight1; // Light1
            BonusPPText.Colour = colourProvider.Highlight1;
            BonusPPSuffix.Colour = colourProvider.Highlight1; // Light1

            scoreManager.ItemAdded += scoreWasAdded;
            scoreManager.ItemRemoved += scoreWasRemoved;
        }

        protected void scoreWasAdded(ScoreInfo score)
        {
            // Only refresh if the score is of the correct ruleset
            if (score.RulesetID == ruleset.Value.ID)
                Schedule(refreshScores);
        }

        protected void scoreWasRemoved(ScoreInfo score)
        {
            // Only refresh if the score was in the list
            if (Scores.FindIndex(s => s.ID == score.ID) >= 0)
                Schedule(refreshScores);
        }

        private void refreshScores()
        {
            // Get all scores for osu!standad with a PP value.
            // Group them by beatmap ID and then grab the highest PP of each group.
            // Sort the scores by PP.
            // Turn the IOrderedEnumerable into a List.
            Scores = scoreManager.QueryScores(s =>
                !s.DeletePending && s.Ruleset.ID == ruleset.Value.ID && s.Beatmap != null && s.PP.HasValue
            ).GroupBy(s => s.BeatmapInfoID).Select(grp =>
                grp.OrderByDescending(s => s.PP).FirstOrDefault()
            ).OrderByDescending(s => s.PP).ToList();

            updateStats();
            recreateList();
        }

        private void updateStats()
        {
            if (!Scores.Any())
            {
                StatsContainer.Hide();
                return;
            }
            else
            {
                StatsContainer.Show();
            }

            var i = 0;
            var pp = (float) Scores.Sum(delegate (ScoreInfo s) { return Math.Pow(0.95, i++) * s.PP; });

            PlaysText.Text = Scores.Count.ToString();
            PPText.Text = pp.ToString("N2");
            BonusPPText.Text = (pp + 416.66667).ToString("N2");
        }

        private void recreateList()
        {
            loadCancellation?.Cancel();
            loadCancellation = new CancellationTokenSource();

            ItemsContainer.Clear();

            if (!Scores.Any())
            {
                missingText.Show();
                missingText.Text = "You don't have any performances for the " + ruleset.Value.Name + " ruleset.";
                return;
            }
            else
            {
                missingText.Hide();
            }

            LoadComponentsAsync(Scores.Select(CreateDrawableItem).Where(d => d != null), drawables =>
            {
                ItemsContainer.AddRange(drawables);
            }, loadCancellation.Token);
        }

        protected Drawable CreateDrawableItem(ScoreInfo model)
        {
            // Why do we have to query the beatmaps and grab their BeatmapSet metadata?
            BeatmapInfo beatmap = beatmapManager.QueryBeatmap(b => b.ID == model.BeatmapInfoID);
            model.Beatmap.Metadata = beatmap.BeatmapSet.Metadata;
            return new DrawableBestPerformanceScore(model, Math.Pow(0.95, ItemsContainer.Count))
            {
                SelectBeatmap = SelectBeatmap
            };
        }

        public void SelectBeatmap(int beatmapInfoID)
        {
            if (beatmap.Disabled) {
                return;
            }

            var bm = beatmapManager.GetWorkingBeatmap(beatmapManager.QueryBeatmap(b => b.ID == beatmapInfoID));

            if (beatmap is Bindable<WorkingBeatmap> working) {
                working.Value = bm;
            }
            beatmap.Value.Track.Restart();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (scoreManager != null)
            {
                scoreManager.ItemAdded -= scoreWasAdded;
                scoreManager.ItemRemoved -= scoreWasRemoved;
            }
        }
    }
}
