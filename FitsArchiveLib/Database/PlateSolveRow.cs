using System;
using LinqToDB.Mapping;

namespace FitsArchiveLib.Database
{
    [Table(Name = "PlateSolves")]
    internal class PlateSolveRow
    {
        [Column(Name = "id"), PrimaryKey, Identity]
        public long Id { get; set; }

        [Column(Name = "fits_id")]
        public long FitsId { get; set; }

        [Column(Name = "solved_date"), Nullable]
        public long? SolvedDate { get; set; }

        [Column(Name = "orientation"), Nullable]
        public double? Orientation { get; set; }

        [Column(Name = "pixscale"), Nullable]
        public double? PixScale { get; set; }

        [Column(Name = "radius"), Nullable]
        public double? Radius { get; set; }

        [Column(Name = "center_ra"), Nullable]
        public double? CenterRa { get; set; }

        [Column(Name = "center_dec"), Nullable]
        public double? CenterDec { get; set; }

    }
}
