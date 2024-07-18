# logifuture-wallet

In this sample App, I used Inmemory DB instead of SQL server for demonstration proposes,
Db configuration should be changed to SQL server with SQLServer connection strings(as example- Azure SQL)

In a real-world app, it should be either a cloud-based SQL server or self-hosted with several replicas

Service doesn't have Auth and permissions validation logic to simplify the logic

Service doesn't have Exception handling/Exception conversion Middleware (to provide user-friendly error codes instead of 500 errors)

To not add User controller with Crud operations nonexisting User created during Wallet creation

In the implementation requests processing from all endpoints goes to IWalletService/IWalletRepository to not spend a lot of time on Repo implementations
but in the final solution Services and Repositories should be created per Domain object
So Transactions should be managed by TransactionsRepository
In the case of user model extension - user management should be performed within the UserRepository

## Rest endpoints

There are several rest endpoints to communicate with the service:

Create Wallet:
```Rest
POST
/api/Wallet - 
[userId] - string($uuid) User Id(will be created if empty)
[currecny] - Wallet currency 


Get Wallet
GET
/api/Wallet/{userId}/{walletId} 


Get All User Wallets
GET
/api/Wallet/{userId}


Add/Remove funds to wallet
POST
/api/Wallet/{walletId}/chage-funds
[Operation] Add/Remove
[Amount] Amount
[Timestamp] DateTime of the operation


Get All Wallet transactions
GET
"{userId}/{walletId}/transactions


Freeze Wallet
POST
"{userId}/{walletId}/transactions

```

## Middleware 
* There CorrelationIdMiddleware
to request Third-party service CorrelationId/TransactionId from a 3-party system
* The real-world project should have permissions validation logic (as Ex: token based with token received from the Auth service)



All the endpoints are accessible via Swagger 

The project can be run via docker:
```bash
docker build -t wallet-api -f Dockerfile .
```

Auto service scaling can performed via Kubernetes cluster configuration  (3-5 instances with 70% CPU)
```bash
kubectl apply -f api-deployment.yaml 
kubectl apply -f hpa.yaml
```

