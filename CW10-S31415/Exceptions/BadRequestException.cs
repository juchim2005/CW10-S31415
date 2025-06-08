using System;

namespace CW10_S31415.Exceptions;

public class BadRequestException(string message) : Exception(message);