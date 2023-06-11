using CsvHelper.Configuration.Attributes;

namespace CountryGwpApi.CountryGwp.Data.Csv;

public record GwpByCountryDataModel
{
    [Index(0)]
    public string Country { get; set; }
    [Index(1)]
    public string VariableId { get; set; }
    [Index(2)]
    public string VariableName { get; set; }
    [Index(3)]
    public string LineOfBusiness { get; set; }

    #region Years
    public decimal? Y2000 { get; set; }
    public decimal? Y2001 { get; set; }
    public decimal? Y2002 { get; set; }
    public decimal? Y2003 { get; set; }
    public decimal? Y2004 { get; set; }
    public decimal? Y2005 { get; set; }
    public decimal? Y2006 { get; set; }
    public decimal? Y2007 { get; set; }
    public decimal? Y2008 { get; set; }
    public decimal? Y2009 { get; set; }
    public decimal? Y2010 { get; set; }
    public decimal? Y2011 { get; set; }
    public decimal? Y2012 { get; set; }
    public decimal? Y2013 { get; set; }
    public decimal? Y2014 { get; set; }
    public decimal? Y2015 { get; set; }
    #endregion
}