using System;

namespace BeerAppServerSide {
    public class BlobDoesNotExistException : Exception {
        public BlobDoesNotExistException() { }

        public BlobDoesNotExistException(string message)
            : base(message) {
        }

        public BlobDoesNotExistException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
