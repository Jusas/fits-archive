using LinqToDB;
using LinqToDB.Data;
using FitsArchiveLib.Attributes;
using LinqToDB.Mapping;

namespace FitsArchiveLib.Database
{
    [Table(Name = "FitsHeaderIndexed")]
    internal class FitsHeaderIndexedRow
    {        
        [PrimaryKey, Identity, Column(Name = "id")]
        public long Id { get; set; }

        [Column(Name = "fits_id")]
        public long FitsId { get; set; }

        [Column(Name = "AUTHOR"), FitsField(Name = "AUTHOR"), Nullable]
        public string Author { get; set; }

        [Column(Name = "BITPIX"), FitsField(Name = "BITPIX", Numeric = true), Nullable]
        public int? Bitpix { get; set; }

        [Column(Name = "NAXIS1"), FitsField(Name = "NAXIS1", Numeric = true), Nullable]
        public int? Naxis1 { get; set; }

        [Column(Name = "NAXIS2"), FitsField(Name = "NAXIS2", Numeric = true), Nullable]
        public int? Naxis2 { get; set; }

        [Column(Name = "COMMENT"), FitsField(Name = "COMMENT", MultiValue = true), Nullable]
        public string Comment { get; set; }
        
        [Column(Name = "INSTRUME"), FitsField(Name = "INSTRUME"), Nullable]
        public string Instrument { get; set; }

        [Column(Name = "TELESCOP"), FitsField(Name = "TELESCOP"), Nullable]
        public string Telescope { get; set; }

        [Column(Name = "OBSERVER"), FitsField(Name = "OBSERVER"), Nullable]
        public string Observer { get; set; }

        [Column(Name = "EXPTIME"), FitsField(Name = "EXPTIME", VarianceValue = true), Nullable]
        public double? ExpTime { get; set; }

        [Column(Name = "EXPOSURE"), FitsField(Name = "EXPOSURE", VarianceValue = true), Nullable]
        public double? Exposure { get; set; }

        [Column(Name = "GAIN"), FitsField(Name = "GAIN", Numeric = true), Nullable]
        public double? Gain { get; set; }

        [Column(Name = "CCD-TEMP"), FitsField(Name = "CCD-TEMP", VarianceValue = true), Nullable]
        public double? CcdTemp { get; set; }

        [Column(Name = "XBINNING"), FitsField(Name = "XBINNING", Numeric = true), Nullable]
        public int? Xbinning { get; set; }

        [Column(Name = "YBINNING"), FitsField(Name = "YBINNING", Numeric = true), Nullable]
        public int? Ybinning { get; set; }

        [Column(Name = "FRAME"), FitsField(Name = "FRAME"), Nullable]
        public string Frame { get; set; }

        [Column(Name = "FILTER"), FitsField(Name = "FILTER"), Nullable]
        public string Filter { get; set; }

        [Column(Name = "FOCALLEN"), FitsField(Name = "FOCALLEN", Numeric = true), Nullable]
        public double? FocalLength { get; set; }

        [Column(Name = "OBJECT"), FitsField(Name = "OBJECT"), Nullable]
        public string Object { get; set; }

        [Column(Name = "OBJCTRA"), FitsField(Name = "OBJCTRA", VarianceValue = true), Nullable]
        public string ObjectRa { get; set; }

        [Column(Name = "OBJCTDEC"), FitsField(Name = "OBJCTDEC", VarianceValue = true), Nullable]
        public string ObjectDec { get; set; }

        [Column(Name = "RA"), FitsField(Name = "RA", VarianceValue = true), Nullable]
        public double? Ra { get; set; }

        [Column(Name = "DEC"), FitsField(Name = "DEC", VarianceValue = true), Nullable]
        public double? Dec { get; set; }

        [Column(Name = "RA_OBJ"), FitsField(Name = "RA_OBJ", VarianceValue = true), Nullable]
        public double? RaObj { get; set; }

        [Column(Name = "DEC_OBJ"), FitsField(Name = "DEC_OBJ", VarianceValue = true), Nullable]
        public double? DecObj { get; set; }

        [Column(Name = "SITELAT"), FitsField(Name = "SITELAT", VarianceValue = true), Nullable]
        public double? SiteLat { get; set; }

        [Column(Name = "SITELONG"), FitsField(Name = "SITELONG", VarianceValue = true), Nullable]
        public double? SiteLong { get; set; }

        [Column(Name = "LATITUDE"), FitsField(Name = "LATITUDE", VarianceValue = true), Nullable]
        public double? Latitude { get; set; }

        [Column(Name = "EQUINOX"), FitsField(Name = "EQUINOX", VarianceValue = true), Nullable]
        public double? Equinox { get; set; }

        [Column(Name = "APERTURE"), FitsField(Name = "APERTURE", Numeric = true), Nullable]
        public double? Aperture { get; set; }

        [Column(Name = "TIME-OBS"), FitsField(Name = "TIME-OBS", DateLike = true), Nullable]
        public string TimeObs { get; set; }

        [Column(Name = "TIME-END"), FitsField(Name = "TIME-END", DateLike = true), Nullable]
        public string TimeEnd { get; set; }

        [Column(Name = "DATE-OBS"), FitsField(Name = "DATE-OBS", DateLike = true), Nullable]
        public string DateObs { get; set; }

        [Column(Name = "DATE-END"), FitsField(Name = "DATE-END", DateLike = true), Nullable]
        public string DateEnd { get; set; }

        [Column(Name = "AIRMASS"), FitsField(Name = "AIRMASS", VarianceValue = true), Nullable]
        public double? Airmass { get; set; }

        [Column(Name = "PROGRAM"), FitsField(Name = "PROGRAM"), Nullable]
        public string Program { get; set; }

        [Column(Name = "SWCREATE"), FitsField(Name = "SWCREATE"), Nullable]
        public string SwCreate { get; set; }

        [Column(Name = "OBJNAME"), FitsField(Name = "OBJNAME"), Nullable]
        public string ObjName { get; set; }

        [Column(Name = "parsed_ra"), Nullable]
        public double? ParsedRa { get; set; }

        [Column(Name = "parsed_dec"), Nullable]
        public double? ParsedDec { get; set; }

    }
}
