# ServerDemo

A basic web service template in C# consististing of a MongoDB database layer, service layer and controller layer, with associated NUnit tests.

The service is based on a simple book model, with entities having an ID, name, author and year.

The service can be run locally with MongoDB, or used online with the appropriate settings being overwritten.

The service uses dependency injection, which allows easy unit testing with mocked dependencies.
