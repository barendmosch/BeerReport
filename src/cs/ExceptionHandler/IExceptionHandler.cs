using System.Net.Http;
using System;

namespace BeerAppServerSide {
    public interface IExceptionHandler {
        HttpResponseMessage ReturnException(Exception e);
    }
}