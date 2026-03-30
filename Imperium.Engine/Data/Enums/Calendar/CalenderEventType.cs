using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperium.Engine.Data.Enums.Calendar
{
    public enum CalendarEventType
    {
        // --- Arena & League ---
        ArenaMatch,
        LeagueSeasonStart,
        LeagueSeasonEnd,
        LeaguePlayoff,
        Championship,

        // --- Patron & Reputation ---
        PatronSummons,
        PatronFestival,
        FactionQuest,

        // --- Dynasty & Personal ---
        Birthday,
        Wedding,
        HeirBorn,
        ComingOfAge,
        MidlifeMilestone,
        RetirementDuel,

        // --- World & Political ---
        RoyalDecree,
        EcclesiasticalEdict,
        WarDeclaration,
        Festival,

        // --- Training & Progression ---
        TrainingSession,
        MentorVisit,
        SpecialExam,

        // --- Misc ---
        Custom
    }
}
