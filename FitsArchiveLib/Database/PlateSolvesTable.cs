using System;
using LinqToDB.Mapping;

namespace FitsArchiveLib.Database
{
    [Table(Name = "PlateSolves")]
    public class PlateSolvesTable
    {
        [Column(Name = "id"), PrimaryKey, Identity]
        public long Id { get; set; }

        [Column(Name = "fits_id")]
        public long FitsId { get; set; }

        [Column(Name = "solved_date")]
        public long? SolvedDate { get; set; }

        [Column(Name = "orientation")]
        public double? Orientation { get; set; }

        [Column(Name = "pixscale")]
        public double? PixScale { get; set; }

        [Column(Name = "radius")]
        public double? Radius { get; set; }

        [Column(Name = "center_ra")]
        public double? CenterRa { get; set; }

        [Column(Name = "center_dec")]
        public double CenterDec { get; set; }

    }
}
