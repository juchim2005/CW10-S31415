using System;
using System.Collections.Generic;

namespace CW10_S31415.DTOs;

public class TripGetDto
{
    public string Name { get; set; } = null!;
    public string Description {get; set;} = null!;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryGetDto> Countries { get; set; } = null!;
    public List<ClientTripGetDto> Clients { get; set; } = null!;
}