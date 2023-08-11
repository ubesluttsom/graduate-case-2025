using System;

namespace Company.Api.Exceptions;

public class CmsException : Exception
{
    public CmsException(string message = "") : base(message)
    {
    }
}