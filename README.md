# CheckoutChallenge

## Overview
This is a challenge test for Checkout.com

## About this Challenge
This challenge was developed using Microsoft .Net Core and the follow features

- SQL Server 2017 database;
- Entity Framework 6;
- Database encryption;
- Console logging;
- Mocked IBankApprovalService;
- Swagger to ease testing/debuggin;
- Monitorization with Prometheus and Grafana;

![Basic layers diagram](https://github.com/rapontual/CheckoutChallenge/blob/master/diagram.png)

All dependencies are working as docker's containers

The application has two controllers:

###### Security

Provides security for the API access. Has these endpoints:

- /api/security/login: used to login and obtain a JWT access token.
- /api/security/create: creates a new login. This can only be used my an login with admin role. 

###### Payment

Provides access to payments. Has these endpoints:

- /api/payments - POST a payment for the logged in Merchant
- /api/payments - GET all payments of the logged in Merchant
- /api/payments/transactionId - GET the payment transaction of the logged in Merchant

## Running
###### Locally:
Start the Challenge.API in Visual Studio

###### Just the application:
Run the `docker-compose` in appication folder

###### Application and Monitoring
Run the `docker-compose` in /monitoring folder

## How to use
1. Use an RESTful API client like Postman or the Swagger endpoint: http://localhost:45300/swagger
2. Login with the User/Merchant credentials. As a start up, the application will create an admin User with the credentials "challenge/password";
3. Use the token generated in /api/security/login endpoint
4. If using the Swagger, click the Authorize button and enter "beaer token" with the generated token
![See how to authorize using Swagger](https://user-images.githubusercontent.com/8179423/99880430-24e23900-2c0b-11eb-98fb-241c6a7e16d1.png)
5. POST/GET payments using api/payments endpoint
