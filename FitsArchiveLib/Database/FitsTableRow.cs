using LinqToDB.Mapping;

namespace FitsArchiveLib.Database
{
    [Table(Name = "Fits")]
    internal class FitsTableRow
    {
        [Column(Name = "id"), PrimaryKey, Identity]
        public long? Id { get; set; }

        [Column(Name = "filename")]
        public string Filename { get; set; }

        [Column(Name = "checksum")]
        public string Checksum { get; set; }

        [Column(Name = "size"), Nullable]
        public long? Size { get; set; }

        [Column(Name = "date_indexed"), Nullable]
        public long? DateIndexed { get; set; }
    }
}
