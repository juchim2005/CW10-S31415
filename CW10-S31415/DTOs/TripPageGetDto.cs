using System.Collections.Generic;

namespace CW10_S31415.DTOs;

public class TripPageGetDto
{
    public int PageNum {get; set;}
    public int PageSize {get; set;}
    public int AllPages {get; set;}
    public List<TripGetDto> Trips {get; set;}
}