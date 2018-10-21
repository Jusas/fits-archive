using LinqToDB.Mapping;

namespace FitsArchiveLib.Database
{
    [Table(Name = "Fits")]
    public class FitsTableRow
    {
        [Column(Name = "id"), PrimaryKey, Identity]
        public long? Id { get; set; }

        [Column(Name = "filename")]
        public string Filename { get; set; }

        [Column(Name = "checksum")]
        public string Checksum { get; set; }

        [Column(Name = "size")]
        public long? Size { get; set; }

        [Column(Name = "date_indexed")]
        public long? DateIndexed { get; set; }
    }
}
