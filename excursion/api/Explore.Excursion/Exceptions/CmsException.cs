using System;

namespace Explore.Excursion.Exceptions;

public class CmsException : Exception
{
    public CmsException(string message = "") : base(message)
    {
    }
}