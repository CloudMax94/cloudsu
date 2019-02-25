// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Scoring;
using osu.Game.Screens.Multi.Ranking.Types;
using osu.Game.Screens.Ranking;
using osu.Game.Screens.Ranking.Types;

namespace osu.Game.Screens.Multi.Ranking
{
    public class MatchResults : Results
    {
        public MatchResults(ScoreInfo score)
            : base(score)
        {
        }

        protected override IEnumerable<IResultPageInfo> CreateResultPages() => new IResultPageInfo[]
        {
            new ScoreOverviewPageInfo(Score, Beatmap.Value),
            new LocalLeaderboardPageInfo(Score, Beatmap.Value),
            new RoomLeaderboardPageInfo(Score, Beatmap.Value),
        };
    }
}
