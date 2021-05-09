# Overview

This income tax calculator app uses an MVC pattern with special emphasis on separation between layers. The core-controller can have multiple alternative service-providers injected to allow flexibility (for example, an alternative ITaxBracketGetter that queries a DB rather than an API).

Special notes:
 - The View was created last, and consists of a bare-bones html page (SimpleView.html).
 - The tax-bracket-provider api's issues were dealt with by:
   - Adding an optional extra query parameter `retries` that decreases "Database not found!" frequency.
   - Attempting even unkown years once, so new data won't be excluded.

# Setup and Run

Setup and run the data-layer via Docker:
 - docker pull ptsdocker16/interview-test-server
 - docker run --init -p 5000:5000 -it ptsdocker16/interview-test-server

Run the Income Tax Calculator API. Options include:
 - Running from Visual Studio.
 - (TODO) Downloading and running [this dockerised version](https://www.docker.com/).

# API Interaction
 
To interface with the API you have 2 main options:
 - Open SimpleView.html
 - Use your browser, or PostMan, against a URL of this format: https://localhost:44359/IncomeTax?year=2020&income=4000&retries=1

# Known Issues

The 'DB' being queried by our API has these issues:
 - It randomly returns a garbage response.
 - It only has values for 2019 and 2020.
 - One of the rate values 2019 is currently setup as a string. A temporary conversion has been put in place.
 
The controller does not check if the tax-bracket-provider API is running. If it isn't you will receive a generic error.

Only very limited checks are being done on the results of tax-bracket-provider. So if it contains bad bracket-data, such as missing brackets, no error will be raised (although some cases will be logged as not taxing all income).

There isn't enough logging taking place outside of `IncomeTaxController`. Given more time all services should eventually have access to an `ILogger` (either via a simple argument, or DI).

# Testing

The calculator portion of this app (the PointIncomeTaxCalculator class) was created using TDD. However, time constraints made more extensive testing impossible. Given time I wanted to include:
 - More edge-case tests for the calculator-logic (such as bad bracket data).
 - Tests for IncomeTaxController using a mocked calculator and ITaxBracketGetter
 - Postman-driven integration tests of the Controllers and Models in action. 

# 3rd party dependencies

These dependencies have been confirmed to work in the latest build. If something breaks inexplicably be sure to check if any have auto-updated.

 - Refit - version 6.0.38
 - Newtonsoft.Json - version 13.0.1
 - NUnit - version 3.12.0
 