using LinqToDB;
using LinqToDB.Data;
using FitsArchiveLib.Attributes;
using LinqToDB.Mapping;

namespace FitsArchiveLib.Database
{
    [Table(Name = "FitsHeaderIndexed")]
    public class FitsHeaderIndexedRow
    {        
        [PrimaryKey, Identity, Column(Name = "id")]
        public long Id { get; set; }

        [Column(Name = "fits_id")]
        public long FitsId { get; set; }

        [Column(Name = "AUTHOR"), SingleValueFitsField]
        public string Author { get; set; }

        [Column(Name = "BITPIX"), SingleValueFitsField]
        public int? Bitpix { get; set; }

        [Column(Name = "NAXIS1"), SingleValueFitsField]
        public int? Naxis1 { get; set; }

        [Column(Name = "NAXIS2"), SingleValueFitsField]
        public int? Naxis2 { get; set; }

        [Column(Name = "COMMENT"), MultiValueFitsField]
        public string Comment { get; set; }
        
        [Column(Name = "INSTRUME"), SingleValueFitsField]
        public string Instrument { get; set; }

        [Column(Name = "TELESCOP"), SingleValueFitsField]
        public string Telescope { get; set; }

        [Column(Name = "OBSERVER"), SingleValueFitsField]
        public string Observer { get; set; }

        [Column(Name = "EXPTIME"), SingleValueFitsField]
        public double? ExpTime { get; set; }

        [Column(Name = "EXPOSURE"), SingleValueFitsField]
        public double? Exposure { get; set; }

        [Column(Name = "GAIN"), SingleValueFitsField]
        public double? Gain { get; set; }

        [Column(Name = "CCD-TEMP"), SingleValueFitsField]
        public double? CcdTemp { get; set; }

        [Column(Name = "XBINNING"), SingleValueFitsField]
        public int? Xbinning { get; set; }

        [Column(Name = "YBINNING"), SingleValueFitsField]
        public int? Ybinning { get; set; }

        [Column(Name = "FRAME"), SingleValueFitsField]
        public string Frame { get; set; }

        [Column(Name = "FILTER"), SingleValueFitsField]
        public string Filter { get; set; }

        [Column(Name = "FOCALLEN"), SingleValueFitsField]
        public double? FocalLength { get; set; }

        [Column(Name = "OBJECT"), SingleValueFitsField]
        public string Object { get; set; }

        [Column(Name = "OBJCTRA"), SingleValueFitsField]
        public string ObjectRa { get; set; }

        [Column(Name = "OBJCTDEC"), SingleValueFitsField]
        public string ObjectDec { get; set; }

        [Column(Name = "RA"), SingleValueFitsField]
        public double? Ra { get; set; }

        [Column(Name = "DEC"), SingleValueFitsField]
        public double? Dec { get; set; }

        [Column(Name = "RA"), SingleValueFitsField]
        public double? RaObj { get; set; }

        [Column(Name = "DEC"), SingleValueFitsField]
        public double? DecObj { get; set; }

        [Column(Name = "SITELAT"), SingleValueFitsField]
        public double? SiteLat { get; set; }

        [Column(Name = "SITELONG"), SingleValueFitsField]
        public double? SiteLong { get; set; }

        [Column(Name = "LATITUDE"), SingleValueFitsField]
        public double? Latitude { get; set; }

        [Column(Name = "EQUINOX"), SingleValueFitsField]
        public double? Equinox { get; set; }

        [Column(Name = "APERTURE"), SingleValueFitsField]
        public double? Aperture { get; set; }

        [Column(Name = "TIME-OBS"), SingleValueFitsField]
        public string TimeObs { get; set; }

        [Column(Name = "TIME-END"), SingleValueFitsField]
        public string TimeEnd { get; set; }

        [Column(Name = "DATE-OBS"), SingleValueFitsField]
        public string DateObs { get; set; }

        [Column(Name = "DATE-END"), SingleValueFitsField]
        public string DateEnd { get; set; }

        [Column(Name = "AIRMASS"), SingleValueFitsField]
        public double? Airmass { get; set; }

        [Column(Name = "PROGRAM"), SingleValueFitsField]
        public string Program { get; set; }

        [Column(Name = "SWCREATE"), SingleValueFitsField]
        public string SwCreate { get; set; }

        [Column(Name = "OBJNAME"), SingleValueFitsField]
        public string ObjName { get; set; }

    }
}
