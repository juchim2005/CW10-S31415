using System;

namespace CW10_S31415.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
    
}