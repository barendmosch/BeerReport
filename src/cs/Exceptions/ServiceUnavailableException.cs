﻿using System;

namespace BeerAppServerSide {
    public class ServiceUnavailableException : Exception {
        public ServiceUnavailableException() { }

        public ServiceUnavailableException(string message)
            : base(message) {
        }

        public ServiceUnavailableException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
