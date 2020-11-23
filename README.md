# CheckoutChallenge

## Overview
This is a challenge test for Checkout.com

## About this Challenge
This challenge was developed using:

- Microsoft .Net Core;
- SQL Server 2017 database;
- Entity Framework 6;
- Database encryption;
- Console logging;
- Mocked IBankApprovalService;
- Swagger to ease testing/debugging;
- Monitorization with Prometheus and Grafana;

![Basic layers diagram](https://github.com/rapontual/CheckoutChallenge/blob/master/diagram.png)

All dependencies are working as docker's containers

The application has two controllers:

###### Security

Provides security for the API access. Has these endpoints:

- /api/security/login: used to login and obtain a JWT access token.
- /api/security/create: creates a new login. This can only be used by an user with admin role. 

###### Payment

Provides access to payments. Has these endpoints:

- /api/payments - POST a payment for the logged User/Merchant
- /api/payments - GET all payments of the logged User/Merchant
- /api/payments/transactionId - GET the payment transaction of the logged User/Merchant

## Running

###### Application and Monitoring
Run the `docker-compose` in application folder

###### Locally
Start the Challenge.API in Visual Studio
A SQL Server instance on 1433 port is necessary. If using an existing SQL Server instance, change the connection string in appsettings.json file.
To use the container (recommended), run docker-compose first to create the container, than run the application. The other containers can be stopped by command line or your prefered container administraton tool, like Docker Dashboard or Kitematic.

## How to use
1. Use an RESTful API client like Postman or the Swagger endpoint: http://localhost:45300/swagger
2. Login with the User/Merchant credentials. When started, the application will create an admin User with the credentials "challenge/password";
3. Use the token generated in /api/security/login endpoint
4. If using the Swagger, click the Authorize button and enter "beaer token" with the generated token
![See how to authorize using Swagger](https://user-images.githubusercontent.com/8179423/99880430-24e23900-2c0b-11eb-98fb-241c6a7e16d1.png)
5. POST/GET payments to api/payments endpoint using the Swagger endpoints or you preferred tool, like Postman or CURL

## Monitoring
The application is monitored by Prometheus, which collects the metrics and by Grafana, which uses dashboards to show the metrics.

- To access Prometheus: http://localhost:9090/
- Prometheus metrics: http://localhost:9090/graph?g0.range_input=1h&g0.expr=http_request_duration_seconds_count&g0.tab=1&g1.range_input=1h&g1.expr=http_requests_received_total&g1.tab=1
- Prometheus graphics: http://localhost:9090/graph?g0.range_input=5m&g0.expr=http_request_duration_seconds_count&g0.tab=0&g1.range_input=5m&g1.expr=http_requests_received_total&g1.tab=0

- To access Grafana (login is admin/P@ssw0rd): http://localhost:3000  

## Final notes
- There's unit tests for all components, only missing (up to now) tests for GET payments;
- The goal is to provide all access between controllers and services/data using the `Challenge.Services` layer, but it's missing migrate the repositories;
- There's a fake class to mock IBankApprovalService (bank client service);
- I only created (up to now) one diagram to document the application structure;
- I used the table Merchant to store the User/Login data.

